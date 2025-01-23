using ProjecteFinal.ViewModel;

namespace ProjecteFinal.Views;

public partial class ViewInformes : ContentPage
{
    public ViewInformes()
    {
        InitializeComponent();
        BindingContext = new InformesVM();
    }
}
