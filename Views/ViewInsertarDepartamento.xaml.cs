using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views;

[QueryProperty(nameof(Departamento), "Departamento")]
public partial class ViewInsertarDepartamento : ContentPage
{
    private InsertarDepartamentoVM vm;

    public ViewInsertarDepartamento()
    {
        InitializeComponent();
        BindingContext = vm = new InsertarDepartamentoVM();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        bool guardado = await vm.GuardarDepartamentoAsync();
        if (guardado)
        {
            await DisplayAlert("Éxito", "Departamento insertado correctamente.", "Aceptar");
            await Navigation.PopAsync();
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
