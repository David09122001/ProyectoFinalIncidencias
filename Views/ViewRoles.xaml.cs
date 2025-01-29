using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views;

public partial class ViewRoles : ContentPage
{
    private RolesVM vm;

    public ViewRoles()
    {
        InitializeComponent();
        BindingContext = vm = new RolesVM();  // Correcta inicialización del ViewModel
    }

    // Acción para insertar un nuevo rol
    private async void OnInsertarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ViewInsertarRol));
    }

    private async void OnModificarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var rol = button?.CommandParameter as Rol;  // Obtienes el objeto Rol

        if (rol != null)
        {
            // Aquí pasas el objeto completo 'rol' como parámetro
            await Shell.Current.GoToAsync($"{nameof(ViewModificarRol)}",
                new Dictionary<string, object>
                {
                { "Rol", rol }  // Pasa el objeto Rol completo
                });
        }
    }


    // Acción para eliminar un rol
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

    // Recargar la lista de roles cuando la página aparezca
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.CargarRolesAsync();  // Cargamos los roles desde la base de datos
    }
}