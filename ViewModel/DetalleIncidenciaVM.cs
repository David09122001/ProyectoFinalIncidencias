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
        public Incidencia Incidencia { get; set; }
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

        public Profesor Profesor { get;  set; } 

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
                Profesor = await profesorDAO.BuscarPorDniAsync(Incidencia.profesorDni);
                OnPropertyChanged(nameof(Profesor)); 
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
                var fontTitle = new XFont("Arial", 20, XFontStyle.Bold);
                var fontSection = new XFont("Arial", 16, XFontStyle.Bold);
                var fontRegular = new XFont("Arial", 12, XFontStyle.Regular);
                var fontBold = new XFont("Arial", 12, XFontStyle.Bold);
                var corporateBlueBrush = new XSolidBrush(XColor.FromArgb(0, 51, 102)); // Azul corporativo oscuro
                var blackBrush = XBrushes.Black;

                // Márgenes y posicionamiento inicial
                double margin = 40;
                double currentY = margin;

                // Encabezado del documento
                graphics.DrawString("Informe de Incidencia", fontTitle, corporateBlueBrush,
                    new XRect(0, currentY, page.Width, page.Height), XStringFormats.TopCenter);
                currentY += 50;

                // Información general con una tabla
                graphics.DrawString("Información General", fontSection, corporateBlueBrush, new XPoint(margin, currentY));
                currentY += 20;

                var tableStartY = currentY;
                double rowHeight = 20;

                // Dibujar líneas de tabla
                graphics.DrawLine(XPens.Black, margin, tableStartY, page.Width - margin, tableStartY);
                currentY += rowHeight;
                graphics.DrawLine(XPens.Black, margin, currentY, page.Width - margin, currentY);
                currentY += rowHeight;
                graphics.DrawLine(XPens.Black, margin, currentY, page.Width - margin, currentY);
                currentY += rowHeight;

                // Dibujar contenido de la tabla
                double col1X = margin;
                double col2X = margin + 120;

                graphics.DrawString("ID:", fontBold, blackBrush, new XPoint(col1X, tableStartY + rowHeight - 5));
                graphics.DrawString(Incidencia.id.ToString(), fontRegular, blackBrush, new XPoint(col2X, tableStartY + rowHeight - 5));

                graphics.DrawString("Estado:", fontBold, blackBrush, new XPoint(col1X, tableStartY + 2 * rowHeight - 5));
                graphics.DrawString(Incidencia.estado, fontRegular, blackBrush, new XPoint(col2X, tableStartY + 2 * rowHeight - 5));

                graphics.DrawString("Aula:", fontBold, blackBrush, new XPoint(col1X, tableStartY + 3 * rowHeight - 5));
                graphics.DrawString(Incidencia.aulaUbicacion, fontRegular, blackBrush, new XPoint(col2X, tableStartY + 3 * rowHeight - 5));

                currentY = tableStartY + 3 * rowHeight + 20;

                // Fechas y Tiempo
                graphics.DrawString($"Fecha de Incidencia: {Incidencia.fechaIncidencia:dd/MM/yyyy}", fontRegular, blackBrush, new XPoint(margin, currentY));
                currentY += 20;

                if (Incidencia.fechaResolucion != null && Incidencia.fechaResolucion != DateTime.MinValue)
                {
                    graphics.DrawString($"Fecha de Resolución: {Incidencia.fechaResolucion:dd/MM/yyyy}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                }

                graphics.DrawString($"Tiempo Invertido: {Incidencia.tiempoInvertido} minutos", fontRegular, blackBrush, new XPoint(margin, currentY));
                currentY += 30;

                // Profesor Responsable
                graphics.DrawString("Responsable de la Incidencia", fontSection, corporateBlueBrush, new XPoint(margin, currentY));
                currentY += 20;

                graphics.DrawString($"Profesor: {Profesor?.nombre ?? "Ningún profesor asignado"}", fontRegular, blackBrush, new XPoint(margin, currentY));
                currentY += 20;
                if (!string.IsNullOrWhiteSpace(Profesor?.email))
                {
                    graphics.DrawString($"Correo Electrónico: {Profesor.email}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                }

                // Separador
                currentY += 10;
                graphics.DrawLine(XPens.Black, margin, currentY, page.Width - margin, currentY);
                currentY += 20;

                // Descripción Detallada
                graphics.DrawString("Descripción Detallada", fontSection, corporateBlueBrush, new XPoint(margin, currentY));
                currentY += 20;
                graphics.DrawString(Incidencia.descripcionDetallada, fontRegular, blackBrush,
                    new XRect(margin, currentY, page.Width - 2 * margin, page.Height - currentY), XStringFormats.TopLeft);
                currentY += 40;

                // Observaciones
                if (!string.IsNullOrWhiteSpace(Incidencia.observaciones))
                {
                    graphics.DrawString("Observaciones", fontSection, corporateBlueBrush, new XPoint(margin, currentY));
                    currentY += 20;
                    graphics.DrawString(Incidencia.observaciones, fontRegular, blackBrush,
                        new XRect(margin, currentY, page.Width - 2 * margin, page.Height - currentY), XStringFormats.TopLeft);
                    currentY += 40;
                }

                // Especialización: Hardware, Software o Red
                if (IncidenciaHW != null)
                {
                    graphics.DrawString("Detalles de Hardware", fontSection, corporateBlueBrush, new XPoint(margin, currentY));
                    currentY += 20;
                    graphics.DrawString($"Dispositivo: {IncidenciaHW.dispositivo}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                    graphics.DrawString($"Modelo: {IncidenciaHW.modelo}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                    graphics.DrawString($"Número de Serie: {IncidenciaHW.numeroSerie}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 40;
                }
                else if (IncidenciaSW != null)
                {
                    graphics.DrawString("Detalles de Software", fontSection, corporateBlueBrush, new XPoint(margin, currentY));
                    currentY += 20;
                    graphics.DrawString($"Sistema Operativo: {IncidenciaSW.sistemaOperativo}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 20;
                    graphics.DrawString($"Aplicación: {IncidenciaSW.aplicacion}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 40;
                }
                else if (IncidenciaRed != null)
                {
                    graphics.DrawString("Detalles de Red", fontSection, corporateBlueBrush, new XPoint(margin, currentY));
                    currentY += 20;
                    graphics.DrawString($"Dispositivo Afectado: {IncidenciaRed.dispositivoAfectado}", fontRegular, blackBrush, new XPoint(margin, currentY));
                    currentY += 40;
                }

                // Guardar el archivo PDF
                var filePath = @"C:\Users\David\Desktop\Projecte Final\informes\InformeDetalleIncidencia.pdf";
                document.Save(filePath);

                // Mostrar mensaje de éxito
                Application.Current.MainPage.DisplayAlert("Éxito", $"Informe generado: {filePath}", "Aceptar");
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"No se pudo generar el informe: {ex.Message}", "Aceptar");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
