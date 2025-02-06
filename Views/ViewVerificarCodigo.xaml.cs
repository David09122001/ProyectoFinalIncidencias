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
                // C�digo correcto, permitir cambio de contrase�a
                await Navigation.PushAsync(new ViewCambiarContrasena(profesor));
            }
            else
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "El c�digo ingresado es incorrecto.";
            }
        }
    }
}
