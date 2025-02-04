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

        public ObservableCollection<string> Estados { get; } = new ObservableCollection<string>
            {
                "Sin asignar", "Asignada", "En proceso", "Pendiente", "Resuelta"
            };

        private string _estadoSeleccionado;
        private string _estadoAnterior;

        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (_estadoSeleccionado != value && !string.IsNullOrWhiteSpace(value))
                {
                    // Si se selecciona "Sin asignar" y hay un responsable, mostrar alerta
                    if (value == "Sin asignar" && !string.IsNullOrWhiteSpace(Incidencia.responsableDni))
                    {
                        Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                        {
                            bool confirmacion = await Application.Current.MainPage.DisplayAlert(
                                "Confirmación",
                                "¿Estás seguro de que deseas cambiar a 'Sin asignar'? Se perderá el profesor responsable.",
                                "Sí", "No"
                            );

                            if (confirmacion)
                            {
                                _estadoSeleccionado = value;
                                Incidencia.estado = value;
                                Incidencia.responsableDni = null; // Se elimina el profesor responsable
                                await incidenciaDAO.ActualizarIncidenciaAsync(Incidencia);

                                OnPropertyChanged(nameof(EstadoSeleccionado));
                                OnPropertyChanged(nameof(Incidencia.estado));
                                OnPropertyChanged(nameof(Incidencia.responsableDni));
                            }
                            else
                            {
                                // Cancelar la selección y mantener el estado anterior
                                OnPropertyChanged(nameof(EstadoSeleccionado));
                            }
                        });

                        return;
                    }

                    _estadoSeleccionado = value;
                    Incidencia.estado = value;
                    OnPropertyChanged(nameof(EstadoSeleccionado));
                    OnPropertyChanged(nameof(Incidencia.estado));
                }
            }
        }


        public string _profesorTemporal;

        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Hardware", "Software", "Red" };

        private string _tipoSeleccionado;
        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                if (_tipoSeleccionado != value)
                {
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

        public Profesor ProfesorActual { get; set; }

        private PermisoDAO permisoDAO = new PermisoDAO();
        public ModificarIncidenciaVM(Incidencia incidencia)
        {
            Incidencia = incidencia;


            _estadoSeleccionado = !string.IsNullOrWhiteSpace(incidencia.estado) ? incidencia.estado : "Sin asignar";
            _estadoInicial = Incidencia.estado;

            TipoSeleccionado = DeterminarTipoDeIncidencia();

            _ = CargarProfesorAsignadoAsync();
            //  _ = CargarPermisosAsync();

            _fechaResolucionOriginal = incidencia.fechaResolucion;
            EliminarAdjuntoCommand = new Command<Adjunto>(EliminarAdjuntoAsync);
            CargarDatosAsync();
        }


        private async Task CargarProfesorAsignadoAsync()
        {
            try
            {
                  if (string.IsNullOrWhiteSpace(Incidencia.responsableDni))
                {
                    ProfesorResponsable = "Ningún profesor asignado";
                    return;
                }

                var profesor = await profesorDAO.BuscarPorDniAsync(Incidencia.responsableDni);

                if (profesor != null)
                {
                    ProfesorResponsable = $"{profesor.nombre} ({profesor.email})";
                 }
                else
                {
                    ProfesorResponsable = "Responsable no encontrado";
                  }


            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("ERROR", $"ERROR en CargarProfesorAsignadoAsync: {ex.Message}", "OK");
                ProfesorResponsable = "Error al cargar responsable";
            }

            OnPropertyChanged(nameof(ProfesorResponsable));
        }


        private async void CargarDatosAsync()
        {
            await CargarAdjuntosAsync();
            await CargarDatosSubtipoAsync();
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
                    return;
            }

            if (_tipoSeleccionado == "Hardware")
                IncidenciaHW = null;
            else if (_tipoSeleccionado == "Software")
                IncidenciaSW = null;
            else if (_tipoSeleccionado == "Red")
                IncidenciaRed = null;


            _tipoSeleccionado = nuevoTipo;

            if (nuevoTipo == "Hardware")
                IncidenciaHW = new Incidencia_HW();
            else if (nuevoTipo == "Software")
                IncidenciaSW = new Incidencia_SW();
            else if (nuevoTipo == "Red")
                IncidenciaRed = new Incidencia_Red();

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
        public async Task<bool> GuardarCambiosAsync()
        {
            try
            {

                // No permitir guardar si faltan campos obligatorios
                if (string.IsNullOrWhiteSpace(Incidencia.descripcionDetallada) ||
                    string.IsNullOrWhiteSpace(Incidencia.aulaUbicacion) ||
                    Incidencia.fechaIncidencia == default)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Completa todos los campos obligatorios.", "Aceptar");
                    return false; // NO SE GUARDA, NO SE CIERRA
                }

                // Obtener el correo del usuario autenticado
                string usuarioEmail = Preferences.Get("UsuarioEmail", string.Empty);
                if (string.IsNullOrWhiteSpace(usuarioEmail))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se encontró el correo del usuario.", "Aceptar");
                    return false;
                }

                // Buscar el profesor autenticado
                var profesorActual = await profesorDAO.ObtenerProfesorPorCorreoAsync(usuarioEmail);
                if (profesorActual == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo obtener el usuario actual.", "Aceptar");
                    return false;
                }

                // Comprobar si tiene permisos para asignar/quitar responsables
                bool tienePermiso = (await permisoDAO.ObtenerPermisosPorRolAsync(profesorActual.rol_id))
                                    .Any(p => p.descripcion.Equals("Asignar responsable", StringComparison.OrdinalIgnoreCase));


                // Si el usuario intenta cambiar a "Sin asignar" sin permiso, bloquear el guardado
                if (_estadoSeleccionado == "Sin asignar" && !tienePermiso)
                {
                    await Application.Current.MainPage.DisplayAlert("Acceso Denegado",
                        "No tienes permisos para asignar o quitar un profesor responsable.", "Aceptar");

                     return false;
                }

                // Guardar si el estado es "Asignada" y hay profesor seleccionado 
                if (Incidencia.estado == "Asignada" && string.IsNullOrWhiteSpace(Incidencia.responsableDni) && string.IsNullOrWhiteSpace(_profesorTemporal))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Para marcar la incidencia como 'Asignada', debes seleccionar un profesor responsable.", "Aceptar");
                    return false; // NO SE GUARDA, NO SE CIERRA
                }

                if (!string.IsNullOrWhiteSpace(_profesorTemporal) && string.IsNullOrWhiteSpace(Incidencia.responsableDni))
                {
                    if (Incidencia.estado == "Sin asignar")
                    {
                        Incidencia.estado = "Asignada"; // Solo cambiar si aún estaba en "Sin asignar"
                    }
                    Incidencia.responsableDni = _profesorTemporal;

                    await EnviarCorreoNuevoResponsableAsync(_profesorTemporal);
                }

                //  Aplicar cambios de profesor responsable SOLO si se ha seleccionado uno nuevo
                if (!string.IsNullOrWhiteSpace(_profesorTemporal))
                {
                    Incidencia.responsableDni = _profesorTemporal; //guarda el cambio en la base de datos
                }

                // Si el estado es "Sin asignar", borrar profesor responsable en la BD
                if (Incidencia.estado == "Sin asignar")
                {
                    Incidencia.responsableDni = null;
                    OnPropertyChanged(nameof(Incidencia.responsableDni));
                    // Guarda en la BD
                    await incidenciaDAO.ActualizarIncidenciaAsync(Incidencia);

                  
                }

                // Si la incidencia se marca como "Resuelta", asignamos la fecha de resolución 
                if (Incidencia.estado == "Resuelta" && !Incidencia.fechaResolucion.HasValue)
                {
                    Incidencia.fechaResolucion = DateTime.Now;
                }

                bool estadoCambiado = _estadoInicial != Incidencia.estado;

                // Guardar
                await incidenciaDAO.ActualizarIncidenciaAsync(Incidencia);
                await ManejarSubtipoAsync();
                await ManejarAdjuntosAsync();

                // Si cambia el estado, registrar log y enviar correo
                if (estadoCambiado)
                {
                    await RegistrarLogAsync();
                    await EnviarCorreosCambioEstadoAsync();
                }

                _estadoInicial = Incidencia.estado;

                // Refrescar lista
                if (Application.Current.MainPage is Shell shell)
                {
                    var viewIncidencias = shell.CurrentPage as ViewIncidencias;
                    if (viewIncidencias != null)
                    {
                        await viewIncidencias.RefrescarIncidencias(); 
                    }
                }

                OnPropertyChanged(nameof(EstadoSeleccionado));
                OnPropertyChanged(nameof(Estados));
                OnPropertyChanged(nameof(ProfesorResponsable));

                return true; //cerrar vista
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"⚠️ Error al guardar: {ex.Message}", "Aceptar");
                return false;  //que no se cierre la vista
            }
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
            string usuarioEmail = Preferences.Get("UsuarioEmail", string.Empty);

            if (string.IsNullOrWhiteSpace(usuarioEmail))
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se encontró el correo del usuario en Preferences.", "Aceptar");
                return;
            }

            var profesorActual = await profesorDAO.ObtenerProfesorPorCorreoAsync(usuarioEmail);

            if (profesorActual == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo encontrar el profesor en la base de datos.", "Aceptar");
                return;
            }
            
            bool tienePermiso = (await permisoDAO.ObtenerPermisosPorRolAsync(profesorActual.rol_id))
                                .Any(p => p.descripcion.Equals("Asignar responsable", StringComparison.OrdinalIgnoreCase));

            if (!tienePermiso)
            {
                await App.Current.MainPage.DisplayAlert("Acceso Denegado",
                    "No tienes permisos para asignar un profesor responsable.", "Aceptar");
                return; 
            }

            var seleccionarProfesorVM = new SeleccionarProfesorVM();

            seleccionarProfesorVM.ProfesorSeleccionado += async (profesor) =>
            {
                if (profesor != null)
                {
                    _profesorTemporal = profesor.dni;
                    ProfesorResponsable = $"{profesor.nombre} ({profesor.email})";

                    OnPropertyChanged(nameof(ProfesorResponsable));

                    await App.Current.MainPage.Navigation.PopAsync();
                }
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo a {destinatario}: {ex.Message}");
            }
        }


        public async Task ResolverPorSAIAsync()
        {
            try
            {
                // Cambiar automáticamente el estado a "Pendiente"
                Incidencia.estado = "Pendiente";
                EstadoSeleccionado = "Pendiente";
                OnPropertyChanged(nameof(EstadoSeleccionado));
                OnPropertyChanged(nameof(Estados));

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
                string correoDestino = "david.carcer09@gmail.com"; // Dirección de correo del SAI
                string asunto = "Nueva Incidencia Asignada al SAI";

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
                                    <p>Gracias.</p>
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

        private async Task EnviarCorreoNuevoResponsableAsync(string dniProfesor)
        {
            try
            {
                var profesorAsignado = await profesorDAO.BuscarPorDniAsync(dniProfesor);
                if (profesorAsignado == null) return;

                string asunto = $"Nueva incidencia asignada: {Incidencia.id}";
                string cuerpo = $@"
            Estimado/a {profesorAsignado.nombre},
            
            Se le ha asignado una nueva incidencia en el sistema de gestión de incidencias.
            
            Detalles:
            - Descripción: {Incidencia.descripcionDetallada}
            - Aula: {Incidencia.aulaUbicacion}
            - Fecha de incidencia: {Incidencia.fechaIncidencia:dd/MM/yyyy}
            - Estado: {Incidencia.estado}
            
            Por favor, revise el sistema para más información.";

                await EnviarCorreoAsync(profesorAsignado.email, asunto, cuerpo);
                Console.WriteLine($"✅ Correo enviado a nuevo responsable: {profesorAsignado.email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al enviar correo al nuevo responsable: {ex.Message}");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el log: {ex.Message}");
            }
        }




    }
}