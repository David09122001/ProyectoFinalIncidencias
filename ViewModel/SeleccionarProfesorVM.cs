using ProjecteFinal.Models;
using ProjecteFinal.DAO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ProjecteFinal.Base;

namespace ProjecteFinal.ViewModel;

public class SeleccionarProfesorVM : BaseViewModel
{
    private ProfesorDAO profesorDAO = new ProfesorDAO();
    public ObservableCollection<Profesor> ProfesoresFiltrados { get; set; } = new ObservableCollection<Profesor>();

    private string _filtro;
    public string Filtro
    {
        get => _filtro;
        set
        {
            _filtro = value;
            OnPropertyChanged();
            FiltrarProfesores();
        }
    }

    // Evento para notificar la selección del profesor
    public event Action<Profesor> ProfesorSeleccionado;

    // Comando para seleccionar un profesor
    public ICommand SeleccionarProfesorCommand { get; }

    public SeleccionarProfesorVM()
    {
        CargarProfesores();
        SeleccionarProfesorCommand = new Command<Profesor>(profesor =>
        {
            // Notificar al suscriptor que se ha seleccionado un profesor
            ProfesorSeleccionado?.Invoke(profesor);
        });
    }

    // Cargar todos los profesores desde la base de datos
    private async void CargarProfesores()
    {
        var profesores = await profesorDAO.BuscarTodosAsync();
        ProfesoresFiltrados = new ObservableCollection<Profesor>(profesores);
        OnPropertyChanged(nameof(ProfesoresFiltrados));
    }

    // Filtrar los profesores en tiempo real
    private void FiltrarProfesores()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            // Si el filtro está vacío, recargar la lista completa
            CargarProfesores();
        }
        else
        {
            // Filtrar profesores por nombre o email
            var profesores = ProfesoresFiltrados.Where(p =>
                p.nombre.Contains(Filtro, StringComparison.OrdinalIgnoreCase) ||
                p.email.Contains(Filtro, StringComparison.OrdinalIgnoreCase))
                .ToList();
            ProfesoresFiltrados = new ObservableCollection<Profesor>(profesores);
            OnPropertyChanged(nameof(ProfesoresFiltrados));
        }
    }
}
