using GestorIncidencias.Models;
using GestorIncidencias.ViewModel;
using System.Collections.ObjectModel;

namespace GestorIncidencias.Views;

public partial class ViewInsertarRol : ContentPage
{
    private RolesVM _vm;

    public ViewInsertarRol()
    {
        InitializeComponent();
        _vm = new RolesVM();
        BindingContext = _vm;
        _vm.CargarPermisosAsyncN(); // Cargar todos los permisos disponibles
    }

    private void OnPermisoClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Permiso permiso)
        {
            permiso.IsAssigned = !permiso.IsAssigned;
            _vm.PermisosDisponibles = new ObservableCollection<Permiso>(_vm.PermisosDisponibles);
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            await _vm.InsertarRolAsync();
            await DisplayAlert("Éxito", "Rol creado correctamente.", "Aceptar");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar el rol: {ex.Message}", "Aceptar");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
