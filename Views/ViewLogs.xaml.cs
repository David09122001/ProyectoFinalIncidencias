using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views;

public partial class ViewLogs : ContentPage
{
	public ViewLogs()
	{
		InitializeComponent();
        BindingContext = new VerLogsVM();
    }

}