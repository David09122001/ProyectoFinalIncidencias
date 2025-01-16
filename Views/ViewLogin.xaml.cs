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
        BaseDatos.BaseDatos.InicializarBaseDatosAsync(); // Inicializa la base de datos
        BindingContext = vm = new LoginVM();
    }

    private async void IniciarClicked(object sender, EventArgs e)
    {
        // Llamar al método del ViewModel para autenticar
        var usuario = await vm.IniciarSesionAsync();

        if (usuario != null)
        {
            // Navegar al menú principal pasando el objeto Profesor
            await Shell.Current.GoToAsync($"{nameof(MainMenu)}",
                new Dictionary<string, object>
                {
                    { "Profesor", usuario }
                });
        }
        else
        {
            // Mostrar mensaje de error desde el ViewModel
            ErrorLabel.Text = vm.MensajeError;
            ErrorLabel.IsVisible = vm.HayError;
        }
    }


}
