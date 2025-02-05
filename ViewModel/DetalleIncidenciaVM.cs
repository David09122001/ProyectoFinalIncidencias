using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjecteFinal.ViewModels
{
    public class DetalleIncidenciaVM : INotifyPropertyChanged
    {
        private Incidencia _incidencia;
        public Incidencia Incidencia
        {
            get => _incidencia;
            set
            {
                _incidencia = value;
                OnPropertyChanged(nameof(Incidencia));
            }
        }

        private Profesor _profesor;
        public Profesor Profesor
        {
            get => _profesor;
            set
            {
                _profesor = value;
                OnPropertyChanged(nameof(Profesor));
            }
        }

        private Profesor _profesorResponsable;
        public Profesor ProfesorResponsable
        {
            get => _profesorResponsable;
            set
            {
                _profesorResponsable = value;
                OnPropertyChanged(nameof(ProfesorResponsable));  
            }
        }


        public ObservableCollection<Adjunto> Adjuntos { get; set; } = new ObservableCollection<Adjunto>();
        public Incidencia_HW IncidenciaHW { get;  set; }
        public Incidencia_SW IncidenciaSW { get;  set; }
        public Incidencia_Red IncidenciaRed { get;  set; }

        public AdjuntoDAO AdjuntoDAO { get; private set; }

        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Hardware", "Software", "Red" };

        private string _tipoSeleccionado;
        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                if (_tipoSeleccionado != value)
                {
                    _ = ConfirmarCambioTipoAsync(value);
                }
            }
        }

        public bool MostrarHW => IncidenciaHW != null;
        public bool MostrarSW => IncidenciaSW != null;
        public bool MostrarRed => IncidenciaRed != null;


        private readonly IncidenciaHWDAO incidenciaHWDAO = new IncidenciaHWDAO();
        private readonly IncidenciaSWDAO incidenciaSWDAO = new IncidenciaSWDAO();
        private readonly IncidenciaRedDAO incidenciaRedDAO = new IncidenciaRedDAO();


        private ProfesorDAO profesorDAO = new ProfesorDAO();
        public ICommand GenerarInformeCommand { get; }

        public DetalleIncidenciaVM(Incidencia incidencia)
        {
            Incidencia = incidencia;
            AdjuntoDAO = new AdjuntoDAO();  
            TipoSeleccionado = Incidencia.estado;
            GenerarInformeCommand = new Command(GenerarInforme);
            CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            await CargarDatosSubtipoAsync();
            await CargarAdjuntosAsync();
            await CargarProfesorAsync();
        }
        private async Task ConfirmarCambioTipoAsync(string nuevoTipo)
        {
            _tipoSeleccionado = nuevoTipo;
            await CargarDatosSubtipoAsync();
        }

        private async Task CargarProfesorAsync()
        {
            if (!string.IsNullOrEmpty(Incidencia.profesorDni))
            {
                // Cargar el profesor creador de la incidencia
                Profesor = await profesorDAO.BuscarPorDniAsync(Incidencia.profesorDni);
                OnPropertyChanged(nameof(Profesor));

                // Cargar el profesor responsable de la incidencia
                if (!string.IsNullOrEmpty(Incidencia.responsableDni))
                {
                    ProfesorResponsable = await profesorDAO.BuscarPorDniAsync(Incidencia.responsableDni);
                    OnPropertyChanged(nameof(ProfesorResponsable));
                }
            }
        }


        private async Task CargarAdjuntosAsync()
        {
            var adjuntos = await AdjuntoDAO.ObtenerAdjuntosPorIncidenciaAsync(Incidencia.id);
            Adjuntos.Clear();
            foreach (var adjunto in adjuntos)
            {
                Adjuntos.Add(adjunto);
            }
        }


        private async Task CargarDatosSubtipoAsync()
        {
            IncidenciaHW = await incidenciaHWDAO.ObtenerIncidenciaHWPorIdAsync(Incidencia.id);
            if (IncidenciaHW != null)
            {
                TipoSeleccionado = "Hardware";
            }
            else
            {
                IncidenciaSW = await incidenciaSWDAO.ObtenerIncidenciaSWPorId(Incidencia.id);
                if (IncidenciaSW != null)
                {
                    TipoSeleccionado = "Software";
                }
                else
                {
                    IncidenciaRed = await incidenciaRedDAO.ObtenerIncidenciaRedPorId(Incidencia.id);
                    if (IncidenciaRed != null)
                    {
                        TipoSeleccionado = "Red";
                    }
                }
            }

            OnPropertyChanged(nameof(IncidenciaHW));
            OnPropertyChanged(nameof(IncidenciaSW));
            OnPropertyChanged(nameof(IncidenciaRed));
            OnPropertyChanged(nameof(MostrarHW));
            OnPropertyChanged(nameof(MostrarSW));
            OnPropertyChanged(nameof(MostrarRed));
        }
        private void GenerarInforme()
        {
            try
            {
                // Crear un nuevo documento PDF
                var document = new PdfDocument();
                var page = document.AddPage();
                var graphics = XGraphics.FromPdfPage(page);

                // Fuentes y estilos
                var fontTitle = new XFont("Arial", 22, XFontStyle.Bold);
                var fontSubtitle = new XFont("Arial", 16, XFontStyle.Bold);
                var fontRegular = new XFont("Arial", 12, XFontStyle.Regular);
                var fontBold = new XFont("Arial", 12, XFontStyle.Bold);
                var blueBrush = new XSolidBrush(XColor.FromArgb(0, 51, 102)); // Azul corporativo
                var blackBrush = XBrushes.Black;

                // Márgenes y posicionamiento inicial
                double margin = 50;
                double currentY = margin;

                // Encabezado del documento
                graphics.DrawRectangle(XBrushes.LightGray, 0, 0, page.Width, 60); // Fondo gris claro del encabezado
                graphics.DrawString("Informe Detallado de Incidencia", fontTitle, blackBrush,
                    new XRect(0, 0, page.Width, 60), XStringFormats.Center);

                currentY += 80;

                // ** Bloque: Descripción Detallada **
                graphics.DrawString("Descripción Detallada", fontSubtitle, blueBrush, new XPoint(margin, currentY));
                currentY += 20;

                // ** Estado y Aula **
                graphics.DrawString("Estado:", fontBold, blackBrush, new XPoint(margin, currentY));
                graphics.DrawString(Incidencia.estado, fontRegular, blackBrush, new XPoint(margin + 100, currentY));
                currentY += 20;

                graphics.DrawString("Aula:", fontBold, blackBrush, new XPoint(margin, currentY));
                graphics.DrawString(Incidencia.aulaUbicacion, fontRegular, blackBrush, new XPoint(margin + 100, currentY));
                currentY += 20;

                // ** Descripción Detallada **
                graphics.DrawString("Descripción:", fontBold, blackBrush, new XPoint(margin, currentY));
                currentY += 20;

                graphics.DrawString(Incidencia.descripcionDetallada, fontRegular, blackBrush,
                    new XRect(margin, currentY, page.Width - 2 * margin, page.Height - currentY), XStringFormats.TopLeft);
                currentY += 60;

                // ** Bloque: Fechas **
                graphics.DrawString("Fechas", fontSubtitle, blueBrush, new XPoint(margin, currentY));
                currentY += 20;

                graphics.DrawString($"Fecha de Incidencia: {Incidencia.fechaIncidencia:dd/MM/yyyy}", fontRegular, blackBrush, new XPoint(margin, currentY));
                currentY += 20;

                if (Incidencia.fechaResolucion != null && Incidencia.fechaResolucion != DateTime.MinValue)
                {
                    graphics.DrawString($"Fecha de Resolución: {Incidencia.fechaResolucion:dd/MM/yyyy}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                }

                graphics.DrawString($"Tiempo Invertido: {Incidencia.tiempoInvertido} minutos", fontRegular, blackBrush, new XPoint(margin, currentY));
                currentY += 40;

                // ** Bloque: Profesor que inserto de la Incidencia **
                graphics.DrawString("Profesor que registró la incidencia", fontSubtitle, blueBrush, new XPoint(margin, currentY));
                currentY += 20;

                graphics.DrawString($"Profesor: {Profesor.nombre}", fontRegular, blackBrush, new XPoint(margin, currentY));
                currentY += 20;

                if (!string.IsNullOrWhiteSpace(Profesor?.email))
                {
                    graphics.DrawString($"Correo Electrónico: {Profesor.email}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                }

                // Separador visual
                currentY += 20;
                graphics.DrawLine(XPens.LightGray, margin, currentY, page.Width - margin, currentY);
                currentY += 20;

                // ** Bloque: Responsable de la Incidencia **
                graphics.DrawString("Responsable de la Incidencia", fontSubtitle, blueBrush, new XPoint(margin, currentY));
                currentY += 20;

                graphics.DrawString($"Profesor: {ProfesorResponsable?.nombre ?? "Ningún profesor asignado"}", fontRegular, blackBrush, new XPoint(margin, currentY));
                currentY += 20;

                if (!string.IsNullOrWhiteSpace(ProfesorResponsable?.email))
                {
                    graphics.DrawString($"Correo Electrónico: {ProfesorResponsable.email}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                }

                // Separador visual
                currentY += 20;
                graphics.DrawLine(XPens.LightGray, margin, currentY, page.Width - margin, currentY);
                currentY += 20;

                // Generar un nombre único para el archivo
                var folderPath = @"C:\Users\David\Desktop\Projecte Final\informes";
                var baseFileName = "InformeDetalleIncidencia";
                var fileExtension = ".pdf";
                var filePath = GetUniqueFileName(folderPath, baseFileName, fileExtension);

                // Guardar el archivo PDF
                document.Save(filePath);

                // Mostrar mensaje de éxito
                Application.Current.MainPage.DisplayAlert("Éxito", $"Informe generado: {filePath}", "Aceptar");
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"No se pudo generar el informe: {ex.Message}", "Aceptar");
            }
        }

        private string GetUniqueFileName(string folderPath, string baseFileName, string fileExtension)
        {
            int counter = 0;
            string filePath;
            do
            {
                var suffix = counter == 0 ? "" : $"({counter})";
                filePath = Path.Combine(folderPath, $"{baseFileName}{suffix}{fileExtension}");
                counter++;
            } while (File.Exists(filePath));

            return filePath;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
