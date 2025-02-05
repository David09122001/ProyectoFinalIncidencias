using ProjecteFinal.Models;

namespace ProjecteFinal.Views;

[QueryProperty(nameof(Profesor), "Profesor")]
public partial class ProfesorMenu : ContentPage
{
    public string MensajeBienvenida { get; set; }
    private Profesor _profesor;
    public Profesor Profesor
    {
        get => _profesor;
        set
        {
            _profesor = value;
            MensajeBienvenida = $"�Bienvenido/a {_profesor.nombre} ({ObtenerNombreRol(_profesor.rol_id)})!";
            OnPropertyChanged(nameof(MensajeBienvenida));

        }
    }

    public ProfesorMenu()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private string ObtenerNombreRol(int rolId)
    {
        return rolId switch
        {
            1 => "Profesor",
            2 => "Mantenimiento TIC",
            3 => "Administrador",
            4 => "Directivo"
        };
    }


    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            frame.BackgroundColor = Color.FromArgb("#E8F0FE"); // Fondo al pasar el rat�n
            frame.HasShadow = true;
        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            frame.BackgroundColor = Colors.White;
            frame.HasShadow = true;
        }
    }

    private async void IncidenciasTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewIncidencias)}",
            new Dictionary<string, object>
            {
                { "Profesor", Profesor }
            });
    }

    private async void LogsTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewLogs)}");
    }

    private void OnOpcionesClicked(object sender, EventArgs e)
    {
        DropdownMenu.IsVisible = !DropdownMenu.IsVisible;
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        DropdownMenu.IsVisible = false; 
        await Shell.Current.GoToAsync($"{nameof(ViewPerfil)}",
          new Dictionary<string, object>
          {
            { "Profesor", Profesor }
          });
    }
    private async void OnCerrarSesionTapped(object sender, EventArgs e)
    {
        DropdownMenu.IsVisible = false;
        bool confirm = await DisplayAlert("Cerrar sesi�n", "�Est�s seguro de que deseas cerrar sesi�n?", "S�", "No");
        if (confirm)
        {
            await Shell.Current.GoToAsync("ViewLogin");
        }
    }
}