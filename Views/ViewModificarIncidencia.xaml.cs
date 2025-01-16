using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;
using System;
using Microsoft.Maui.Storage;

namespace ProjecteFinal.Views;

[QueryProperty(nameof(Incidencia), "Incidencia")]
public partial class ViewModificarIncidencia : ContentPage
{
    private ModificarIncidenciaVM vm;

    private Incidencia _incidencia;
    public Incidencia Incidencia
    {
        get { return _incidencia; }
        set { _incidencia = value; OnPropertyChanged(); }
    }

    public ViewModificarIncidencia()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        if (Incidencia != null)
        {
            BindingContext = vm = new ModificarIncidenciaVM(Incidencia);
            Loaded -= OnLoaded;
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (vm.GuardarIncidencia())
        {
            await DisplayAlert("Éxito", "Incidencia modificada correctamente.", "Aceptar");

            if (Shell.Current.Navigation.NavigationStack.OfType<ViewIncidencias>().FirstOrDefault() is ViewIncidencias viewIncidencias &&
                viewIncidencias.BindingContext is IncidenciasVM incidenciasVM)
            {
                incidenciasVM.ActualizarIncidencia(vm.Incidencia);
            }

            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Completa los campos obligatorios.", "Aceptar");
        }
    }



    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
