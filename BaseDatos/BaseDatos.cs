using SQLite;
using ProjecteFinal.Config;
using System.IO;
using System.Threading.Tasks;
using ProjecteFinal.Models;

namespace ProjecteFinal.BaseDatos
{
    public static class BaseDatos
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            if (connection != null)
            {
                return connection;
            }
            else
            {
                connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                return connection;
            }
        }

        // Inicializar base de datos
        public static async Task InicializarBaseDatosAsync()
        {
            var db = GetConnection();

            // Eliminar tablas existentes
            EliminarTablasAsync();

            // Crear tablas nuevas
            CrearTablasAsync();

            // Insertar datos iniciales
            InsertarDatosInicialesAsync();
        }

        // Eliminar tablas existentes
        public static async Task EliminarTablasAsync()
        {
            GetConnection().DropTableAsync<Adjunto>().Wait();
            GetConnection().DropTableAsync<Departamento>().Wait();
            GetConnection().DropTableAsync<Profesor>().Wait();
            GetConnection().DropTableAsync<Incidencia>().Wait();
            GetConnection().DropTableAsync<Incidencia_HW>().Wait();
            GetConnection().DropTableAsync<Incidencia_SW>().Wait();
            GetConnection().DropTableAsync<Incidencia_Red>().Wait();
            GetConnection().DropTableAsync<Log>().Wait();
            GetConnection().DropTableAsync<Permiso>().Wait();
            GetConnection().DropTableAsync<Rol>().Wait();
            GetConnection().DropTableAsync<RolPermiso>().Wait();
        }

        // Crear tablas nuevas
        public static async Task CrearTablasAsync()
        {
            GetConnection().CreateTableAsync<Departamento>().Wait();
            GetConnection().CreateTableAsync<Profesor>().Wait();
            GetConnection().CreateTableAsync<Incidencia>().Wait();
            GetConnection().CreateTableAsync<Incidencia_HW>().Wait();
            GetConnection().CreateTableAsync<Incidencia_SW>().Wait();
            GetConnection().CreateTableAsync<Incidencia_Red>().Wait();
            GetConnection().CreateTableAsync<Adjunto>().Wait();
            GetConnection().CreateTableAsync<Log>().Wait();
            GetConnection().CreateTableAsync<Permiso>().Wait();
            GetConnection().CreateTableAsync<Rol>().Wait();
            GetConnection().CreateTableAsync<RolPermiso>().Wait();
        }

        // Insertar datos iniciales
        public static async Task InsertarDatosInicialesAsync()
        {
            // Insertar datos en Departamentos
            GetConnection().InsertAsync(new Departamento { codigo = "INF", nombre = "Informática", ubicacion = "Aula 204" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "ADM", nombre = "Administración", ubicacion = "Aula 208" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "MAT", nombre = "Matemáticas", ubicacion = "Aula 210" }).Wait();

            // Insertar roles
            GetConnection().InsertAsync(new Rol { nombre = "Profesor" }).Wait();
            GetConnection().InsertAsync(new Rol { nombre = "Mantenimiento TIC" }).Wait();
            GetConnection().InsertAsync(new Rol { nombre = "Administrador" }).Wait();
            GetConnection().InsertAsync(new Rol { nombre = "Directivo" }).Wait();

            // Insertar permisos
            GetConnection().InsertAsync(new Permiso { descripcion = "Añadir incidencias" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Modificar/borrar incidencias" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Modificar/borrar mis incidencias" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Añadir/borrar/modificar tipo de HW" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Alta/baja/modificar roles y permisos" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Operaciones importación y exportación" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Informes sobre incidencias" }).Wait();

            // Insertar relaciones roles-permisos
        
            //Profesor
            GetConnection().InsertAsync(new RolPermiso { rolId = 1, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 1, permisoCodigo = 3 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 1, permisoCodigo = 7 }).Wait();
            //Mantenimiento TIC
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 2 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 4 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 5 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 7 }).Wait();
         
            //Administrador
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 2 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 3 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 4 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 5 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 6 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 7 }).Wait();
            //Directivo
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 2 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 4 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 5 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 7 }).Wait();

            // Insertar profesores
            GetConnection().InsertAsync(new Profesor
            {
                dni = "20881900X",
                nombre = "Tester",
                departamentoCodigo = "INF",
                email = "1",
                contrasena = "1",
                rol_id = 3
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "20761603X",
                nombre = "Profesor Prueba",
                departamentoCodigo = "INF",
                email = "profesorprueba@gmail.com",
                contrasena = "1234",
                rol_id = 1
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "20889987X",
                nombre = "Profesor Prueba2",
                departamentoCodigo = "INF",
                email = "profesorprueba2@gmail.com",
                contrasena = "1234",
                rol_id = 1
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "20761223X",
                nombre = "Profesor Prueba3",
                departamentoCodigo = "INF",
                email = "profesorprueba3@gmail.com",
                contrasena = "1234",
                rol_id = 1
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "87654321B",
                nombre = "MantenimientoTIC Prueba",
                departamentoCodigo = "ADM",
                email = "mantenimientoticprueba@gmail.com",
                contrasena = "1234",
                rol_id = 2
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "20762603X",
                nombre = "Administrador Prueba",
                departamentoCodigo = "INF",
                email = "administradorprueba@gmail.com",
                contrasena = "1234",
                rol_id = 3
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "87454321B",
                nombre = "Directivo Prueba",
                departamentoCodigo = "ADM",
                email = "directivoprueba@gmail.com",
                contrasena = "1234",
                rol_id = 4
            }).Wait();

            // Insertar incidencias
            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-1),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "20761603X",
                aulaUbicacion = "Aula 101",
                descripcionDetallada = "El ordenador no enciende",
                observaciones = "Revisar fuente de alimentación",
                estado = EstadoIncidencia.Comunicada.ToString(),
                tiempoInvertido = 0,
                comunicada = false
            }).Wait();

            GetConnection().InsertAsync(new Incidencia_HW
            {
                id = 1,
                dispositivo = "Ordenador",
                modelo = "HP Pavilion",
                numeroSerie = "12345-67890"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-2),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "20889987X",
                aulaUbicacion = "Aula 202",
                descripcionDetallada = "No funciona el software",
                observaciones = "Actualizar software",
                estado = EstadoIncidencia.Resolviendo.ToString(),
                tiempoInvertido = 2,
                comunicada = true
            }).Wait();

            GetConnection().InsertAsync(new Incidencia_SW
            {
                id = 2,
                sistemaOperativo = "Windows 10",
                aplicacion = "Office 365"
            }).Wait();


            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-2),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "20761223X",
                aulaUbicacion = "Aula 212",
                descripcionDetallada = "No funciona el internet",
                observaciones = "He abierto el navegador y dice que no hay acceso a la red",
                estado = EstadoIncidencia.Resolviendo.ToString(),
                tiempoInvertido = 2,
                comunicada = true
            }).Wait();

            GetConnection().InsertAsync(new Incidencia_Red
            {
                id = 3,
                dispositivoAfectado = "Ordenador HP sobremesa"
            }).Wait();

            // Insertar logs
            GetConnection().InsertAsync(new Log { incidenciaId = 1, estado = 1 }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 2, estado = 2 }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 3, estado = 1 }).Wait();


            //Insertar tipos HW
            GetConnection().InsertAsync(new Incidencia_HW { id = -1, dispositivo = "Servidor" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -2, dispositivo = "Ordenador" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -3, dispositivo = "Monitor" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -4, dispositivo = "Impresora" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -5, dispositivo = "Router" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -6, dispositivo = "Switch" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -7, dispositivo = "Proyector" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -8, dispositivo = "Ratón" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -9, dispositivo = "Teclado" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -10, dispositivo = "Altavoces" }).Wait();
            GetConnection().InsertAsync(new Incidencia_HW { id = -11, dispositivo = "Otro tipo de HW" }).Wait();

        }
    }
}
