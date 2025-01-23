using ProjecteFinal.Models;

namespace ProjecteFinal.Views;
[QueryProperty(nameof(Profesor), "Profesor")]
public partial class MainMenu : ContentPage
{
    private Profesor _profesor;
    public Profesor Profesor
    {
        get { return _profesor; }
        set
        {
            _profesor = value;
            OnPropertyChanged();
        }
    }

    public MainMenu()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (Profesor != null)
        {
            Console.WriteLine($"Profesor autenticado: {_profesor.nombre}");
        }
        Loaded -= OnLoaded;
    }


    private async void IncidenciasTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewIncidencias)}",
            new Dictionary<string, object>
            {
            { "Profesor", Profesor }
            });
    }

    private async void ProfesoresTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewProfesores)}");
    }
    private async void DepartamentosTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewDepartamentos)}");
    }
    private async void TiposHWTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewTiposHW)}");
    }
    private async void InformesTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewInformes)}");
    }
    private async void LogsTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewLogs)}");
    }

    private async void PerfilTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewPerfil)}",
           new Dictionary<string, object>
           {
            { "Profesor", Profesor }
           });
    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Frame frame)
        {
            frame.BackgroundColor = Color.FromArgb("#E8F0FE"); // Fondo al pasar el ratón
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


}