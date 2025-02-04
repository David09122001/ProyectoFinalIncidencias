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

        public static async Task InicializarBaseDatosAsync()
        {
            var db = GetConnection();

            EliminarTablasAsync();

            CrearTablasAsync();

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
            GetConnection().InsertAsync(new Departamento { codigo = "BIO", nombre = "Biología", ubicacion = "Aula 103" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "LEN", nombre = "Lengua y Literatura", ubicacion = "Aula 201" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "ING", nombre = "Inglés", ubicacion = "Aula 202" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "HIS", nombre = "Historia", ubicacion = "Aula 203" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "GEO", nombre = "Geografía", ubicacion = "Aula 205" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "EDF", nombre = "Educación Física", ubicacion = "Gimnasio" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "MUS", nombre = "Música", ubicacion = "Aula de Música" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "FIL", nombre = "Filosofía", ubicacion = "Aula 206" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "ECO", nombre = "Economía", ubicacion = "Aula 207" }).Wait();
            GetConnection().InsertAsync(new Departamento { codigo = "TEC", nombre = "Tecnología", ubicacion = "Taller de Tecnología" }).Wait();

            // Insertar roles
            GetConnection().InsertAsync(new Rol { nombre = "Profesor" }).Wait();
            GetConnection().InsertAsync(new Rol { nombre = "Mantenimiento TIC" }).Wait();
            GetConnection().InsertAsync(new Rol { nombre = "Administrador" }).Wait();
            GetConnection().InsertAsync(new Rol { nombre = "Directivo" }).Wait();

            //Insertar permisos
            GetConnection().InsertAsync(new Permiso { descripcion = "Añadir Incidencias" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Modificar/Eliminar Incidencias" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Gestionar hardware" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Gestionar permisos y roles" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Generar informes" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Gestionar profesores" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Gestionar departamentos" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Ver logs" }).Wait();
            GetConnection().InsertAsync(new Permiso { descripcion = "Asignar responsable" }).Wait();


            // Insertar relaciones roles-permisos

            //Profesor
            GetConnection().InsertAsync(new RolPermiso { rolId = 1, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 1, permisoCodigo = 2 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 1, permisoCodigo = 5 }).Wait();
           
            //Mantenimiento TIC
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 2 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 3 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 4 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 5 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 6 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 7 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 2, permisoCodigo = 9 }).Wait();

            //Administrador
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 2 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 3 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 4 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 6 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 7 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 8 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 3, permisoCodigo = 9 }).Wait();

            //Directivo
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 1 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 2 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 3 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 4 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 5 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 6 }).Wait();
            GetConnection().InsertAsync(new RolPermiso { rolId = 4, permisoCodigo = 7 }).Wait();

            // Insertar profesores
            GetConnection().InsertAsync(new Profesor
            {
                dni = "20881963X",
                nombre = "Admin",
                departamentoCodigo = "INF",
                email = "1",
                contrasena = "1",
                rol_id = 3
            }).Wait();


            // Insertar profesores estándar con DNIs correctos y correos @edu.gva
            GetConnection().InsertAsync(new Profesor
            {
                dni = "86157907E",
                nombre = "Ana Pérez",
                departamentoCodigo = "INF",
                email = "ana.perez@edu.gva",
                contrasena = "1234",
                rol_id = 1
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "82092814H",
                nombre = "Carlos Gómez",
                departamentoCodigo = "ADM",
                email = "carlos.gomez@edu.gva",
                contrasena = "1234",
                rol_id = 2
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "12821444W",
                nombre = "Elena Fernández",
                departamentoCodigo = "MAT",
                email = "elena.fernandez@edu.gva",
                contrasena = "1234",
                rol_id = 1
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "51936794B",
                nombre = "Javier Ruiz",
                departamentoCodigo = "BIO",
                email = "javier.ruiz@edu.gva",
                contrasena = "1234",
                rol_id = 3
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "87884040M",
                nombre = "Lucía Ortega",
                departamentoCodigo = "TEC",
                email = "lucia.ortega@edu.gva",
                contrasena = "1234",
                rol_id = 4
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "38866809Y",
                nombre = "Manuel García",
                departamentoCodigo = "HIS",
                email = "manuel.garcia@edu.gva",
                contrasena = "1234",
                rol_id = 2
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "95508638B",
                nombre = "Sara López",
                departamentoCodigo = "LEN",
                email = "sara.lopez@edu.gva",
                contrasena = "1234",
                rol_id = 3
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "24884764Y",
                nombre = "Pedro Domínguez",
                departamentoCodigo = "GEO",
                email = "pedro.dominguez@edu.gva",
                contrasena = "1234",
                rol_id = 1
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "13211237Z",
                nombre = "Marta Navarro",
                departamentoCodigo = "EDF",
                email = "marta.navarro@edu.gva",
                contrasena = "1234",
                rol_id = 4
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "25474715F",
                nombre = "David Martín",
                departamentoCodigo = "ECO",
                email = "david.martin@edu.gva",
                contrasena = "1234",
                rol_id = 2
            }).Wait();

            GetConnection().InsertAsync(new Profesor
            {
                dni = "20423550X",
                nombre = "David",
                departamentoCodigo = "INF",
                email = "david.carcer09@gmail.com",
                contrasena = "1",
                rol_id = 3
            }).Wait();


            // Insertar incidencias
            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-5),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "86157907E", // Ana Pérez (INF)
                aulaUbicacion = "Aula 101",
                descripcionDetallada = "El ordenador no enciende",
                estado = "Sin asignar"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-3),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "12821444W", // Elena Fernández (MAT)
                aulaUbicacion = "Aula 302",
                descripcionDetallada = "No se detecta conexión a internet",
                estado = "Sin asignar"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-4),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "51936794B", // Javier Ruiz (BIO)
                responsableDni = "82092814H", // Carlos Gómez (ADM)
                aulaUbicacion = "Aula 202",
                descripcionDetallada = "El proyector no funciona",
                observaciones = "Revisar conexiones de HDMI",
                estado = "Asignada"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-7),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "38866809Y", // Manuel García (HIS)
                responsableDni = "13211237Z", // Marta Navarro (EDF)
                aulaUbicacion = "Gimnasio",
                descripcionDetallada = "Altavoz del aula no emite sonido",
                observaciones = "Puede ser problema de cableado",
                estado = "Asignada"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-6),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "95508638B", // Sara López (LEN)
                responsableDni = "24884764Y", // Pedro Domínguez (GEO)
                aulaUbicacion = "Aula 402",
                descripcionDetallada = "El software de gestión de alumnos no carga",
                observaciones = "Reinstalando la aplicación",
                estado = "En proceso"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-2),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "25474715F", // David Martín (ECO)
                responsableDni = "87884040M", // Lucía Ortega (TEC)
                aulaUbicacion = "Laboratorio 2",
                descripcionDetallada = "Fallo en switch de red, no conecta",
                observaciones = "Pendiente de cambiar switch",
                estado = "Pendiente"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia
            {
                fechaIncidencia = DateTime.Now.AddDays(-10),
                fechaIntroduccion = DateTime.Now,
                profesorDni = "13211237Z", // Marta Navarro (EDF)
                responsableDni = "51936794B", // Javier Ruiz (BIO)
                aulaUbicacion = "Aula 103",
                descripcionDetallada = "La impresora no imprime correctamente",
                observaciones = "Se ha reemplazado el cartucho de tinta",
                estado = "Resuelta",
                tiempoInvertido = 225,
                fechaResolucion = DateTime.Now.AddDays(-1)
            }).Wait();

            // Hardware (HW)
            GetConnection().InsertAsync(new Incidencia_HW
            {
                id = 1,
                dispositivo = "Ordenador",
                modelo = "HP Pavilion",
                numeroSerie = "12345-67890"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia_HW
            {
                id = 2,
                dispositivo = "Proyector",
                modelo = "Epson EB-X41",
                numeroSerie = "PJT-987654"
            }).Wait();

            // Software (SW)
            GetConnection().InsertAsync(new Incidencia_SW
            {
                id = 3,
                sistemaOperativo = "Windows 10",
                aplicacion = "Microsoft Teams"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia_SW
            {
                id = 4,
                sistemaOperativo = "Ubuntu 20.04",
                aplicacion = "LibreOffice"
            }).Wait();

            // Red
            GetConnection().InsertAsync(new Incidencia_Red
            {
                id = 5,
                dispositivoAfectado = "Switch Cisco 2960"
            }).Wait();

            GetConnection().InsertAsync(new Incidencia_Red
            {
                id = 6,
                dispositivoAfectado = "Punto de acceso WiFi"
            }).Wait();



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

            // LOGS
            GetConnection().InsertAsync(new Log { incidenciaId = 1, estado = "Sin asignar", fecha = DateTime.Now.AddDays(-5) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 1, estado = "Asignada", fecha = DateTime.Now.AddDays(-4) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 1, estado = "En proceso", fecha = DateTime.Now.AddDays(-3) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 1, estado = "Pendiente", fecha = DateTime.Now.AddDays(-2) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 1, estado = "Resuelta", fecha = DateTime.Now.AddDays(-1) }).Wait();

            GetConnection().InsertAsync(new Log { incidenciaId = 2, estado = "Sin asignar", fecha = DateTime.Now.AddDays(-3) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 2, estado = "Asignada", fecha = DateTime.Now.AddDays(-2) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 2, estado = "En proceso", fecha = DateTime.Now.AddDays(-1) }).Wait();

            GetConnection().InsertAsync(new Log { incidenciaId = 3, estado = "Sin asignar", fecha = DateTime.Now.AddDays(-7) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 3, estado = "Asignada", fecha = DateTime.Now.AddDays(-6) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 3, estado = "En proceso", fecha = DateTime.Now.AddDays(-5) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 3, estado = "Pendiente", fecha = DateTime.Now.AddDays(-2) }).Wait();

            GetConnection().InsertAsync(new Log { incidenciaId = 4, estado = "Sin asignar", fecha = DateTime.Now.AddDays(-4) }).Wait();

            GetConnection().InsertAsync(new Log { incidenciaId = 5, estado = "Sin asignar", fecha = DateTime.Now.AddDays(-3) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 5, estado = "Asignada", fecha = DateTime.Now.AddDays(-2) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 5, estado = "Pendiente", fecha = DateTime.Now.AddDays(-1) }).Wait();

            GetConnection().InsertAsync(new Log { incidenciaId = 6, estado = "Sin asignar", fecha = DateTime.Now.AddDays(-10) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 6, estado = "Asignada", fecha = DateTime.Now.AddDays(-8) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 6, estado = "En proceso", fecha = DateTime.Now.AddDays(-6) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 6, estado = "Pendiente", fecha = DateTime.Now.AddDays(-4) }).Wait();
            GetConnection().InsertAsync(new Log { incidenciaId = 6, estado = "Resuelta", fecha = DateTime.Now.AddDays(-1) }).Wait();


        }
    }
}
