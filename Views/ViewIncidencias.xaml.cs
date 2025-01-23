using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;
using System.ComponentModel;

namespace ProjecteFinal.Views;
[QueryProperty(nameof(Profesor), "Profesor")]
public partial class ViewIncidencias : ContentPage, INotifyPropertyChanged
{
    private Profesor _profesor;

    public Profesor Profesor
    {
        get => _profesor;
        set
        {
            _profesor = value;
            OnPropertyChanged();
        }
    }

    
    private IncidenciasVM vm;

    public ViewIncidencias()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }
    private void OnToggleFiltrosClicked(object sender, EventArgs e)
    {
        vm.isFiltros = !vm.isFiltros;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        BindingContext = vm = new IncidenciasVM();
        Loaded -= OnLoaded;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewInsertarIncidencia)}",
            new Dictionary<string, object>
            {
                { "Profesor", Profesor }
            });
    }

    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        if (BindingContext is IncidenciasVM viewModel)
        {
            viewModel.AplicarFiltros();
        }
    }

    private void OnBorrarFiltrosClicked(object sender, EventArgs e)
    {
        if (BindingContext is IncidenciasVM viewModel)
        {
            viewModel.ReiniciarFiltros();
        }
    }




    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var incidencia = button?.CommandParameter as Incidencia;

        if (incidencia != null)
        {
             await Shell.Current.GoToAsync($"{nameof(ViewModificarIncidencia)}",
                new Dictionary<string, object>
                {
                    { "Incidencia", incidencia }
                });
        }
    }

    private async void OnIncidenciaTapped(object sender, EventArgs e)
    {
        var incidenciaSeleccionada = (sender as Label)?.BindingContext as Incidencia;
        if (incidenciaSeleccionada != null)
        {
             await Navigation.PushAsync(new ViewDetalleIncidencia(incidenciaSeleccionada));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var incidencia = button?.CommandParameter as Incidencia;

        if (incidencia != null && await DisplayAlert("Confirmar", "Eliminar esta incidencia?", "Si", "No"))
        {
            if (vm.EliminarIncidencia(incidencia))
            {
                await DisplayAlert("Éxito", "Incidencia eliminada correctamente.", "Aceptar");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar la incidencia.", "Aceptar");
            }
        }
    }
}