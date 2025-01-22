using ProjecteFinal.Models;
using Microsoft.Maui.Controls;
using System;
using ProjecteFinal.ViewModels;

namespace ProjecteFinal.Views;
public partial class ViewDetalleIncidencia : ContentPage
{
    public Incidencia Incidencia { get; set; }

    private DetalleIncidenciaVM vm;
    public ViewDetalleIncidencia(Incidencia incidencia)
    {
        InitializeComponent();
        Incidencia = incidencia;
        vm = new DetalleIncidenciaVM(incidencia);  // Inicializamos el ViewModel con la incidencia
        BindingContext = vm;

        // Verificar si la fecha de resoluci�n es DateTime.MinValue (0)
        if (Incidencia.fechaResolucion == DateTime.MinValue)
        {
            // Si la fecha es 0 o no v�lida, ocultamos la secci�n
            FechaResolucionFrame.IsVisible = false;
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Accede a los adjuntos de la incidencia recibida
        Console.WriteLine($"Adjuntos: {Incidencia.Adjuntos.Count}");
    }

    // M�todo para regresar a la p�gina anterior
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();  // Volver a la p�gina anterior
    }
}