using ProjecteFinal.Models;
using ProjecteFinal.DAO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProjecteFinal.Base;
using System.Windows.Input;
using System.Net.Mail;
using System.Net;

namespace ProjecteFinal.ViewModel
{
    public class InsertarIncidenciaVM : BaseViewModel
    {
        private IncidenciaDAO incidenciaDAO = new IncidenciaDAO();
        private IncidenciaHWDAO incidenciaHWDAO = new IncidenciaHWDAO();
        private IncidenciaSWDAO incidenciaSWDAO = new IncidenciaSWDAO();
        private IncidenciaRedDAO incidenciaRedDAO = new IncidenciaRedDAO();
        private AdjuntoDAO adjuntoDAO = new AdjuntoDAO();

        public ObservableCollection<Adjunto> Adjuntos { get; set; } = new ObservableCollection<Adjunto>();
        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Hardware", "Software", "Red" };

        private string _tipoSeleccionado;
        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                _tipoSeleccionado = value;
                OnPropertyChanged();
                ActualizarFormulario();
            }
        }

        public Incidencia Incidencia { get; set; } = new Incidencia();
        public Incidencia_HW IncidenciaHW { get; set; } = new Incidencia_HW(); 
        public Incidencia_SW IncidenciaSW { get; set; } = new Incidencia_SW(); 
        public Incidencia_Red IncidenciaRed { get; set; } = new Incidencia_Red();

        public bool MostrarHW { get; set; }
        public bool MostrarSW { get; set; }
        public bool MostrarRed { get; set; }
        public ICommand EliminarAdjuntoCommand { get; }

        private readonly SemaphoreSlim _semaforo = new SemaphoreSlim(1, 1); // Máximo 1 tarea simultánea

        private bool _isGuardando = true;
        public bool IsGuardando
        {
            get => _isGuardando;
            set
            {
                _isGuardando = value;
                OnPropertyChanged();
            }
        }

        public InsertarIncidenciaVM(Profesor profesor)
        {
            Incidencia.profesorDni = profesor.dni;
            Incidencia.fechaIncidencia = DateTime.Now;
            EliminarAdjuntoCommand = new Command<Adjunto>(EliminarAdjunto);
        }

        private async void EliminarAdjunto(Adjunto adjunto)
        {
            if (Adjuntos.Contains(adjunto))
            {
                Adjuntos.Remove(adjunto);

                // Eliminar de la base de datos
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

            private void ActualizarFormulario()
        {
            MostrarHW = TipoSeleccionado == "Hardware";
            MostrarSW = TipoSeleccionado == "Software";
            MostrarRed = TipoSeleccionado == "Red";
            OnPropertyChanged(nameof(MostrarHW));
            OnPropertyChanged(nameof(MostrarSW));
            OnPropertyChanged(nameof(MostrarRed));
        }

        private async Task EnviarCorreoAsync()
        {
            try
            {
                string correoDestino = "david.carcer09@gmail.com";
                string asunto = "Nueva Incidencia Reportada";

                // Cuerpo dinámico
                string cuerpo = $@"
        <!DOCTYPE html>
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2 style='color: #007ACC;'>Nueva Incidencia Reportada</h2>
            <p>Estimado equipo de <strong>Mantenimiento TIC</strong>,</p>
            <p>Se ha registrado una nueva incidencia en el sistema de gestión. A continuación, se detallan los datos de la incidencia:</p>
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
            <p style='margin-top: 20px;'>Por favor, revisen esta incidencia a la mayor brevedad posible.</p>
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
                    IsBodyHtml = true // HTML activado
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
                Console.WriteLine("Correo enviado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
            }
        }

        public async Task<bool> GuardarIncidenciaAsync()
        {
            // Intentar adquirir el semáforo; evita múltiples ejecuciones simultáneas
            if (!await _semaforo.WaitAsync(0))
            {
                return false; // Si ya está en uso, no se ejecuta de nuevo
            }

            IsGuardando = false; // Deshabilitar botón

            try
            {
                // Validar campos obligatorios generales
                if (string.IsNullOrWhiteSpace(Incidencia.descripcionDetallada))
                {
                    throw new InvalidOperationException("La descripción detallada es obligatoria.");
                }

                if (string.IsNullOrWhiteSpace(Incidencia.aulaUbicacion))
                {
                    throw new InvalidOperationException("El aula es obligatoria.");
                }

                if (string.IsNullOrWhiteSpace(TipoSeleccionado))
                {
                    throw new InvalidOperationException("Debes seleccionar un tipo de incidencia.");
                }

                // Determinar estado inicial
                if (string.IsNullOrWhiteSpace(Incidencia.estado))
                {
                    Incidencia.estado = "Pendiente";
                }

                // Guardar incidencia principal
                await incidenciaDAO.AñadirIncidenciaAsync(Incidencia);

                // Guardar subtipos
                if (TipoSeleccionado == "Hardware")
                {
                    if (string.IsNullOrWhiteSpace(IncidenciaHW.dispositivo))
                    {
                        throw new InvalidOperationException("Completa los datos específicos de Hardware.");
                    }

                    IncidenciaHW.id = Incidencia.id;
                    await incidenciaHWDAO.AñadirIncidenciaHWAsync(IncidenciaHW);
                }
                else if (TipoSeleccionado == "Software")
                {
                    if (string.IsNullOrWhiteSpace(IncidenciaSW.sistemaOperativo))
                    {
                        throw new InvalidOperationException("Completa los datos específicos de Software.");
                    }

                    IncidenciaSW.id = Incidencia.id;
                    await incidenciaSWDAO.AñadirIncidenciaSWAsync(IncidenciaSW);
                }
                else if (TipoSeleccionado == "Red")
                {
                    if (string.IsNullOrWhiteSpace(IncidenciaRed.dispositivoAfectado))
                    {
                        throw new InvalidOperationException("Completa los datos específicos de Red.");
                    }

                    IncidenciaRed.id = Incidencia.id;
                    await incidenciaDAO.AñadirSubtipoRedAsync(IncidenciaRed);
                }

                // Guardar adjuntos
                foreach (var adjunto in Adjuntos)
                {
                    adjunto.IncidenciaId = Incidencia.id;
                    await adjuntoDAO.AñadirAdjuntoAsync(adjunto);
                }

                // Enviar correo
                await EnviarCorreoAsync();

                return true; // Éxito
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GuardarIncidenciaAsync: {ex.Message}");
                throw; // Relanzar la excepción para que se maneje en la vista
            }
            finally
            {
                IsGuardando = true; // Rehabilitar botón
                _semaforo.Release(); // Liberar el semáforo para permitir nuevas ejecuciones
            }
        }



        public void AgregarAdjunto(string nombre, string ruta, byte[] datos)
        {
            Adjuntos.Add(new Adjunto { Nombre = nombre, Ruta = ruta, Datos = datos });
        }
    }
}
