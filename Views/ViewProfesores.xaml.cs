using ProjecteFinal.Models;
using ProjecteFinal.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace ProjecteFinal.Views
{
    public partial class ViewProfesores : ContentPage
    {
        private ProfesoresVM vm;

        public ViewProfesores()
        {
            InitializeComponent();
            BindingContext = vm = new ProfesoresVM();
        }

        private async void OnInsertarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewInsertarModificarProfesor(null, vm));
        }

        private async void OnModificarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var profesor = button?.CommandParameter as Profesor;

            if (profesor != null)
            {
                await Navigation.PushAsync(new ViewInsertarModificarProfesor(profesor, vm));
            }
        }


        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var profesor = button?.CommandParameter as Profesor;

            if (profesor != null && await DisplayAlert("Confirmar", $"¿Estás seguro de que deseas eliminar al profesor {profesor.nombre}?", "Sí", "No"))
            {
                if (vm.EliminarProfesor(profesor))
                {
                    await DisplayAlert("Éxito", "Profesor eliminado correctamente.", "Aceptar");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo eliminar al profesor.", "Aceptar");
                }
            }
        }
    }
}
