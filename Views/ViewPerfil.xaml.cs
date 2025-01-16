using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views;

[QueryProperty(nameof(Profesor), "Profesor")]
public partial class ViewPerfil : ContentPage
{
    private PerfilVM vm;

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

    public ViewPerfil()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        BindingContext = vm = new PerfilVM(Profesor);
        Loaded -= OnLoaded;
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            if (await vm.GuardarNuevaContraseñaAsync())
            {
                await DisplayAlert("Éxito", "Contraseña actualizada correctamente.", "Aceptar");
            }
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Aviso", ex.Message, "Aceptar");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar la contraseña: {ex.Message}", "Aceptar");
        }
    }
}
