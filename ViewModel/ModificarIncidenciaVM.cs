using ProjecteFinal.Models;
using ProjecteFinal.DAO;
using ProjecteFinal.Base;

namespace ProjecteFinal.ViewModel;
public class ModificarIncidenciaVM : BaseViewModel
{
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

    public ModificarIncidenciaVM(Incidencia incidencia)
    {
        Incidencia = incidencia;

        // Establecer la fecha de resolución por defecto si está vacía
        if (Incidencia.fechaResolucion == default)
        {
            Incidencia.fechaResolucion = DateTime.Today;
        }
    }

    public bool GuardarIncidencia()
    {
        // Validación de campos obligatorios
        if (string.IsNullOrWhiteSpace(Incidencia.descripcionDetallada) ||
            string.IsNullOrWhiteSpace(Incidencia.aulaUbicacion) ||
            Incidencia.fechaIncidencia == default)
        {
            return false; // No guardar si faltan campos obligatorios
        }

        // Guardar incidencia en la base de datos
        var dao = new IncidenciaDAO();
        dao.ActualizarIncidencia(Incidencia);
        return true;
    }
}
