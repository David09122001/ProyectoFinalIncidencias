using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views;

public partial class ViewRoles : ContentPage
{
    private RolesVM vm;

    public ViewRoles()
    {
        InitializeComponent();
        BindingContext = vm = new RolesVM();  // Correcta inicializaci�n del ViewModel
    }

    // Acci�n para insertar un nuevo rol
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
            // Aqu� pasas el objeto completo 'rol' como par�metro
            await Shell.Current.GoToAsync($"{nameof(ViewModificarRol)}",
                new Dictionary<string, object>
                {
                { "Rol", rol }  // Pasa el objeto Rol completo
                });
        }
    }


    // Acci�n para eliminar un rol
    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var rol = button?.CommandParameter as Rol;

        if (rol != null)
        {
            bool confirm = await DisplayAlert("Confirmar",
                $"�Est�s seguro de que deseas eliminar el rol '{rol.nombre}'?",
                "S�",
                "No");

            if (confirm)
            {
                try
                {
                    await vm.EliminarRolAsync(rol);  // Usamos el ViewModel para manejar la eliminaci�n
                    await DisplayAlert("�xito", "Rol eliminado correctamente.", "Aceptar");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo eliminar el rol: {ex.Message}", "Aceptar");
                }
            }
        }
    }

    // Recargar la lista de roles cuando la p�gina aparezca
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.CargarRolesAsync();  // Cargamos los roles desde la base de datos
    }
}