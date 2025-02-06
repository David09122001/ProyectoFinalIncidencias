using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using GestorIncidencias.DAO;
using GestorIncidencias.Models;
using GestorIncidencias.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GestorIncidencias.ViewModel
{
    public class IncidenciasVM : INotifyPropertyChanged
    {
        private readonly IncidenciaDAO incidenciaDAO;
        private readonly ProfesorDAO profesorDAO;

        public ObservableCollection<Incidencia> Incidencias { get;  set; } 
        public ObservableCollection<Incidencia> IncidenciasFiltradas { get; set; }

        // Filtros
        private string _filtroBusqueda;
        public string FiltroBusqueda
        {
            get => _filtroBusqueda;
            set
            {
                _filtroBusqueda = value;
                OnPropertyChanged(FiltroBusqueda);
                AplicarFiltros();
            }
        }

        private string _filtroEstado;
        public string FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                _filtroEstado = value;
                OnPropertyChanged(FiltroEstado);
                AplicarFiltros();
            }
        }

        private string _filtroTipoIncidencia;
        public string FiltroTipoIncidencia
        {
            get => _filtroTipoIncidencia;
            set
            {
                _filtroTipoIncidencia = value;
                OnPropertyChanged(FiltroTipoIncidencia);
                AplicarFiltros();
            }
        }

        private string _filtroResponsable;
        public string FiltroResponsable
        {
            get => _filtroResponsable;
            set
            {
                _filtroResponsable = value;
                OnPropertyChanged(FiltroResponsable);
                AplicarFiltros();
            }
        }

        private string _filtroOrdenacion;
        public string FiltroOrdenacion
        {
            get => _filtroOrdenacion;
            set
            {
                _filtroOrdenacion = value;
                OnPropertyChanged(FiltroOrdenacion);
                AplicarFiltros();
            }
        }


        private Incidencia _selectedIncidencia;
        public Incidencia SelectedIncidencia
        {
            get => _selectedIncidencia;
            set
            {
                _selectedIncidencia = value;
                OnPropertyChanged(nameof(SelectedIncidencia));
            }
        }

        private string _nombreProfesor;
        public string NombreProfesor
        {
            get => _nombreProfesor;
            set
            {
                _nombreProfesor = value;
                OnPropertyChanged(nameof(NombreProfesor));
            }
        }

        private bool _isFiltros;

        public bool isFiltros
        {
            get => _isFiltros;
            set
            {
                _isFiltros = value;
                OnPropertyChanged(nameof(isFiltros));
            }
        }

        private string _tipoBusqueda;
        public string TipoBusqueda
        {
            get => _tipoBusqueda;
            set
            {
                _tipoBusqueda = value;
                OnPropertyChanged(TipoBusqueda);
                AplicarFiltros();
            }
        }

        public ObservableCollection<string> OpcionesBusqueda { get; private set; }
        public ObservableCollection<string> OpcionesResponsable { get; private set; }
        public ObservableCollection<string> Estados { get; private set; }
        public ObservableCollection<string> TiposIncidencia { get; private set; }
        public ObservableCollection<string> OpcionesOrdenacion { get; private set; }

        public Command GenerarInformeCommand { get; }

        public IncidenciasVM()
        {
            incidenciaDAO = new IncidenciaDAO();
            profesorDAO = new ProfesorDAO();
            Incidencias = new ObservableCollection<Incidencia>();
            IncidenciasFiltradas = new ObservableCollection<Incidencia>();

            
            OpcionesBusqueda = new ObservableCollection<string> { " ", "Descripción", "Responsable" };
            OpcionesResponsable = new ObservableCollection<string> { " ", "Sí", "No" };

            Estados  = new ObservableCollection<string> {" ", "Sin asignar", "Asignada", "En proceso", "Pendiente", "Resuelta" };
            TiposIncidencia = new ObservableCollection<string> { " ", "Hardware", "Software", "Red" };
            OpcionesOrdenacion = new ObservableCollection<string> { " ", "Más reciente a menos", "Menos reciente a más" };

            GenerarInformeCommand = new Command(async () => await GenerarInformeAsync());
            isFiltros = false;
        }

        private async Task GenerarInformeAsync()
        {
            if (IncidenciasFiltradas == null || IncidenciasFiltradas.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "No hay incidencias para generar el informe.", "Aceptar");
                return;
            }

            try
            {
                var pdfDoc = new PdfDocument();
                var page = pdfDoc.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                var gfx = XGraphics.FromPdfPage(page);
                var fontRegular = new XFont("Verdana", 8, XFontStyle.Regular);
                var fontBold = new XFont("Verdana", 10, XFontStyle.Bold);

                double margin = 40;
                double y = margin;

                // ** Encabezado **
                gfx.DrawString("Informe de Incidencias", fontBold, XBrushes.Black, new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
                y += 30;
                gfx.DrawString($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", fontRegular, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, page.Height), XStringFormats.TopLeft);
                y += 20;
                gfx.DrawString($"Número total de incidencias: {IncidenciasFiltradas.Count}", fontRegular, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, page.Height), XStringFormats.TopLeft);
                y += 40;

                // Column widths
                double colIdWidth = 40;
                double colDescWidth = 160;
                double colEstadoWidth = 80;
                double colRespWidth = 120;
                double colFechaWidth = 80;

                // ** Tabla - Cabecera **
                gfx.DrawString("ID", fontBold, XBrushes.Black, new XRect(margin, y, colIdWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Descripción", fontBold, XBrushes.Black, new XRect(margin + colIdWidth, y, colDescWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Estado", fontBold, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth, y, colEstadoWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Profesor", fontBold, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth, y, colRespWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Fecha", fontBold, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth + colRespWidth, y, colFechaWidth, 20), XStringFormats.TopLeft);

                y += 20;
                gfx.DrawLine(XPens.Black, margin, y, page.Width - margin, y);
                y += 10;

                // ** Tabla - Datos **
                foreach (var incidencia in IncidenciasFiltradas)
                {
                    gfx.DrawString(incidencia.id.ToString(), fontRegular, XBrushes.Black, new XRect(margin, y, colIdWidth, 20), XStringFormats.TopLeft);

                    string descripcion = incidencia.descripcionDetallada.Length > 25
                        ? incidencia.descripcionDetallada.Substring(0, 25) + "..."
                        : incidencia.descripcionDetallada;
                    gfx.DrawString(descripcion, fontRegular, XBrushes.Black, new XRect(margin + colIdWidth, y, colDescWidth, 20), XStringFormats.TopLeft);

                    gfx.DrawString(incidencia.estado, fontRegular, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth, y, colEstadoWidth, 20), XStringFormats.TopLeft);

                    string responsable = string.IsNullOrWhiteSpace(incidencia.NombreResponsable) ? "Ningún profesor asignado" : incidencia.NombreResponsable;
                    if (responsable.Length > 15)
                    {
                        responsable = responsable.Substring(0, 15) + "...";
                    }
                    gfx.DrawString(responsable, fontRegular, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth, y, colRespWidth, 20), XStringFormats.TopLeft);

                    gfx.DrawString(incidencia.fechaIncidencia.ToString("dd/MM/yyyy"), fontRegular, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth + colRespWidth, y, colFechaWidth, 20), XStringFormats.TopCenter);

                    y += 20;

                    if (y > page.Height - margin)
                    {
                        page = pdfDoc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = margin;
                    }
                }

                // ** Pie de página **
                gfx.DrawLine(XPens.Black, margin, page.Height - 40, page.Width - margin, page.Height - 40);
                gfx.DrawString($"Página {pdfDoc.PageCount}", fontRegular, XBrushes.Black, new XRect(0, page.Height - 30, page.Width, 20), XStringFormats.TopCenter);

                // Generar un nombre único para el archivo
                var folderPath = @"C:\Users\David\Desktop\Projecte Final\informes";
                var baseFileName = "InformeIncidencias";
                var fileExtension = ".pdf";
                var filePath = GetUniqueFileName(folderPath, baseFileName, fileExtension);

                // Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    pdfDoc.Save(stream);
                }

                await Application.Current.MainPage.DisplayAlert("Éxito", $"Informe generado en: {filePath}", "Aceptar");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo generar el informe: {ex.Message}", "Aceptar");
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


        public async Task CargarIncidenciasAsync(Profesor profesor)
        {
            Incidencias.Clear();
            IncidenciasFiltradas.Clear();
            List<Incidencia> listaIncidencias;

            if (profesor.rol_id == 1) //rol profesor
            {
                listaIncidencias = incidenciaDAO.ObtenerIncidenciasPorProfesor(profesor.dni).ToList();
            }
            else
            {
                listaIncidencias = incidenciaDAO.ObtenerIncidencias().ToList();
            }

            foreach (var incidencia in listaIncidencias)
            {
                if (!string.IsNullOrWhiteSpace(incidencia.responsableDni))
                {
                    var profesorResponsable = await profesorDAO.BuscarPorDniAsync(incidencia.responsableDni);
                    incidencia.NombreResponsable = profesorResponsable?.nombre ?? "Ningún profesor asignado";
                }
                else
                {
                    incidencia.NombreResponsable = "Ningún profesor asignado";
                }

                Incidencias.Add(incidencia);
            }

            IncidenciasFiltradas = new ObservableCollection<Incidencia>(Incidencias.Where(i => i.estado != "Resuelta").OrderByDescending(i => i.fechaIncidencia));


            OnPropertyChanged(nameof(IncidenciasFiltradas));
        }

        public bool EliminarIncidencia(Incidencia incidencia)
        {
            try
            {
                incidenciaDAO.EliminarIncidencia(incidencia);
                Incidencias.Remove(incidencia);
                IncidenciasFiltradas.Remove(incidencia);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void AplicarFiltros()
        {
            var filtrado = Incidencias.ToList();

            // Barra de búsqueda
            if (!string.IsNullOrWhiteSpace(FiltroBusqueda))
            {
                switch (TipoBusqueda)
                {
                    case "Descripción":
                        filtrado = filtrado.Where(i => i.descripcionDetallada.Contains(FiltroBusqueda, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "Responsable":
                        filtrado = filtrado.Where(i => !string.IsNullOrWhiteSpace(i.NombreResponsable) && i.NombreResponsable.Contains(FiltroBusqueda, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default: // Sin filtro específico (ambos campos)
                        filtrado = filtrado.Where(i =>
                            i.descripcionDetallada.Contains(FiltroBusqueda, StringComparison.OrdinalIgnoreCase) ||
                            (!string.IsNullOrWhiteSpace(i.NombreResponsable) &&
                             i.NombreResponsable.Contains(FiltroBusqueda, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(FiltroResponsable) && FiltroResponsable != " ")
            {
                if (FiltroResponsable == "Sí")
                    filtrado = filtrado.Where(i => !string.IsNullOrWhiteSpace(i.NombreResponsable) && i.NombreResponsable != "Ningún profesor asignado").ToList();
                else if (FiltroResponsable == "No")
                    filtrado = filtrado.Where(i => i.NombreResponsable == "Ningún profesor asignado").ToList();
            }
            // Filtro de Estado
            if (!string.IsNullOrWhiteSpace(FiltroEstado) && FiltroEstado != " ")
            {
                filtrado = filtrado.Where(i => i.estado == FiltroEstado).ToList();
            }

            // Filtro de Tipo de Incidencia
            if (!string.IsNullOrWhiteSpace(FiltroTipoIncidencia) && FiltroTipoIncidencia != " ")
            {
                List<int> idsFiltrados = new List<int>();

                switch (FiltroTipoIncidencia)
                {
                    case "Hardware":
                        idsFiltrados = incidenciaDAO.ObtenerIdsDeIncidenciasHardware();
                        break;
                    case "Software":
                        idsFiltrados = incidenciaDAO.ObtenerIdsDeIncidenciasSoftware();
                        break;
                    case "Red":
                        idsFiltrados = incidenciaDAO.ObtenerIdsDeIncidenciasRed();
                        break;
                }

                // Filtrar por los IDs obtenidos
                filtrado = filtrado.Where(i => idsFiltrados.Contains(i.id)).ToList();
            }

            // Ordenar resultados
            if (!string.IsNullOrWhiteSpace(FiltroOrdenacion) && FiltroOrdenacion != " ")
            {
                if (FiltroOrdenacion == "Más reciente a menos")
                    filtrado = filtrado.OrderByDescending(i => i.fechaIncidencia).ToList();
                else if (FiltroOrdenacion == "Menos reciente a más")
                    filtrado = filtrado.OrderBy(i => i.fechaIncidencia).ToList();
            }

            // Actualizar la lista filtrada
            IncidenciasFiltradas.Clear();
            foreach (var incidencia in filtrado)
            {
                IncidenciasFiltradas.Add(incidencia);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ReiniciarFiltros()
        {
            // Restablecer valores de los filtros
            FiltroBusqueda = string.Empty;
            TipoBusqueda = "Todos";
            FiltroResponsable = null;
            FiltroEstado = null; 
            FiltroTipoIncidencia = null;
            FiltroOrdenacion = "Más FiltroOrdenacionreciente a menos";

            // Volver al estado inicial excluyendo incidencias con estado "Resuelta" y de más reciente a menos.
            IncidenciasFiltradas = new ObservableCollection<Incidencia>(Incidencias.Where(i => i.estado != "Resuelta").OrderByDescending(i => i.fechaIncidencia));

            OnPropertyChanged(nameof(FiltroBusqueda));
            OnPropertyChanged(nameof(TipoBusqueda));
            OnPropertyChanged(nameof(FiltroResponsable));
            OnPropertyChanged(nameof(FiltroEstado));
            OnPropertyChanged(nameof(FiltroTipoIncidencia));
            OnPropertyChanged(nameof(FiltroOrdenacion));
            OnPropertyChanged(nameof(IncidenciasFiltradas)); 
        }


    }
}