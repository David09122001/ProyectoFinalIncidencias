using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;
using System;

namespace ProjecteFinal.Views;

public partial class ViewLogin : ContentPage
{
    private LoginVM vm;
    public ViewLogin()
    {
        InitializeComponent();
        BaseDatos.BaseDatos.InicializarBaseDatosAsync(); 
        BindingContext = vm = new LoginVM();
    }

    private async void IniciarClicked(object sender, EventArgs e)
    {
        var usuario = await vm.IniciarSesionAsync();

        if (usuario != null)
        {
            Preferences.Set("UsuarioEmail", usuario.email);

            // Obtener información del rol
            var rolDao = new RolDAO();
            var permisoDao = new PermisoDAO();
            var rol = await rolDao.ObtenerRolPorIdAsync(usuario.rol_id);

            if (rol != null)
            {
                // Comprobar si el rol tiene permisos específicos
                var permisos = await permisoDao.ObtenerPermisosPorRolAsync(rol.id);

                if (rol.nombre == "Profesor")
                {
                    // Redirigir a ProfesorMenu
                    await Shell.Current.GoToAsync($"{nameof(ProfesorMenu)}",
                        new Dictionary<string, object>
                        {
                        { "Profesor", usuario }
                        });
                }
                else
                {
                    // Redirigir al menú completo
                    await Shell.Current.GoToAsync($"{nameof(MainMenu)}",
                        new Dictionary<string, object>
                        {
                        { "Profesor", usuario }
                        });
                }
            }
            else
            {
                ErrorLabel.Text = "Error al obtener el rol del usuario.";
                ErrorLabel.IsVisible = true;
            }
        }
        else
        {
            ErrorLabel.Text = vm.MensajeError;
            ErrorLabel.IsVisible = vm.HayError;
        }
    }

    private async void OlvidarContrasenaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ViewRestablecerContrasena());
    }



}
