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
        private RolDAO rolDAO;

        public Profesor Profesor { get; private set; }

        public string NombreRol { get; private set; }
        public string ContraseñaActual { get; set; }
        public string NuevaContraseña { get; set; }
        public string ConfirmarNuevaContraseña { get; set; }

        public PerfilVM(Profesor profesor)
        {
            profesorDAO = new ProfesorDAO();
            rolDAO = new RolDAO();
            Profesor = profesor;
            _ = CargarRolAsync(profesor.rol_id);
        }

        private async Task CargarRolAsync(int rolId)
        {
            var rol = await rolDAO.ObtenerRolPorIdAsync(rolId);
            NombreRol = rol?.nombre ?? "Rol no asignado";
            OnPropertyChanged(nameof(NombreRol));
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
