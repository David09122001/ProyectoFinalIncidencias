using GestorIncidencias.DAO;
using GestorIncidencias.Models;

namespace GestorIncidencias.Views;
[QueryProperty(nameof(Profesor), "Profesor")]
public partial class MainMenu : ContentPage
{
    public string MensajeBienvenida { get; set; }

    private List<Permiso> _permisosUsuario;

    private Profesor _profesor;
    public Profesor Profesor
    {
        get => _profesor;
        set
        {
            _profesor = value;
            MensajeBienvenida = $"¡Bienvenido/a {_profesor.nombre} ({ObtenerNombreRol(_profesor.rol_id)})!";
            OnPropertyChanged(nameof(MensajeBienvenida));

        }
    }

    public MainMenu()
    {
        InitializeComponent();
        BindingContext = this;
        _permisosUsuario = new List<Permiso>();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (Profesor != null)
        {
            Console.WriteLine($"Profesor autenticado: {_profesor.nombre}");

            // Cargar los permisos del usuario
            var permisoDao = new PermisoDAO();
            _permisosUsuario = await permisoDao.ObtenerPermisosPorRolAsync(Profesor.rol_id);
        }

        Loaded -= OnLoaded;
    }

    private bool TienePermiso(string descripcionPermiso)
    {
        return _permisosUsuario.Any(p => p.descripcion.Equals(descripcionPermiso, StringComparison.OrdinalIgnoreCase));
    }

    private async void MostrarAlertaSinPermisos()
    {
        await DisplayAlert("Permisos insuficientes", "No tienes los permisos necesarios para acceder a esta funcionalidad.", "Aceptar");
    }

    private async void IncidenciasTapped(object sender, EventArgs e)
    {
        if (TienePermiso("Añadir Incidencias"))
        {
            await Shell.Current.GoToAsync($"{nameof(ViewIncidencias)}",
                new Dictionary<string, object>
                {
                    { "Profesor", Profesor }
                });
        }
        else
        {
            MostrarAlertaSinPermisos();
        }
    }

    private async void LogsTapped(object sender, EventArgs e)
    {
        if (TienePermiso("Ver logs"))
        {
            await Shell.Current.GoToAsync($"{nameof(ViewLogs)}");
        }
        else
        {
            MostrarAlertaSinPermisos();
        }
    }

    private async void ProfesoresTapped(object sender, EventArgs e)
    {
        if (TienePermiso("Gestionar profesores"))
        {
            await Shell.Current.GoToAsync($"{nameof(ViewProfesores)}");
        }
        else
        {
            MostrarAlertaSinPermisos();
        }
    }

    private async void DepartamentosTapped(object sender, EventArgs e)
    {
        if (TienePermiso("Gestionar departamentos"))
        {
            await Shell.Current.GoToAsync($"{nameof(ViewDepartamentos)}");
        }
        else
        {
            MostrarAlertaSinPermisos();
        }
    }

    private async void TiposHWTapped(object sender, EventArgs e)
    {
        if (TienePermiso("Gestionar hardware"))
        {
            await Shell.Current.GoToAsync($"{nameof(ViewTiposHW)}");
        }
        else
        {
            MostrarAlertaSinPermisos();
        }
    }

    private async void RolesTapped(object sender, EventArgs e)
    {
        if (TienePermiso("Gestionar permisos y roles"))
        {
            await Shell.Current.GoToAsync($"{nameof(ViewRoles)}");
        }
        else
        {
            MostrarAlertaSinPermisos();
        }
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
        DropdownMenu.IsVisible = false; // Ocultar el menú
        bool confirm = await DisplayAlert("Cerrar sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");
        if (confirm)
        {
            await Shell.Current.GoToAsync("ViewLogin");
        }
    }


}