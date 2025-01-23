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

    public event Action<Profesor> ProfesorSeleccionado;

    public ICommand SeleccionarProfesorCommand { get; }

    public SeleccionarProfesorVM()
    {
        CargarProfesores();
        SeleccionarProfesorCommand = new Command<Profesor>(profesor =>
        {
            ProfesorSeleccionado?.Invoke(profesor);
        });
    }

    private async void CargarProfesores()
    {
        var profesores = await profesorDAO.BuscarTodosAsync();
        ProfesoresFiltrados = new ObservableCollection<Profesor>(profesores);
        OnPropertyChanged(nameof(ProfesoresFiltrados));
    }

    private void FiltrarProfesores()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            CargarProfesores();
        }
        else
        {
            var profesores = ProfesoresFiltrados.Where(p =>
                p.nombre.Contains(Filtro, StringComparison.OrdinalIgnoreCase) ||
                p.email.Contains(Filtro, StringComparison.OrdinalIgnoreCase))
                .ToList();
            ProfesoresFiltrados = new ObservableCollection<Profesor>(profesores);
            OnPropertyChanged(nameof(ProfesoresFiltrados));
        }
    }
}
