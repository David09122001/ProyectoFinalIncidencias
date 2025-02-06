using GestorIncidencias.Models;
using GestorIncidencias.ViewModel;

namespace GestorIncidencias.Views;

public partial class ViewRoles : ContentPage
{
    private RolesVM vm;

    public ViewRoles()
    {
        InitializeComponent();
        BindingContext = vm = new RolesVM(); 
    }

    private async void OnInsertarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ViewInsertarRol));
    }

    private async void OnModificarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var rol = button?.CommandParameter as Rol; 

        if (rol != null)
        {
            await Shell.Current.GoToAsync($"{nameof(ViewModificarRol)}",
                new Dictionary<string, object>
                {
                { "Rol", rol }  // Pasa el objeto Rol completo
                });
        }
    }


    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var rol = button?.CommandParameter as Rol;

        if (rol != null)
        {
            bool confirm = await DisplayAlert("Confirmar",
                $"¿Estás seguro de que deseas eliminar el rol '{rol.nombre}'?",
                "Sí",
                "No");

            if (confirm)
            {
                try
                {
                    await vm.EliminarRolAsync(rol);  // Usamos el ViewModel para manejar la eliminación
                    await DisplayAlert("Éxito", "Rol eliminado correctamente.", "Aceptar");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo eliminar el rol: {ex.Message}", "Aceptar");
                }
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.CargarRolesAsync(); 
    }
}