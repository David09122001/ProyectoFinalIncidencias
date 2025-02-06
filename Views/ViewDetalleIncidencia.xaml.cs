using GestorIncidencias.Models;
using Microsoft.Maui.Controls;
using System;
using GestorIncidencias.ViewModels;

namespace GestorIncidencias.Views;
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