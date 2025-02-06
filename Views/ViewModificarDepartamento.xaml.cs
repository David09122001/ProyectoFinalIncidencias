using GestorIncidencias.Models;
using GestorIncidencias.ViewModel;
using System.Collections.ObjectModel;

namespace GestorIncidencias.Views
{
    [QueryProperty(nameof(Departamento), "Departamento")]
    public partial class ViewModificarDepartamento : ContentPage
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
                OnPropertyChanged(nameof(IsCodigoEditable));
            }
        }

        public bool IsCodigoEditable => string.IsNullOrWhiteSpace(_departamento?.codigo);

        public ViewModificarDepartamento()
        {
            InitializeComponent();
            vm = new DepartamentosVM();
            BindingContext = this;

            if (_departamento == null)
                Departamento = new Departamento();
        }


        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            try
            {
                if (_departamento == null)
                    throw new ArgumentException("El departamento no puede ser nulo.");

                await vm.GuardarDepartamentoAsync(_departamento);

                await DisplayAlert("Éxito", "Departamento modificado correctamente.", "Aceptar");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo modificar el departamento: {ex.Message}", "Aceptar");
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }


}