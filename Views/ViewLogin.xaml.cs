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
            await Shell.Current.GoToAsync($"{nameof(MainMenu)}",
                new Dictionary<string, object>
                {
                    { "Profesor", usuario }
                });
        }
        else
        {
            ErrorLabel.Text = vm.MensajeError;
            ErrorLabel.IsVisible = vm.HayError;
        }
    }


}
