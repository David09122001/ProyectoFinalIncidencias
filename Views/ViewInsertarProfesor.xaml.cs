using GestorIncidencias.Models;
using GestorIncidencias.ViewModel;
using GestorIncidencias.ViewModels;
using System;

namespace GestorIncidencias.Views
{
    public partial class ViewInsertarProfesor : ContentPage
    {
        private readonly InsertarProfesorVM vm;
        private readonly ProfesoresVM _profesoresVM;

        public ViewInsertarProfesor(ProfesoresVM profesoresVM)
        {
            InitializeComponent();
            _profesoresVM = profesoresVM;
            vm = new InsertarProfesorVM();
            BindingContext = vm;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            bool resultado = await vm.GuardarProfesorAsync();
            if (resultado)
            {
                // Recargar la lista de profesores
                if (_profesoresVM != null)
                {
                    await _profesoresVM.RecargarDatos();
                }

                await DisplayAlert("Éxito", "El profesor ha sido guardado correctamente.", "Aceptar");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar el profesor. Verifica los datos ingresados.", "Aceptar");
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
