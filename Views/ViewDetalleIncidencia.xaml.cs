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
        vm = new DetalleIncidenciaVM(incidencia);  
        BindingContext = vm;

        if (Incidencia.fechaResolucion == DateTime.MinValue)
        {
            // Si la fecha es 0 o no v�lida, ocultamos la secci�n
            //FechaResolucionFrame.IsVisible = false;
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Console.WriteLine($"Adjuntos: {Incidencia.Adjuntos.Count}");
    }

    
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();  
    }
}