using ProjecteFinal.Models;
using ProjecteFinal.ViewModel;
using System;
using System.IO;
using Microsoft.Maui.Storage;

namespace ProjecteFinal.Views
{
    [QueryProperty(nameof(Profesor), "Profesor")]
    public partial class ViewInsertarIncidencia : ContentPage
    {
        private InsertarIncidenciaVM vm;

        private Profesor _profesor;
        public Profesor Profesor
        {
            get => _profesor;
            set
            {
                _profesor = value;
                OnPropertyChanged();
            }
        }

        public ViewInsertarIncidencia()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            BindingContext = vm = new InsertarIncidenciaVM(Profesor);
            Loaded -= OnLoaded;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Guardar clicado. Iniciando el guardado...");
            try
            {
                var resultado = await vm.GuardarIncidenciaAsync();
                if (!resultado)
                {
                    Console.WriteLine("Error: Validación fallida.");
                    await DisplayAlert("Error", "Faltan campos obligatorios. Por favor, completa todos los datos.", "Aceptar");
                    return;
                }

                Console.WriteLine("Guardado con éxito.");
                await DisplayAlert("Éxito", "Incidencia guardada correctamente.", "Aceptar");

                // Refrescar lista de incidencias
                if (Shell.Current.Navigation.NavigationStack.OfType<ViewIncidencias>().FirstOrDefault() is ViewIncidencias viewIncidencias &&
                    viewIncidencias.BindingContext is IncidenciasVM incidenciasVM)
                {
                    incidenciasVM.CargarIncidenciasAsync();
                }

                await Navigation.PopAsync();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Validación fallida: {ex.Message}");
                await DisplayAlert("Aviso", ex.Message, "Aceptar");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar: {ex.Message}");
                await DisplayAlert("Error", $"Ha ocurrido un error: {ex.Message}", "Aceptar");
            }
            finally
            {
                Console.WriteLine("Finalizando guardado.");
            }
        }


        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnHacerFotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        var localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                        using (var sourceStream = await photo.OpenReadAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await sourceStream.CopyToAsync(memoryStream);
                            vm.AgregarAdjunto(photo.FileName, localFilePath, memoryStream.ToArray());
                        }

                        await DisplayAlert("Éxito", "Foto capturada y adjuntada correctamente.", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "El dispositivo no soporta la captura de fotos.", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo capturar la foto: {ex.Message}", "Aceptar");
            }
        }

        private async void OnSeleccionarArchivoClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();
                if (result != null)
                {
                    var localFilePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);

                    using (var sourceStream = await result.OpenReadAsync())
                    using (var memoryStream = new MemoryStream())
                    {
                        await sourceStream.CopyToAsync(memoryStream);
                        vm.AgregarAdjunto(result.FileName, localFilePath, memoryStream.ToArray());
                    }

                    await DisplayAlert("Éxito", "Archivo adjuntado correctamente.", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo adjuntar el archivo: {ex.Message}", "Aceptar");
            }
        }

       


    }
}
