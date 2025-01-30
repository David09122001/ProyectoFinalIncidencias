using ProjecteFinal.ViewModels;

namespace ProjecteFinal.Views
{
    public partial class ViewRestablecerContrasena : ContentPage
    {
        private readonly RestablecerContrasenaVM vm;

        public ViewRestablecerContrasena()
        {
            InitializeComponent();
            vm = new RestablecerContrasenaVM();
            BindingContext = vm;
        }

        private async void EnviarCodigoClicked(object sender, EventArgs e)
        {
            bool exito = await vm.EnviarCodigoAsync(CorreoEntry.Text);
            if (exito)
            {
                await DisplayAlert("C�digo enviado", "Se ha enviado un c�digo de verificaci�n a tu correo.", "Aceptar");
                await Navigation.PushAsync(new ViewVerificarCodigo(vm.ProfesorEncontrado, vm.CodigoGenerado));
            }
            else
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = vm.MensajeError;
            }
        }
    }
}
