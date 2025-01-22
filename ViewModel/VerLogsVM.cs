using ProjecteFinal.Models;
using ProjecteFinal.DAO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ProjecteFinal.Base;

namespace ProjecteFinal.ViewModel
{
    public class VerLogsVM : BaseViewModel
    {
        private readonly LogDAO logDAO = new LogDAO();
        private ObservableCollection<Log> _logs;
        private string _filtroIncidenciaId;

        public ObservableCollection<Log> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged();
            }
        }

        public string FiltroIncidenciaId
        {
            get => _filtroIncidenciaId;
            set
            {
                _filtroIncidenciaId = value;
                OnPropertyChanged();
            }
        }

        public ICommand FiltrarCommand { get; }
        public ICommand BorrarFiltroCommand { get; }

        public VerLogsVM()
        {
            FiltrarCommand = new Command(async () => await FiltrarLogsAsync());
            BorrarFiltroCommand = new Command(async () => await CargarTodosLosLogsAsync());
            CargarTodosLosLogsAsync().ConfigureAwait(false);
        }

        private async Task CargarTodosLosLogsAsync()
        {
            Logs = await logDAO.ObtenerLogsAsync();
        }

        private async Task FiltrarLogsAsync()
        {
            if (int.TryParse(FiltroIncidenciaId, out int incidenciaId))
            {
                Logs = await logDAO.ObtenerLogsPorIncidenciaAsync(incidenciaId);
            }
        }
    }
}
