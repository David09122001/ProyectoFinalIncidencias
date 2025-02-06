using GestorIncidencias.DAO;
using GestorIncidencias.Models;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GestorIncidencias.ViewModels
{
    public class RestablecerContrasenaVM : INotifyPropertyChanged
    {
        private readonly ProfesorDAO profesorDAO;
        public string CodigoGenerado { get; private set; }
        public Profesor ProfesorEncontrado { get; private set; }
        public string MensajeError { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public RestablecerContrasenaVM()
        {
            profesorDAO = new ProfesorDAO();
        }

        public async Task<bool> EnviarCodigoAsync(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                MensajeError = "Por favor, introduce un correo válido.";
                return false;
            }

            ProfesorEncontrado = await profesorDAO.ObtenerProfesorPorCorreoAsync(correo);

            if (ProfesorEncontrado != null)
            {
                CodigoGenerado = new Random().Next(100000, 999999).ToString();
                return await EnviarCorreoAsync(ProfesorEncontrado.email, CodigoGenerado);
            }
            else
            {
                MensajeError = "No se encontró un usuario con ese correo.";
                return false;
            }
        }

        private async Task<bool> EnviarCorreoAsync(string correoDestino, string codigo)
        {
            try
            {
                string asunto = "Código de Verificación - Restablecimiento de Contraseña";
                string cuerpo = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #007ACC;'>Código de Verificación</h2>
                    <p>Estimado usuario,</p>
                    <p>Usa el siguiente código para restablecer tu contraseña:</p>
                    <h1 style='color: #007ACC; text-align: center;'>{codigo}</h1>
                    <p>Si no solicitaste este cambio, ignora este mensaje.</p>
                    <p><strong>Sistema de Gestión de Incidencias</strong></p>
                </body>
                </html>";

                MailMessage message = new MailMessage
                {
                    From = new MailAddress("iscapopproyecto@gmail.com", "Gestión Incidencias"),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(correoDestino));

                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("iscapopproyecto@gmail.com", "wjre zcur tdxg lakz")
                })
                {
                    await Task.Run(() => client.Send(message));
                    Console.WriteLine("Correo enviado correctamente.");
                }

                return true;
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al enviar correo: {ex.Message}";
                Console.WriteLine(MensajeError);
                return false;
            }
        }
    }
}
