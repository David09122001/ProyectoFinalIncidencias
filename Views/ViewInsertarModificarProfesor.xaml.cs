using ProjecteFinal.Models;
using ProjecteFinal.ViewModels;
using System;

namespace ProjecteFinal.Views
{
    public partial class ViewInsertarModificarProfesor : ContentPage
    {
        private InsertarModificarProfesorVM vm;

        public ViewInsertarModificarProfesor(Profesor profesor = null)
        {
            InitializeComponent();
            BindingContext = vm = new InsertarModificarProfesorVM(profesor);
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (BindingContext is InsertarModificarProfesorVM vm)
            {
                bool resultado = await vm.GuardarProfesorAsync();
                if (resultado)
                {
                    await DisplayAlert("Éxito", "El profesor ha sido guardado correctamente.", "Aceptar");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo guardar el profesor, verifica los datos ingresados.", "Aceptar");
                }
            }
        }


        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Cancelar", "¿Estás seguro de cancelar? Los cambios no se guardarán.", "Sí", "No");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
