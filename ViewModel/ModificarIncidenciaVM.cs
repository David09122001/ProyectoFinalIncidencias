using ProjecteFinal.Models;
using ProjecteFinal.DAO;
using ProjecteFinal.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ProjecteFinal.Views;
using System.Net.Mail;
using System.Net;

namespace ProjecteFinal.ViewModel
{
    public class ModificarIncidenciaVM : BaseViewModel
    {
        private IncidenciaDAO incidenciaDAO = new IncidenciaDAO();
        private AdjuntoDAO adjuntoDAO = new AdjuntoDAO();
        private ProfesorDAO profesorDAO = new ProfesorDAO();

        public Incidencia Incidencia { get; set; }
        public ObservableCollection<Adjunto> Adjuntos { get; set; } = new ObservableCollection<Adjunto>();

        public ObservableCollection<string> Estados
        {
            get
            {
                var estadosDisponibles = new ObservableCollection<string> { "Resolviendo", "Solucionada", "Inviable" };

                // Asegurar que "Pendiente" y "Comunicada" se muestren si son el estado actual
                if (_estadoSeleccionado == "Pendiente" || _estadoSeleccionado == "Comunicada")
                {
                    estadosDisponibles.Insert(0, _estadoSeleccionado);
                }

                return estadosDisponibles;
            }
        }




