namespace ProjecteFinal
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Views.ViewLogin), typeof(Views.ViewLogin));
            Routing.RegisterRoute(nameof(Views.MainMenu), typeof(Views.MainMenu));
            Routing.RegisterRoute(nameof(Views.ViewTiposHW), typeof(Views.ViewTiposHW));
            Routing.RegisterRoute(nameof(Views.Informes), typeof(Views.Informes));
            Routing.RegisterRoute(nameof(Views.ViewIncidencias), typeof(Views.ViewIncidencias));
            Routing.RegisterRoute(nameof(Views.Profesores), typeof(Views.Profesores));
            Routing.RegisterRoute(nameof(Views.ViewLogs), typeof(Views.ViewLogs));
            Routing.RegisterRoute(nameof(Views.ViewInsertarIncidencia), typeof(Views.ViewInsertarIncidencia));
            Routing.RegisterRoute(nameof(Views.ViewModificarIncidencia), typeof(Views.ViewModificarIncidencia));
            Routing.RegisterRoute(nameof(Views.ViewDepartamentos), typeof(Views.ViewDepartamentos));
            Routing.RegisterRoute(nameof(Views.ViewInsertarModificarDepartamento), typeof(Views.ViewInsertarModificarDepartamento));
            Routing.RegisterRoute(nameof(Views.ViewPerfil), typeof(Views.ViewPerfil));
            Routing.RegisterRoute(nameof(Views.ViewSeleccionarProfesor), typeof(Views.ViewSeleccionarProfesor));
        }
    }
}
