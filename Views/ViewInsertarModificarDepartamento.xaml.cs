using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;
using System.Collections.ObjectModel;

namespace ProjecteFinal.Views
{
    [QueryProperty(nameof(Departamento), "Departamento")]
    public partial class ViewInsertarModificarDepartamento : ContentPage
    {
        private DepartamentosVM vm;

        private Departamento _departamento;
        public Departamento Departamento
        {
            get => _departamento;
            set
            {
                _departamento = value ?? new Departamento(); 
                OnPropertyChanged(nameof(Departamento)); 
            }
        }

        public ViewInsertarModificarDepartamento()
        {
            InitializeComponent();
            BindingContext = this;

            if (Departamento == null)
            {
                Console.WriteLine("Departamento inicializado como null, creando nuevo objeto.");
                Departamento = new Departamento(); 
            }
            else
            {
                Console.WriteLine($"Departamento cargado: {Departamento.codigo}, {Departamento.nombre}, {Departamento.ubicacion}");
            }
        }


        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            try
            {
                if (Departamento == null)
                    throw new ArgumentException("El departamento no puede ser nulo.");

                vm = new DepartamentosVM();
                await vm.GuardarDepartamentoAsync(Departamento);
                await DisplayAlert("Éxito", "Departamento guardado correctamente.", "Aceptar");
                await Navigation.PopAsync();
            }
            catch (InvalidOperationException ex)
            {
                await DisplayAlert("Aviso", ex.Message, "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar el departamento: {ex.Message}", "Aceptar");
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }


}