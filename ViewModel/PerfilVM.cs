using ProjecteFinal.Base;
using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModel
{
    public class PerfilVM : BaseViewModel
    {
        private ProfesorDAO profesorDAO;

        public Profesor Profesor { get; private set; }

        public string ContraseñaActual { get; set; }
        public string NuevaContraseña { get; set; }
        public string ConfirmarNuevaContraseña { get; set; }

        public PerfilVM(Profesor profesor)
        {
            profesorDAO = new ProfesorDAO();
            Profesor = profesor;
        }

        public async Task CargarProfesorPorCorreoAsync(string correo)
        {
            Profesor = await profesorDAO.ObtenerProfesorPorCorreoAsync(correo);
            if (Profesor == null)
            {
                throw new InvalidOperationException("No se encontró ningún profesor con este correo.");
            }
        }


        public async Task<bool> GuardarNuevaContraseñaAsync()
        {
            if (string.IsNullOrWhiteSpace(ContraseñaActual) ||
                string.IsNullOrWhiteSpace(NuevaContraseña) ||
                string.IsNullOrWhiteSpace(ConfirmarNuevaContraseña))
            {
                throw new InvalidOperationException("Todos los campos son obligatorios.");
            }

            if (NuevaContraseña != ConfirmarNuevaContraseña)
            {
                throw new InvalidOperationException("Las nuevas contraseñas no coinciden.");
            }

            if (ContraseñaActual != Profesor.contrasena)
            {
                throw new InvalidOperationException("La contraseña actual es incorrecta.");
            }

            Profesor.contrasena = NuevaContraseña;
            await profesorDAO.ActualizarProfesorAsync(Profesor);

            return true;
        }
    }
}
