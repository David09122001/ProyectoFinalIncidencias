using GestorIncidencias.Models;
using GestorIncidencias.ViewModel;
using GestorIncidencias.ViewModels;
using System;

namespace GestorIncidencias.Views
{
    public partial class ViewModificarProfesor : ContentPage
    {
        private ModificarProfesorVM vm;

        private readonly ProfesoresVM _profesoresVM;

        public ViewModificarProfesor(Profesor profesor = null, ProfesoresVM profesoresVM = null)
        {
            InitializeComponent();
            BindingContext = new ModificarProfesorVM(profesor);
            _profesoresVM = profesoresVM;
        }


        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (BindingContext is ModificarProfesorVM vm)
            {
                bool resultado = await vm.GuardarProfesorAsync();
                if (resultado)
                {
                    // Recargar la lista de profesores
                    if (_profesoresVM != null)
                    {
                        await _profesoresVM.RecargarDatos();
                    }

                    await DisplayAlert("�xito", "El profesor ha sido guardado correctamente.", "Aceptar");
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
            var confirm = await DisplayAlert("Cancelar", "�Est�s seguro de cancelar? Los cambios no se guardar�n.", "S�", "No");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
