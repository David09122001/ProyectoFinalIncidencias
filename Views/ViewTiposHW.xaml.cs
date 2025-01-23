using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views
{
    public partial class ViewTiposHW : ContentPage
    {
        private TiposHWVM vm;

        public ViewTiposHW()
        {
            InitializeComponent();
            BindingContext = vm = new TiposHWVM();
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            try
            {
                await vm.GuardarTipoAsync();
                await DisplayAlert("Éxito", "Tipo de hardware guardado correctamente.", "Aceptar");
            }
            catch (InvalidOperationException ex)
            {
                await DisplayAlert("Aviso", ex.Message, "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar el tipo: {ex.Message}", "Aceptar");
            }
        }


        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var tipo = button?.CommandParameter as Incidencia_HW;

            if (tipo != null && await DisplayAlert("Confirmar", $"¿Eliminar el tipo {tipo.dispositivo}?", "Sí", "No"))
            {
                try
                {
                    await vm.EliminarTipoAsync(tipo);
                    await DisplayAlert("Éxito", "Tipo eliminado correctamente.", "Aceptar");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo eliminar el tipo: {ex.Message}", "Aceptar");
                }
            }
        }

        private async void OnModificarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var tipo = button?.CommandParameter as Incidencia_HW;

            if (tipo != null)
            {
                string nuevoNombre = await DisplayPromptAsync("Modificar Tipo",
                    $"Introduce un nuevo nombre para el tipo \"{tipo.dispositivo}\":",
                    initialValue: tipo.dispositivo);

                if (!string.IsNullOrWhiteSpace(nuevoNombre))
                {
                    try
                    {
                        await vm.ModificarTipoAsync(tipo, nuevoNombre);
                        await DisplayAlert("Éxito", "Tipo modificado correctamente.", "Aceptar");
                    }
                    catch (InvalidOperationException ex)
                    {
                        await DisplayAlert("Aviso", ex.Message, "Aceptar");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"No se pudo modificar el tipo: {ex.Message}", "Aceptar");
                    }
                }
            }
        }



    }
}
