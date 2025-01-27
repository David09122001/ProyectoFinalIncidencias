using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using ProjecteFinal.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ProjecteFinal.ViewModel
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

        private string _profesorNombre;
        public string ProfesorNombre
        {
            get => _profesorNombre;
            set
            {
                _profesorNombre = value;
                OnPropertyChanged(nameof(ProfesorNombre));
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

            Estados = new ObservableCollection<string> { " ", "Pendiente", "Comunicada", "Resolviendo", "Solucionada", "Inviable" };
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
                var fontRegular = new XFont("Verdana", 8, XFontStyle.Regular); // Reducir tamaño de fuente
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

                // Column widths (ajustados)
                double colIdWidth = 40;
                double colDescWidth = 160; // Reducido
                double colEstadoWidth = 80;
                double colRespWidth = 120; // Reducido
                double colFechaWidth = 80; // Más espacio para la fecha

                // ** Tabla - Cabecera **
                gfx.DrawString("ID", fontBold, XBrushes.Black, new XRect(margin, y, colIdWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Descripción", fontBold, XBrushes.Black, new XRect(margin + colIdWidth, y, colDescWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Estado", fontBold, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth, y, colEstadoWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Responsable", fontBold, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth, y, colRespWidth, 20), XStringFormats.TopLeft);
                gfx.DrawString("Fecha", fontBold, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth + colRespWidth, y, colFechaWidth, 20), XStringFormats.TopLeft);

                y += 20;
                gfx.DrawLine(XPens.Black, margin, y, page.Width - margin, y); // Línea bajo el encabezado
                y += 10;

                // ** Tabla - Datos **
                foreach (var incidencia in IncidenciasFiltradas)
                {
                    gfx.DrawString(incidencia.id.ToString(), fontRegular, XBrushes.Black, new XRect(margin, y, colIdWidth, 20), XStringFormats.TopLeft);

                    string descripcion = incidencia.descripcionDetallada.Length > 25 // Ajustar truncamiento
                        ? incidencia.descripcionDetallada.Substring(0, 25) + "..."
                        : incidencia.descripcionDetallada;
                    gfx.DrawString(descripcion, fontRegular, XBrushes.Black, new XRect(margin + colIdWidth, y, colDescWidth, 20), XStringFormats.TopLeft);

                    gfx.DrawString(incidencia.estado, fontRegular, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth, y, colEstadoWidth, 20), XStringFormats.TopLeft);

                    string responsable = string.IsNullOrWhiteSpace(incidencia.responsableDni) ? "Ningún profesor asignado" : incidencia.responsableDni;
                    if (responsable.Length > 15) // Ajustar truncamiento
                    {
                        responsable = responsable.Substring(0, 15) + "...";
                    }
                    gfx.DrawString(responsable, fontRegular, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth, y, colRespWidth, 20), XStringFormats.TopLeft);

                    // Alinear la fecha al centro
                    gfx.DrawString(incidencia.fechaIncidencia.ToString("dd/MM/yyyy"), fontRegular, XBrushes.Black, new XRect(margin + colIdWidth + colDescWidth + colEstadoWidth + colRespWidth, y, colFechaWidth, 20), XStringFormats.TopCenter);

                    y += 20;

                    // Salto de página si excede el espacio
                    if (y > page.Height - margin)
                    {
                        page = pdfDoc.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = margin;
                    }
                }

                // ** Pie de página **
                gfx.DrawLine(XPens.Black, margin, page.Height - 40, page.Width - margin, page.Height - 40); // Línea
                gfx.DrawString($"Página {pdfDoc.PageCount}", fontRegular, XBrushes.Black, new XRect(0, page.Height - 30, page.Width, 20), XStringFormats.TopCenter);

                // Guardar el archivo
                var filePath = @"C:\Users\David\Desktop\Projecte Final\informes\InformeIncidencias.pdf";
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


        public async Task CargarIncidenciasAsync(Profesor profesor)
        {
            Incidencias.Clear();
            List<Incidencia> listaIncidencias;

            if (profesor.rol_id == 1) // Si el rol es "Profesor"
            {
                // Obtener solo las incidencias del profesor
                listaIncidencias = incidenciaDAO.ObtenerIncidenciasPorProfesor(profesor.dni).ToList();
            }
            else
            {
                // Obtener todas las incidencias
                listaIncidencias = incidenciaDAO.ObtenerIncidencias().ToList();
            }

            foreach (var incidencia in listaIncidencias)
            {
                // Obtener el nombre del profesor responsable
                if (!string.IsNullOrWhiteSpace(incidencia.responsableDni))
                {
                    var responsable = await profesorDAO.BuscarPorDniAsync(incidencia.responsableDni);
                    incidencia.responsableDni = responsable?.nombre ?? "Ningún profesor asignado";
                }
                else
                {
                    incidencia.responsableDni = "Ningún profesor asignado";
                }

                Incidencias.Add(incidencia);
                IncidenciasFiltradas.Add(incidencia);
            }
            AplicarFiltros();
        }



        public async void AñadirIncidencia(Incidencia nuevaIncidencia, Profesor profesor)
        {
            try
            {
                // Agregar la incidencia al DAO
                await incidenciaDAO.AñadirIncidenciaAsync(nuevaIncidencia);

                // Recargar las incidencias desde el DAO
                await CargarIncidenciasAsync(profesor);

                // Aplicar los filtros actuales
                AplicarFiltros();

                Console.WriteLine("Incidencia añadida correctamente y lista actualizada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al añadir incidencia: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo añadir la incidencia. Inténtalo de nuevo.", "Aceptar");
            }
        }


        public void ActualizarIncidencia(Incidencia incidenciaModificada)
        {
            incidenciaDAO.ActualizarIncidencia(incidenciaModificada);

            var incidenciaExistente = Incidencias.FirstOrDefault(i => i.id == incidenciaModificada.id);
            if (incidenciaExistente != null)
            {
                incidenciaExistente.descripcionDetallada = incidenciaModificada.descripcionDetallada;
                incidenciaExistente.aulaUbicacion = incidenciaModificada.aulaUbicacion;
                incidenciaExistente.fechaIncidencia = incidenciaModificada.fechaIncidencia;
                incidenciaExistente.observaciones = incidenciaModificada.observaciones;
                incidenciaExistente.tiempoInvertido = incidenciaModificada.tiempoInvertido;
                incidenciaExistente.fechaResolucion = incidenciaModificada.fechaResolucion;
            }
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
                        filtrado = filtrado.Where(i => !string.IsNullOrWhiteSpace(i.responsableDni) && i.responsableDni.Contains(FiltroBusqueda, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default: // Sin filtro específico (ambos campos)
                        filtrado = filtrado.Where(i =>
                            i.descripcionDetallada.Contains(FiltroBusqueda, StringComparison.OrdinalIgnoreCase) ||
                            (!string.IsNullOrWhiteSpace(i.responsableDni) &&
                             i.responsableDni.Contains(FiltroBusqueda, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(FiltroResponsable) && FiltroResponsable != " ")
            {
                if (FiltroResponsable == "Sí")
                    filtrado = filtrado.Where(i => !string.IsNullOrWhiteSpace(i.responsableDni) && i.responsableDni != "Ningún profesor asignado").ToList();
                else if (FiltroResponsable == "No")
                    filtrado = filtrado.Where(i => i.responsableDni == "Ningún profesor asignado").ToList();
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
            FiltroBusqueda = string.Empty;
            TipoBusqueda = "Todos";
            FiltroResponsable = null;
            FiltroEstado = null;
            FiltroTipoIncidencia = null;
            FiltroOrdenacion = null;

            AplicarFiltros();

            OnPropertyChanged(nameof(FiltroBusqueda));
            OnPropertyChanged(nameof(TipoBusqueda));
            OnPropertyChanged(nameof(FiltroResponsable));
            OnPropertyChanged(nameof(FiltroEstado));
            OnPropertyChanged(nameof(FiltroTipoIncidencia));
            OnPropertyChanged(nameof(FiltroOrdenacion));
        }

    }
}