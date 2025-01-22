using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views;
[QueryProperty(nameof(Profesor), "Profesor")]
public partial class ViewIncidencias : ContentPage
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

    private void OnLoaded(object sender, EventArgs e)
    {
        BindingContext = vm = new IncidenciasVM();
        Loaded -= OnLoaded;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        // Navegar a InsertarIncidencia pasando el Profesor
        await Shell.Current.GoToAsync($"{nameof(ViewInsertarIncidencia)}",
            new Dictionary<string, object>
            {
                { "Profesor", Profesor }
            });
    }
    
    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var incidencia = button?.CommandParameter as Incidencia;

        if (incidencia != null)
        {
            // Navegar a la vista ModificarIncidencia con la incidencia seleccionada
            await Shell.Current.GoToAsync($"{nameof(ViewModificarIncidencia)}",
                new Dictionary<string, object>
                {
                    { "Incidencia", incidencia }
                });
        }
    }

    // Método para manejar el evento de tocar la incidencia
    private async void OnIncidenciaTapped(object sender, EventArgs e)
    {
        // Obtener la incidencia asociada al elemento tocado
        var incidenciaSeleccionada = (sender as Label)?.BindingContext as Incidencia;
        if (incidenciaSeleccionada != null)
        {
            // Navegar a la página de detalles de la incidencia, pasando el objeto Incidencia
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