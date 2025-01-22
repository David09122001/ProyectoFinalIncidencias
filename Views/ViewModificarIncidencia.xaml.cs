
using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Linq;

namespace ProjecteFinal.Views;

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

    private void OnLoaded(object sender, EventArgs e)
    {
        if (Incidencia != null)
        {
            BindingContext = vm = new ModificarIncidenciaVM(Incidencia);

            // Establecer la fecha actual solo si la fecha de resolución no tiene un valor
            if (Incidencia.fechaResolucion == DateTime.MinValue)
            {
                Incidencia.fechaResolucion = DateTime.Now; // Establecer la fecha actual por defecto
            }

            Loaded -= OnLoaded;
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            // Verificar el estado de la incidencia
            if (Incidencia.estado == "Solucionada")
            {
                // Si el estado es "Resuelta", aseguramos que la fecha de resolución esté asignada.
                if (Incidencia.fechaResolucion == DateTime.MinValue)
                {
                    // Asignar la fecha actual si no se ha asignado ninguna fecha aún
                    Incidencia.fechaResolucion = DateTime.Now;
                }
            }
            else
            {
                // Si el estado no es "Resuelta", asignamos null a la fecha de resolución
                Incidencia.fechaResolucion = null;
            }

            // Guardar los cambios de la incidencia, asegurándose de que la fecha esté bien asignada
            await vm.GuardarCambiosAsync();

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
        if (BindingContext is ModificarIncidenciaVM vm)
        {
            await vm.ResolverPorSAIAsync();
            await DisplayAlert("Incidencia Asignada al SAI", "La incidencia ha sido asignada correctamente al SAI.", "Aceptar");
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
