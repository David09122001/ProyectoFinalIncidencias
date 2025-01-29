using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace ProjecteFinal.Views;

[QueryProperty(nameof(Rol), "Rol")]
public partial class ViewModificarRol : ContentPage
{
    private RolesVM _vm;

    public Rol Rol
    {
        get => _vm?.RolSeleccionado;
        set
        {
            if (_vm != null)
            {
                _vm.RolSeleccionado = value;
                OnPropertyChanged();
            }
        }
    }

    public ViewModificarRol()
    {
        InitializeComponent();

        // Inicializar y asignar el ViewModel
        _vm = new RolesVM();
        BindingContext = _vm;

        // Cargar permisos al cargar la vista
        if (_vm.RolSeleccionado != null)
        {
            _ = _vm.CargarPermisosAsync(_vm.RolSeleccionado.id);
        }
    }

    private void OnPermisoClicked(object sender, EventArgs e)
    {
        Console.WriteLine("Evento OnPermisoClicked disparado.");
        if (sender is Button button && button.BindingContext is Permiso permiso)
        {
            Console.WriteLine($"Permiso antes del cambio: {permiso.descripcion}, Asignado: {permiso.IsAssigned}");

            permiso.IsAssigned = !permiso.IsAssigned;

            Console.WriteLine($"Permiso después del cambio: {permiso.descripcion}, Asignado: {permiso.IsAssigned}");

            _vm.PermisosDisponibles = new ObservableCollection<Permiso>(_vm.PermisosDisponibles);
        }
        else
        {
            Console.WriteLine("Error: No se pudo obtener el contexto del botón.");
        }
    }




    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            if (_vm.RolSeleccionado == null)
                throw new ArgumentException("El rol no puede ser nulo.");

            // Guardar los permisos seleccionados
            await _vm.GuardarRolAsync();

            await DisplayAlert("Éxito", "Rol y permisos guardados correctamente.", "Aceptar");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar el rol: {ex.Message}", "Aceptar");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        // Simplemente retrocede a la página anterior
        await Navigation.PopAsync();
    }

}