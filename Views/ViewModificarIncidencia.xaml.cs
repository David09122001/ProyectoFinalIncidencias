
using GestorIncidencias.Models;
using GestorIncidencias.ViewModel;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Linq;
using GestorIncidencias.DAO;

namespace GestorIncidencias.Views;

[QueryProperty(nameof(Incidencia), "Incidencia")]
public partial class ViewModificarIncidencia : ContentPage
{
    private ModificarIncidenciaVM vm;

    private Incidencia _incidencia;
    public Incidencia Incidencia
    {
        get => _incidencia;
        set
        {
            _incidencia = value;
            OnPropertyChanged();
        }
    }

    public ViewModificarIncidencia()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        if (Incidencia == null)
            return;


        BindingContext = vm = new ModificarIncidenciaVM(Incidencia);

        if (Incidencia.fechaResolucion == DateTime.MinValue)
        {
            Incidencia.fechaResolucion = DateTime.Now;
        }

        Loaded -= OnLoaded;
    }




    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            bool cambiosGuardados = await vm.GuardarCambiosAsync();

            if (!cambiosGuardados) 
            {
                return;
            }

            await DisplayAlert("Éxito", "La incidencia se ha actualizado correctamente.", "Aceptar");
            await Navigation.PopAsync();

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Aceptar");
        }
    }



    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cancelar", "¿Estás seguro de que quieres cancelar los cambios?", "Sí", "No");
        if (confirm)
             vm._profesorTemporal = null;
            await Navigation.PopAsync();
    }

    private async void OnCambiarProfesorClicked(object sender, EventArgs e)
    {
        if (vm != null)
        {
            await vm.CambiarProfesorAsync();
        }
    }

    private async void OnHacerFotoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.CapturePhotoAsync();
            if (result != null)
            {
                string localFilePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);

                // Guardar en el almacenamiento local
                using var stream = await result.OpenReadAsync();
                using var localStream = File.OpenWrite(localFilePath);
                await stream.CopyToAsync(localStream);

                // Leer en memoria
                using var memoryStream = new MemoryStream();
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(memoryStream);

                vm.AgregarAdjunto(result.FileName, localFilePath, memoryStream.ToArray());
                await DisplayAlert("Éxito", "Foto adjuntada correctamente.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al capturar foto: {ex.Message}", "Aceptar");
        }
    }

    private async void OnResolverPorSAIClicked(object sender, EventArgs e)
    {
        bool confirmacion = await DisplayAlert("Confirmación",
            "¿Estás seguro de que deseas asignar esta incidencia al SAI?", "Sí", "No");

        if (!confirmacion)
        {
            return;
        }

        if (BindingContext is ModificarIncidenciaVM vm)
        {
            await vm.ResolverPorSAIAsync();
            await DisplayAlert("Incidencia Asignada al SAI",
                "La incidencia ha sido asignada correctamente al SAI.", "Aceptar");
        }
    }


    private async void OnSeleccionarArchivoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona un archivo",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                string localFilePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);

                // Guardar en el almacenamiento local
                using var stream = await result.OpenReadAsync();
                using var localStream = File.OpenWrite(localFilePath);
                await stream.CopyToAsync(localStream);

                // Leer en memoria
                using var memoryStream = new MemoryStream();
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(memoryStream);

                vm.AgregarAdjunto(result.FileName, localFilePath, memoryStream.ToArray());
                await DisplayAlert("Éxito", "Archivo adjuntado correctamente.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al seleccionar archivo: {ex.Message}", "Aceptar");
        }
    }
}
