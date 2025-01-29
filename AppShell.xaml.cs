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
            Routing.RegisterRoute(nameof(Views.ViewRoles), typeof(Views.ViewRoles));
            Routing.RegisterRoute(nameof(Views.ViewIncidencias), typeof(Views.ViewIncidencias));
            Routing.RegisterRoute(nameof(Views.ViewProfesores), typeof(Views.ViewProfesores));
            Routing.RegisterRoute(nameof(Views.ViewLogs), typeof(Views.ViewLogs));
            Routing.RegisterRoute(nameof(Views.ViewInsertarIncidencia), typeof(Views.ViewInsertarIncidencia));
            Routing.RegisterRoute(nameof(Views.ViewModificarIncidencia), typeof(Views.ViewModificarIncidencia));
            Routing.RegisterRoute(nameof(Views.ViewDepartamentos), typeof(Views.ViewDepartamentos));
            Routing.RegisterRoute(nameof(Views.ViewInsertarModificarDepartamento), typeof(Views.ViewInsertarModificarDepartamento));
            Routing.RegisterRoute(nameof(Views.ViewPerfil), typeof(Views.ViewPerfil));
            Routing.RegisterRoute(nameof(Views.ViewSeleccionarProfesor), typeof(Views.ViewSeleccionarProfesor));
            Routing.RegisterRoute(nameof(Views.ViewDetalleIncidencia), typeof(Views.ViewDetalleIncidencia));
            Routing.RegisterRoute(nameof(Views.ViewModificarRol), typeof(Views.ViewModificarRol));
            Routing.RegisterRoute(nameof(Views.ViewInsertarRol), typeof(Views.ViewInsertarRol));
            Routing.RegisterRoute(nameof(Views.ProfesorMenu), typeof(Views.ProfesorMenu));
        }


    }
}
