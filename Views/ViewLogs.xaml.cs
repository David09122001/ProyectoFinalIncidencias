using GestorIncidencias.ViewModel;

namespace GestorIncidencias.Views;

public partial class ViewLogs : ContentPage
{
	public ViewLogs()
	{
		InitializeComponent();
        BindingContext = new VerLogsVM();
    }

}