        private string _estadoSeleccionado;
        private string _estadoAnterior;

        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (_estadoSeleccionado != value && !string.IsNullOrWhiteSpace(value))
                {
                    _estadoSeleccionado = value;
                    Incidencia.estado = value; // Sincronizar con el modelo
                    Console.WriteLine($"EstadoSeleccionado actualizado a: {value}"); // Depuración
                    OnPropertyChanged(nameof(EstadoSeleccionado));
                }
            }
        }



        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Hardware", "Software", "Red" };

        private string _tipoSeleccionado;
        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                if (_tipoSeleccionado != value)
                {
                    // Llamar al método para confirmar el cambio de subtipo
                    _ = ConfirmarCambioTipoAsync(value);
                }
            }
        }


        public bool MostrarHW => TipoSeleccionado == "Hardware";
        public bool MostrarSW => TipoSeleccionado == "Software";
        public bool MostrarRed => TipoSeleccionado == "Red";

        public Incidencia_HW IncidenciaHW { get; set; } = new Incidencia_HW();
        public Incidencia_SW IncidenciaSW { get; set; } = new Incidencia_SW();
        public Incidencia_Red IncidenciaRed { get; set; } = new Incidencia_Red();

        public ICommand EliminarAdjuntoCommand { get; }

        private string _profesorResponsable;
        public string ProfesorResponsable
        {
            get => _profesorResponsable;
            set
            {
                _profesorResponsable = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _fechaResolucionOriginal;

        private string _estadoInicial;
        public ModificarIncidenciaVM(Incidencia incidencia)
        {
            Incidencia = incidencia;

            // Inicializar EstadoSeleccionado desde la incidencia
            _estadoSeleccionado = incidencia.estado ?? "Pendiente";
            Incidencia.estado = _estadoSeleccionado;
            _estadoInicial = incidencia.estado;

            TipoSeleccionado = DeterminarTipoDeIncidencia();

            CargarProfesorAsignadoAsync().ConfigureAwait(false);
            _fechaResolucionOriginal = incidencia.fechaResolucion;

            EliminarAdjuntoCommand = new Command<Adjunto>(EliminarAdjuntoAsync);

            CargarDatosAsync();
        }




        private async Task CargarProfesorAsignadoAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Incidencia.responsableDni))
                {
                    var profesor = await profesorDAO.BuscarPorDniAsync(Incidencia.responsableDni);

                    if (profesor != null)
                    {
                        ProfesorResponsable = $"{profesor.nombre} ({profesor.email})";
                    }
                    else if (Incidencia.responsableDni == "SAI")
                    {
                        ProfesorResponsable = "Servicio de Asistencia Informática (SAI)";
                    }
                    else
                    {
                        ProfesorResponsable = "Responsable no encontrado";
                    }
                }
                else
                {
                    ProfesorResponsable = "Ningún profesor asignado";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el profesor asignado: {ex.Message}");
                ProfesorResponsable = "Error al cargar responsable";
            }

            OnPropertyChanged(nameof(ProfesorResponsable));
        }


        private async void CargarDatosAsync()
        {
            await CargarAdjuntosAsync();
            await CargarDatosSubtipoAsync();
        }

        private void ActualizarPicker()
        {
            OnPropertyChanged(nameof(Estados)); // Refrescar la lista de estados
            OnPropertyChanged(nameof(EstadoSeleccionado)); // Refrescar el valor seleccionado
        }

        private void CambiarEstadoComunicada()
        {
            if (!string.IsNullOrWhiteSpace(Incidencia.responsableDni))
            {
                Incidencia.estado = "Comunicada";
                EstadoSeleccionado = "Comunicada"; // Refrescar el picker
                Console.WriteLine($"Estado cambiado a: {EstadoSeleccionado}"); // Depuración
                OnPropertyChanged(nameof(EstadoSeleccionado));
                OnPropertyChanged(nameof(Estados));
            }
        }


        public async Task CambiarEstadoAsync(string nuevoEstado)
        {
            // Si el estado no cambia, no hacemos nada
            if (_estadoSeleccionado == nuevoEstado)
                return;

            // Validar si intenta cambiar a "Resolviendo" sin un profesor responsable asignado
            if (nuevoEstado == "Resolviendo" && string.IsNullOrWhiteSpace(Incidencia.responsableDni))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Asignación requerida",
                    "Para cambiar el estado a 'Resolviendo', debes asignar un profesor responsable.",
                    "Aceptar");
                return;
            }

            // Confirmar cambio de estado
            bool confirm = await App.Current.MainPage.DisplayAlert(
                "Cambiar Estado",
                $"¿Estás seguro de que quieres cambiar el estado a '{nuevoEstado}'?",
                "Sí",
                "No");

            if (!confirm)
                return; // Si el usuario cancela, no hacemos el cambio

            // Cambiar el estado seleccionado
            _estadoSeleccionado = nuevoEstado;

            // Notificar el cambio
            OnPropertyChanged(nameof(EstadoSeleccionado));
        }




        private async Task ConfirmarCambioTipoAsync(string nuevoTipo)
        {
            if (!string.IsNullOrWhiteSpace(_tipoSeleccionado))
            {
                bool confirm = await App.Current.MainPage.DisplayAlert(
                    "Cambiar Tipo de Incidencia",
                    "Al cambiar el tipo de incidencia, los datos específicos del tipo actual se perderán. ¿Desea continuar?",
                    "Sí",
                    "No");

                if (!confirm)
                    return; // Si el usuario cancela, no hacemos el cambio
            }

            // Limpiar datos específicos del tipo anterior
            if (_tipoSeleccionado == "Hardware")
                IncidenciaHW = null; // Reiniciar a null
            else if (_tipoSeleccionado == "Software")
                IncidenciaSW = null; // Reiniciar a null
            else if (_tipoSeleccionado == "Red")
                IncidenciaRed = null; // Reiniciar a null

            // Asignar el nuevo subtipo
            _tipoSeleccionado = nuevoTipo;

            // Inicializar nuevos datos del tipo seleccionado
            if (nuevoTipo == "Hardware")
                IncidenciaHW = new Incidencia_HW();
            else if (nuevoTipo == "Software")
                IncidenciaSW = new Incidencia_SW();
            else if (nuevoTipo == "Red")
                IncidenciaRed = new Incidencia_Red();

            // Notificar los cambios
            OnPropertyChanged(nameof(TipoSeleccionado));
            OnPropertyChanged(nameof(MostrarHW));
            OnPropertyChanged(nameof(MostrarSW));
            OnPropertyChanged(nameof(MostrarRed));
            OnPropertyChanged(nameof(IncidenciaHW));
            OnPropertyChanged(nameof(IncidenciaSW));
            OnPropertyChanged(nameof(IncidenciaRed));
        }


        private async Task CargarDatosSubtipoAsync()
        {
            if (TipoSeleccionado == "Hardware")
            {
                IncidenciaHW = await incidenciaDAO.ObtenerSubtipoHardwareAsync(Incidencia.id);
                OnPropertyChanged(nameof(IncidenciaHW));
            }
            else if (TipoSeleccionado == "Software")
            {
                IncidenciaSW = await incidenciaDAO.ObtenerSubtipoSoftwareAsync(Incidencia.id);
                OnPropertyChanged(nameof(IncidenciaSW));
            }
            else if (TipoSeleccionado == "Red")
            {
                IncidenciaRed = await incidenciaDAO.ObtenerSubtipoRedAsync(Incidencia.id);
                OnPropertyChanged(nameof(IncidenciaRed));
            }

            OnPropertyChanged(nameof(MostrarHW));
            OnPropertyChanged(nameof(MostrarSW));
            OnPropertyChanged(nameof(MostrarRed));
        }

        private async Task CargarAdjuntosAsync()
        {
            var adjuntos = await adjuntoDAO.ObtenerAdjuntosPorIncidenciaAsync(Incidencia.id);
            Adjuntos.Clear();
            foreach (var adjunto in adjuntos)
            {
                Adjuntos.Add(adjunto);
            }
        }

        private string DeterminarTipoDeIncidencia()
        {
            if (incidenciaDAO.EsIncidenciaHardware(Incidencia.id)) return "Hardware";
            if (incidenciaDAO.EsIncidenciaSoftware(Incidencia.id)) return "Software";
            if (incidenciaDAO.EsIncidenciaRed(Incidencia.id)) return "Red";
            return null;
        }
        public async Task GuardarCambiosAsync()
        {
            // Validar que los campos obligatorios no estén vacíos
            if (string.IsNullOrWhiteSpace(Incidencia.descripcionDetallada) ||
                string.IsNullOrWhiteSpace(Incidencia.aulaUbicacion) ||
                Incidencia.fechaIncidencia == default)
            {
                throw new InvalidOperationException("Completa todos los campos obligatorios.");
            }

            // Validar que el estado "Resolviendo" solo sea posible si hay un profesor asignado
            if (EstadoSeleccionado == "Resolviendo" && string.IsNullOrWhiteSpace(Incidencia.responsableDni))
            {
                throw new InvalidOperationException("No puedes asignar el estado 'Resolviendo' sin seleccionar un profesor.");
            }

            // Si el estado es null o "Pendiente" y hay un profesor asignado, cambiar a "Comunicada"
            if (!string.IsNullOrWhiteSpace(Incidencia.responsableDni) &&
                (string.IsNullOrWhiteSpace(Incidencia.estado) || Incidencia.estado == "Pendiente"))
            {
                CambiarEstadoComunicada();
            }

            // Validar que el estado no sea null antes de guardar
            if (string.IsNullOrWhiteSpace(Incidencia.estado))
            {
                throw new InvalidOperationException("El estado de la incidencia no puede ser nulo.");
            }

            // Detectar cambios de estado
            bool estadoCambiado = _estadoInicial != Incidencia.estado;

            // Guardar en la base de datos
            Console.WriteLine($"Guardando incidencia con estado: {Incidencia.estado}"); // Depuración
            await incidenciaDAO.ActualizarIncidenciaAsync(Incidencia);

            // Manejar subtipo y adjuntos
            await ManejarSubtipoAsync();
            await ManejarAdjuntosAsync();

            // Si el estado cambió, registrar un log y enviar correos
            if (estadoCambiado)
            {
                await RegistrarLogAsync(); // Registrar el log
                await EnviarCorreosCambioEstadoAsync(); // Enviar correos
            }

            // Actualizar el estado inicial para futuras ediciones
            _estadoInicial = Incidencia.estado;

            // Refrescar el picker después de guardar
            OnPropertyChanged(nameof(EstadoSeleccionado));
            OnPropertyChanged(nameof(Estados));
        }


        private async Task EnviarCorreosCambioEstadoAsync()
        {
            try
            {
                var destinatarios = new List<string>();

                // Profesor que creó la incidencia
                var profesorCreador = await profesorDAO.BuscarPorDniAsync(Incidencia.profesorDni);
                if (profesorCreador != null)
                {
                    destinatarios.Add(profesorCreador.email);
                }

                // Profesor responsable (si existe)
                if (!string.IsNullOrWhiteSpace(Incidencia.responsableDni))
                {
                    var profesorResponsable = await profesorDAO.BuscarPorDniAsync(Incidencia.responsableDni);
                    if (profesorResponsable != null)
                    {
                        destinatarios.Add(profesorResponsable.email);
                    }
                }

                // Coordinador TIC 
                var coordinadorTIC = "david.carcer09@gmail.com";
                destinatarios.Add(coordinadorTIC);

                // Enviar correos a todos los destinatarios
                foreach (var email in destinatarios.Distinct())
                {
                    await EnviarCorreoAsync(email, $"Cambio de estado en la incidencia {Incidencia.id}",
                        $"El estado de la incidencia ha cambiado a '{Incidencia.estado}'.\n\n" +
                        $"Descripción: {Incidencia.descripcionDetallada}\n" +
                        $"Aula: {Incidencia.aulaUbicacion}\n" +
                        $"Fecha de incidencia: {Incidencia.fechaIncidencia:dd/MM/yyyy}\n\n" +
                        $"Por favor, revisa el sistema para más detalles.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correos: {ex.Message}");
            }
        }





        private async Task ManejarSubtipoAsync()
        {
            // Eliminar subtipos anteriores
            await incidenciaDAO.EliminarSubtipoHardwareAsync(Incidencia.id);
            await incidenciaDAO.EliminarSubtipoSoftwareAsync(Incidencia.id);
            await incidenciaDAO.EliminarSubtipoRedAsync(Incidencia.id);

            // Manejar el nuevo subtipo seleccionado
            if (TipoSeleccionado == "Hardware")
            {
                // Validar campos específicos de Hardware
                if (string.IsNullOrWhiteSpace(IncidenciaHW.dispositivo) ||
                    string.IsNullOrWhiteSpace(IncidenciaHW.modelo) ||
                    string.IsNullOrWhiteSpace(IncidenciaHW.numeroSerie))
                {
                    throw new InvalidOperationException("Completa todos los campos específicos de Hardware.");
                }

                // Asignar el ID y guardar el subtipo Hardware
                IncidenciaHW.id = Incidencia.id;
                await incidenciaDAO.AñadirSubtipoHardwareAsync(IncidenciaHW);
            }
            else if (TipoSeleccionado == "Software")
            {
                // Validar campos específicos de Software
                if (string.IsNullOrWhiteSpace(IncidenciaSW.sistemaOperativo) ||
                    string.IsNullOrWhiteSpace(IncidenciaSW.aplicacion))
                {
                    throw new InvalidOperationException("Completa todos los campos específicos de Software.");
                }

                // Asignar el ID y guardar el subtipo Software
                IncidenciaSW.id = Incidencia.id;
                await incidenciaDAO.AñadirSubtipoSoftwareAsync(IncidenciaSW);
            }
            else if (TipoSeleccionado == "Red")
            {
                // Validar campos específicos de Red
                if (string.IsNullOrWhiteSpace(IncidenciaRed.dispositivoAfectado))
                {
                    throw new InvalidOperationException("Completa todos los campos específicos de Red.");
                }

                // Asignar el ID y guardar el subtipo Red
                IncidenciaRed.id = Incidencia.id;
                await incidenciaDAO.AñadirSubtipoRedAsync(IncidenciaRed);
            }
        }


        private async Task ManejarAdjuntosAsync()
        {
            var adjuntosEnDB = await adjuntoDAO.ObtenerAdjuntosPorIncidenciaAsync(Incidencia.id);
            var adjuntosAEliminar = adjuntosEnDB.Where(a => !Adjuntos.Any(local => local.Id == a.Id)).ToList();

            foreach (var adjunto in adjuntosAEliminar)
            {
                await adjuntoDAO.EliminarAdjuntoAsync(adjunto);
            }

            foreach (var adjunto in Adjuntos)
            {
                adjunto.IncidenciaId = Incidencia.id;
                if (adjunto.Id <= 0)
                {
                    await adjuntoDAO.AñadirAdjuntoAsync(adjunto);
                }
                else
                {
                    await adjuntoDAO.ActualizarAdjuntoAsync(adjunto);
                }
            }
        }

        public void AgregarAdjunto(string nombre, string ruta, byte[] datos)
        {
            Adjuntos.Add(new Adjunto { Nombre = nombre, Ruta = ruta, Datos = datos });
        }

        private async void EliminarAdjuntoAsync(Adjunto adjunto)
        {
            if (Adjuntos.Contains(adjunto))
            {
                Adjuntos.Remove(adjunto);

                try
                {
                    await adjuntoDAO.EliminarAdjuntoAsync(adjunto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar el adjunto: {ex.Message}");
                }
            }
        }
        public async Task CambiarProfesorAsync()
        {
            var seleccionarProfesorVM = new SeleccionarProfesorVM();
            seleccionarProfesorVM.ProfesorSeleccionado += (profesor) =>
            {
                Incidencia.responsableDni = profesor.dni;
                ProfesorResponsable = $"{profesor.nombre} ({profesor.email})";
                OnPropertyChanged(nameof(ProfesorResponsable));

                // Cerrar automáticamente la vista de selección
                App.Current.MainPage.Navigation.PopAsync();
            };

            var seleccionarProfesorView = new ViewSeleccionarProfesor
            {
                BindingContext = seleccionarProfesorVM
            };

            await App.Current.MainPage.Navigation.PushAsync(seleccionarProfesorView);
        }

        private async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                MailAddress addressFrom = new MailAddress("iscapopproyecto@gmail.com", "Gestión Incidencias");
                MailAddress addressTo = new MailAddress("david.carcer09@gmail.com");
                MailMessage message = new MailMessage(addressFrom, addressTo)
                {
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = false
                };

                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("iscapopproyecto@gmail.com", "wjre zcur tdxg lakz")
                };

                await Task.Run(() => client.Send(message));
                Console.WriteLine($"Correo enviado correctamente a {destinatario}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo a {destinatario}: {ex.Message}");
            }
        }



        private async Task ValidarEstadoAsync(string nuevoEstado)
        {
            // Si el responsable es el SAI, no validar
            if (Incidencia.responsableDni == "SAI")
            {
                _estadoSeleccionado = nuevoEstado;
                OnPropertyChanged(nameof(EstadoSeleccionado));
                return;
            }

            // Si intenta cambiar a "Comunicada", restaurar el estado anterior
            if (nuevoEstado == "Comunicada")
            {
                await App.Current.MainPage.DisplayAlert(
                    "No permitido",
                    "El estado 'Comunicada' no puede ser seleccionado manualmente.",
                    "Aceptar");

                _estadoSeleccionado = _estadoAnterior;
                OnPropertyChanged(nameof(EstadoSeleccionado));
                return;
            }

            // Validar si intenta seleccionar "Resolviendo" sin profesor responsable asignado
            if (nuevoEstado == "Resolviendo" && string.IsNullOrWhiteSpace(Incidencia.responsableDni))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Asignación requerida",
                    "Para cambiar el estado a 'Resolviendo', debes asignar un profesor responsable.",
                    "Aceptar");

                _estadoSeleccionado = _estadoAnterior;
                OnPropertyChanged(nameof(EstadoSeleccionado));
                return;
            }

            // Cambiar el estado si es válido
            _estadoSeleccionado = nuevoEstado;
            OnPropertyChanged(nameof(EstadoSeleccionado));
        }

        public async Task ResolverPorSAIAsync()
        {
            try
            {
                // Asignar al SAI como responsable
                Incidencia.responsableDni = "SAI";
                ProfesorResponsable = "Servicio de Asistencia Informática (SAI)";
                OnPropertyChanged(nameof(ProfesorResponsable));

                // Cambiar automáticamente el estado a "Comunicada"
                Incidencia.estado = "Comunicada";
                EstadoSeleccionado = "Comunicada"; // Refrescar el picker
                OnPropertyChanged(nameof(EstadoSeleccionado));
                OnPropertyChanged(nameof(Estados));

                // Guardar en la base de datos
                await incidenciaDAO.ActualizarIncidenciaAsync(Incidencia);

                // Enviar correo al SAI
                await EnviarCorreoAlSAIAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asignar incidencia al SAI: {ex.Message}");
            }
        }

        private async Task EnviarCorreoAlSAIAsync()
        {
            try
            {
                string correoDestino = "sai@example.com"; // Dirección de correo del SAI
                string asunto = "Nueva Incidencia Asignada al SAI";

                // Cuerpo del correo con HTML dinámico
                string cuerpo = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <h2 style='color: #007ACC;'>Nueva Incidencia Asignada</h2>
    <p>Estimado equipo del <strong>Servicio de Asistencia Informática (SAI)</strong>,</p>
    <p>Se les ha asignado una nueva incidencia. A continuación, se detallan los datos de la incidencia:</p>
    <table style='border-collapse: collapse; width: 100%; max-width: 600px; margin-top: 20px; font-size: 14px;'>
        <tr>
            <td style='border: 1px solid #ddd; padding: 8px; background-color: #f4f4f4;'><strong>Descripción</strong></td>
            <td style='border: 1px solid #ddd; padding: 8px;'>{Incidencia.descripcionDetallada}</td>
        </tr>
        <tr>
            <td style='border: 1px solid #ddd; padding: 8px; background-color: #f4f4f4;'><strong>Aula</strong></td>
            <td style='border: 1px solid #ddd; padding: 8px;'>{Incidencia.aulaUbicacion}</td>
        </tr>
        <tr>
            <td style='border: 1px solid #ddd; padding: 8px; background-color: #f4f4f4;'><strong>Fecha de Incidencia</strong></td>
            <td style='border: 1px solid #ddd; padding: 8px;'>{Incidencia.fechaIncidencia:dd/MM/yyyy}</td>
        </tr>
        <tr>
            <td style='border: 1px solid #ddd; padding: 8px; background-color: #f4f4f4;'><strong>Tipo</strong></td>
            <td style='border: 1px solid #ddd; padding: 8px;'>{TipoSeleccionado}</td>
        </tr>
    </table>
    <p style='margin-top: 20px;'>Por favor, gestionen esta incidencia a la mayor brevedad posible.</p>
    <p>Gracias,</p>
    <p style='color: #007ACC;'><strong>Sistema de Gestión de Incidencias</strong></p>
</body>
</html>";

                // Configuración del correo
                MailAddress addressFrom = new MailAddress("iscapopproyecto@gmail.com", "Gestión Incidencias");
                MailAddress addressTo = new MailAddress(correoDestino);
                MailMessage message = new MailMessage(addressFrom, addressTo)
                {
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = true
                };

                // Configuración del cliente SMTP
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("iscapopproyecto@gmail.com", "wjre zcur tdxg lakz")
                };

                await Task.Run(() => client.Send(message));
                Console.WriteLine("Correo enviado correctamente al SAI.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo al SAI: {ex.Message}");
            }
        }

        private async Task RegistrarLogAsync()
        {
            try
            {
                var log = new Log
                {
                    incidenciaId = Incidencia.id,
                    estado = Incidencia.estado, 
                    fecha = DateTime.Now
                };

                await new LogDAO().AñadirLogAsync(log);
                Console.WriteLine($"Log registrado: Incidencia {log.incidenciaId}, Estado {log.estado}, Fecha {log.fecha}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el log: {ex.Message}");
            }
        }




    }
}
