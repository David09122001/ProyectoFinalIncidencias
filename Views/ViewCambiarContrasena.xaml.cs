using GestorIncidencias.DAO;
using GestorIncidencias.Models;
using System;

namespace GestorIncidencias.Views
{
    public partial class ViewCambiarContrasena : ContentPage
    {
        private readonly Profesor profesor;
        private readonly ProfesorDAO profesorDAO;

        public ViewCambiarContrasena(Profesor profesor)
        {
            InitializeComponent();
            this.profesor = profesor;
            profesorDAO = new ProfesorDAO();
        }

        private async void GuardarContrasenaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NuevaContrasenaEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmarContrasenaEntry.Text))
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "Todos los campos son obligatorios.";
                return;
            }

            if (NuevaContrasenaEntry.Text != ConfirmarContrasenaEntry.Text)
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "Las contrase�as no coinciden.";
                return;
            }

            // Guardar la nueva contrase�a en la base de datos
            profesor.contrasena = NuevaContrasenaEntry.Text;
            await profesorDAO.ActualizarProfesorAsync(profesor);

            await DisplayAlert("�xito", "Contrase�a actualizada correctamente.", "Aceptar");
            await Navigation.PopToRootAsync(); // Volver al login
        }
    }
}
