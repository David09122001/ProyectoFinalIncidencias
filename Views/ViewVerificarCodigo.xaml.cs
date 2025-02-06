using GestorIncidencias.Models;
using System;

namespace GestorIncidencias.Views
{
    public partial class ViewVerificarCodigo : ContentPage
    {
        private readonly Profesor profesor;
        private readonly string codigoCorrecto;

        public ViewVerificarCodigo(Profesor profesor, string codigo)
        {
            InitializeComponent();
            this.profesor = profesor;
            this.codigoCorrecto = codigo;
        }

        private async void VerificarCodigoClicked(object sender, EventArgs e)
        {
            if (CodigoEntry.Text == codigoCorrecto)
            {
                // Código correcto, permitir cambio de contraseña
                await Navigation.PushAsync(new ViewCambiarContrasena(profesor));
            }
            else
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "El código ingresado es incorrecto.";
            }
        }
    }
}
