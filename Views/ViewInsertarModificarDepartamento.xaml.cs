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
                OnPropertyChanged(nameof(IsCodigoEditable));
            }
        }

        public bool IsCodigoEditable => string.IsNullOrWhiteSpace(_departamento?.codigo);

        public ViewInsertarModificarDepartamento()
        {
            InitializeComponent();
            BindingContext = this;

            if (_departamento == null)
                Departamento = new Departamento();
        }


        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            try
            {
                if (Departamento == null)
                    throw new ArgumentException("El departamento no puede ser nulo.");

                var vm = new DepartamentosVM();

                // Verificar si el código ya está en uso antes de guardar
                var existente = await vm.departamentoDAO.ObtenerDepartamentoPorCodigoAsync(Departamento.codigo);
                if (existente != null)
                {
                    throw new InvalidOperationException("Ya existe un departamento con este código.");
                }

                await vm.GuardarDepartamentoAsync(Departamento);
                await DisplayAlert("Éxito", "Departamento guardado correctamente.", "Aceptar");
                await Navigation.PopAsync();
            }
            catch (InvalidOperationException ex)
            {
                await DisplayAlert("Aviso", ex.Message, "Aceptar");
            }
            catch (ArgumentException ex)
            {
                await DisplayAlert("Error", ex.Message, "Aceptar");
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