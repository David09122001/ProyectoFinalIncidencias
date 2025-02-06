using GestorIncidencias.Models;
using GestorIncidencias.ViewModel;


namespace GestorIncidencias.Views
{
    public partial class ViewDepartamentos : ContentPage
    {
        private DepartamentosVM vm;

        public ViewDepartamentos()
        {
            InitializeComponent();
            BindingContext = vm = new DepartamentosVM();
        }

        private async void OnInsertarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ViewInsertarDepartamento));
        }

        private async void OnModificarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var departamento = button?.CommandParameter as Departamento;

            if (departamento != null)
            {
                await Shell.Current.GoToAsync($"{nameof(ViewModificarDepartamento)}",
                    new Dictionary<string, object>
                    {
                        { "Departamento", departamento }
                    });
            }
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var departamento = button?.CommandParameter as Departamento;

            if (departamento != null)
            {
                bool confirm = await DisplayAlert("Confirmar",
                    $"¿Estás seguro de que deseas eliminar el departamento '{departamento.nombre}'?",
                    "Sí",
                    "No");

                if (confirm)
                {
                    try
                    {
                        await vm.EliminarDepartamentoAsync(departamento);
                        await DisplayAlert("Éxito", "Departamento eliminado correctamente.", "Aceptar");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"No se pudo eliminar el departamento: {ex.Message}", "Aceptar");
                    }
                }
            }
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Recargar la lista al volver a la vista
            await vm.CargarDepartamentosAsync();
        }
    }
}
