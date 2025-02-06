using GestorIncidencias.DAO;
using GestorIncidencias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorIncidencias.ViewModel
{
    public class LoginVM
    {
        private ProfesorDAO profesorDAO;

        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string MensajeError { get; set; }
        public bool HayError { get; set; }

        public LoginVM()
        {
            profesorDAO = new ProfesorDAO();
            HayError = false;
        }

        public async Task<Profesor> IniciarSesionAsync()
        {
            // Validar campos vacíos
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Contrasena))
            {
                MensajeError = "Por favor, completa todos los campos.";
                HayError = true;
                return null;
            }

            // Buscar usuario en la base de datos
            var usuario = await profesorDAO.ObtenerProfesorPorCorreoAsync(Correo);

            if (usuario != null)
            {
                // Validar contraseña
                if (usuario.contrasena == Contrasena)
                {
                    HayError = false;
                    return usuario; // Usuario autenticado
                }
                else
                {
                    MensajeError = "Contraseña incorrecta.";
                    HayError = true;
                    return null;
                }
            }
            else
            {
                MensajeError = "Usuario no encontrado.";
                HayError = true;
                return null;
            }
        }
    }
}
