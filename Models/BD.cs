using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
namespace Swidge.Models;
public class BD
{
    
    private string _connectionString = @"Server=localhost;Database=Swidge;Integrated Security=True;TrustServerCertificate=True;";

    public List<Usuario> GetUsuarios() {

        List<Usuario> usuarios = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Usuario";
            usuarios = connection.Query<Usuario>(query).ToList();

            foreach (Usuario usuario in usuarios)
            {
                usuario.NumeroTarjeta = string.IsNullOrWhiteSpace(usuario.NumeroTarjeta) ? null : EncryptionHelper.Decrypt(usuario.NumeroTarjeta);
                usuario.CodigoSeguridad = string.IsNullOrWhiteSpace(usuario.CodigoSeguridad) ? null : EncryptionHelper.Decrypt(usuario.CodigoSeguridad);
            }

        }
        return usuarios;

    }
    public List<Vendedor> GetVendedores() {

        List<Vendedor> vendedores = new List<Vendedor>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Vendedor";
            vendedores = connection.Query<Vendedor>(query).ToList();

        }
        return vendedores;

    }

    public List<Comentario> GetComentarios() {

        List<Comentario> comentarios = new List<Comentario>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT ID, Calificacion, Comentario AS ComentarioTexto, DNIusuario FROM dbo.Comentario";
            comentarios = connection.Query<Comentario>(query).ToList();

        }
        return comentarios;

    }

    public List<Entrada> GetEntradas()
    {
        List<Entrada> entradas = new List<Entrada>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            entradas = connection.Query<Entrada>(
                "SELECT * FROM dbo.Entrada WHERE Vendida = 0"
            ).ToList();

            foreach (Entrada entrada in entradas)
            {
                entrada.Evento = connection.QueryFirstOrDefault<Evento>(
                    "SELECT * FROM dbo.Evento WHERE ID = @ID",
                    new { ID = entrada.IDEvento });

                if (entrada.Evento != null)
                {
                    dynamic categoria = connection.QueryFirstOrDefault<dynamic>(
                        "SELECT Nombre FROM dbo.Categoria WHERE ID = @ID",
                        new { ID = entrada.Evento.IDCategoria });

                    entrada.Evento.Categoria = categoria?.Nombre ?? "";

                    Estadio? estadio = connection.QueryFirstOrDefault<Estadio>(
                        "SELECT * FROM dbo.Estadio WHERE ID = @ID",
                        new { ID = entrada.Evento.IDEstadio });

                    entrada.Evento.Lugar = estadio?.NombreLugar ?? estadio?.Nombre ?? "";
                    estadio = connection.QueryFirstOrDefault<Estadio>(
                        "SELECT * FROM dbo.Estadio WHERE ID = @ID",
                        new { ID = entrada.Evento.IDEstadio });

                    entrada.Evento.Lugar = estadio?.NombreLugar ?? estadio?.Nombre ?? "";
                    entrada.Evento.CantidadEntradas = GetCantidadEntradasPorEvento(entrada.IDEvento);
                }
            }
        }
        return entradas;
    }

    public List<Evento> GetEventos() {

        List<Evento> eventos = new List<Evento>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Evento";
            eventos = connection.Query<Evento>(query).ToList();

        }
        return eventos;

    }

    public List<Reclamo> GetReclamos() {

        List<Reclamo> reclamos = new List<Reclamo>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Reclamo";
            reclamos = connection.Query<Reclamo>(query).ToList();

        }
        return reclamos;

    }

    public List<Transaccion> GetTransacciones() {

        List<Transaccion> transacciones = new List<Transaccion>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Transaccion";
            transacciones = connection.Query<Transaccion>(query).ToList();

        }
        return transacciones;

    }

    public List<Ubicacion> GetUbicacion() {

        List<Ubicacion> ubicaciones = new List<Ubicacion>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM dbo.Ubicacion";
            ubicaciones = connection.Query<Ubicacion>(query).ToList();

        }
        return ubicaciones;

    }
    public Evento GetEventoPorId(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM dbo.Evento WHERE ID = @ID";
            return connection.QueryFirstOrDefault<Evento>(query, new { ID = id });
        }
    }
    public Usuario? ObtenerUsuarioParaLogin(string nombreUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            // Se busca solo por nombre de usuario: el hash se verifica en C# con BCrypt.Verify.
            string query = @"SELECT TOP 1 * FROM Usuario WHERE NombreUsuario = @NombreUsuario";
            return connection.QueryFirstOrDefault<Usuario>(query, new { NombreUsuario = nombreUsuario });
        }
    }

    public bool ExisteDNI(int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM [dbo].[Usuario] WHERE [DNI] = @DNI";
            return connection.ExecuteScalar<int>(query, new { DNI = dni }) > 0;
        }
    }

    public bool ExisteNombreUsuario(string nombreUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM [dbo].[Usuario] WHERE [NombreUsuario] = @NombreUsuario";
            return connection.ExecuteScalar<int>(query, new { NombreUsuario = nombreUsuario }) > 0;
        }
    }

    public bool ExisteMail(string mail)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM [dbo].[Usuario] WHERE [Mail] = @Mail";
            return connection.ExecuteScalar<int>(query, new { Mail = mail }) > 0;
        }
    }

    public int AgregarUsuario(Usuario usuario)
    {
    int resultado;
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string storedProcedure = "InsertarUsuario";
        resultado = connection.QueryFirstOrDefault<int>(
            storedProcedure,
            new
            {
                DNI = usuario.DNI,
                Mail = usuario.Mail,
                NombreUsuario = usuario.NombreUsuario,
                Contraseña = usuario.Contraseña,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                FechaNacimiento = usuario.FechaNacimiento,
                Ciudad = usuario.Ciudad
            },
            commandType: CommandType.StoredProcedure);
    }
    return resultado;
    }




    public Usuario? ObtenerUsuarioPorDNI(int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT TOP 1 * FROM [dbo].[Usuario] WHERE [DNI] = @DNI";
            Usuario? usuario = connection.QueryFirstOrDefault<Usuario>(query, new { DNI = dni });

            if (usuario == null)
                return null;

            usuario.NumeroTarjeta = string.IsNullOrWhiteSpace(usuario.NumeroTarjeta) ? null : EncryptionHelper.Decrypt(usuario.NumeroTarjeta);
            usuario.CodigoSeguridad = string.IsNullOrWhiteSpace(usuario.CodigoSeguridad) ? null : EncryptionHelper.Decrypt(usuario.CodigoSeguridad);
            return usuario;
        }
    }

    public void CambiarContraseña(int dni, string nuevaContrasena)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE [dbo].[Usuario] SET [Contraseña] = @Contrasena WHERE [DNI] = @DNI";
            connection.Execute(query, new { DNI = dni, Contrasena = nuevaContrasena });
        }
    }

    public Entrada? GetEntradaPorId(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"SELECT * FROM dbo.Entrada WHERE ID = @ID AND Vendida = 0";

            Entrada? entrada = connection.QueryFirstOrDefault<Entrada>(
                query,
                new { ID = id }
            );

            if (entrada == null)
                return null;

            CargarDetalleEntrada(connection, entrada);
            return entrada;
        }
    }

    public Entrada? GetEntradaDetallePorId(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"SELECT * FROM dbo.Entrada WHERE ID = @ID";

            Entrada? entrada = connection.QueryFirstOrDefault<Entrada>(
                query,
                new { ID = id }
            );

            if (entrada == null)
                return null;

            CargarDetalleEntrada(connection, entrada);
            return entrada;
        }
    }

    private static void CargarDetalleEntrada(SqlConnection connection, Entrada entrada)
    {
        entrada.Evento = connection.QueryFirstOrDefault<Evento>(
            "SELECT * FROM dbo.Evento WHERE ID = @ID",
            new { ID = entrada.IDEvento });

        if (entrada.Evento != null)
        {
            var estadio = connection.QueryFirstOrDefault<Estadio>(
                "SELECT * FROM dbo.Estadio WHERE ID = @ID",
                new { ID = entrada.Evento.IDEstadio });

            entrada.Evento.Lugar = estadio?.NombreLugar ?? estadio?.Nombre ?? "";
        }

        entrada.Vendedor = connection.QueryFirstOrDefault<Usuario>(
            @"SELECT u.*
              FROM dbo.Usuario u
              INNER JOIN dbo.Vendedor v ON v.DNIusuario = u.DNI
              WHERE v.ID = @IDVendedor",
            new { IDVendedor = entrada.IDVendedor });

        if (entrada.IDubicacion > 0)
        {
            entrada.Ubicacion = connection.QueryFirstOrDefault<Ubicacion>(
                "SELECT * FROM dbo.Ubicacion WHERE ID = @ID",
                new { ID = entrada.IDubicacion });
        }

        if (entrada.DNIusuario.HasValue)
        {
            entrada.Comprador = connection.QueryFirstOrDefault<Usuario>(
                "SELECT * FROM dbo.Usuario WHERE DNI = @DNI",
                new { DNI = entrada.DNIusuario.Value });
        }
    }

    public Evento? BuscarEventoPorId(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "BuscarEventoPorId";

            Evento? evento = connection.Query<Evento>(
                storedProcedure,
                new { ID = id },
                commandType: CommandType.StoredProcedure
            ).FirstOrDefault();

            return evento;
        }
    }
    public Entrada BuscarEntradaPorId(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "BuscarEntradaPorId";

            Entrada entrada = connection.QueryFirstOrDefault<Entrada>(
                storedProcedure,
                new { ID = id },
                commandType: CommandType.StoredProcedure
            );

            return entrada;
        }
    }

    public Vendedor? BuscarVendedorPorID(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "BuscarVendedorPorID";

            Vendedor? vendedor = connection.Query<Vendedor>(
                storedProcedure,
                new { ID = id },
                commandType: CommandType.StoredProcedure
            ).FirstOrDefault();

            return vendedor;
        }
    }
    public Vendedor? BuscarVendedorPorDNIusuario(int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "BuscarVendedorPorDNIusuario";

            Vendedor? vendedor = connection.Query<Vendedor>(
                storedProcedure,
                new { DNIusuario = dni },
                commandType: CommandType.StoredProcedure
            ).FirstOrDefault();

            return vendedor;
        }
    }
    public Ubicacion BuscarUbicacionPorIDevento(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "BuscarUbicacionPorIDevento";

            Ubicacion ubicacion = connection.QueryFirstOrDefault<Ubicacion>(
                storedProcedure,
                new { IDevento = id },
                commandType: CommandType.StoredProcedure
            );

            return ubicacion;
        }
    }
    public Ubicacion BuscarUbicacionPorId(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "BuscarUbicacionPorId";

            Ubicacion ubicacion = connection.QueryFirstOrDefault<Ubicacion>(
                storedProcedure,
                new { ID = id },
                commandType: CommandType.StoredProcedure
            );

            return ubicacion;
        }
    }
    public int InsertarVendedor(int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"INSERT INTO dbo.Vendedor (DNIusuario)
                             VALUES (@DNI);
                             SELECT CAST(SCOPE_IDENTITY() AS int);";

            return connection.ExecuteScalar<int>(query, new { DNI = dni });
        }
    }

    public int PublicarEntrada(int idEvento, int idVendedor, decimal precio, string qr, int idUbicacion)
    {
        string qrFinal = string.IsNullOrWhiteSpace(qr)
            ? $"QR_{DateTime.Now:yyyyMMddHHmmss}_{idEvento}_{idVendedor}"
            : qr;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "PublicarEntrada";
            return connection.ExecuteScalar<int>(storedProcedure, new
            {
                IDEvento = idEvento,
                IDvendedor = idVendedor,
                Precio = precio,
                QR = qrFinal,
                IDubicacion = idUbicacion
            }, commandType: CommandType.StoredProcedure);
        }
    }

    public void MarcarEntradaVendida(int idEntrada, int dniUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                connection.Execute(
                    "dbo.MarcarEntradaVendida",
                    new { IDEntrada = idEntrada, DNIusuario = dniUsuario },
                    transaction,
                    commandType: CommandType.StoredProcedure);

                int existente = connection.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM dbo.Transaccion WHERE IDentrada = @IDEntrada",
                    new { IDEntrada = idEntrada },
                    transaction);

                if (existente == 0)
                {
                    connection.Execute(
                        @"INSERT INTO dbo.Transaccion
                          (DNIusuario, MontoTotal, Comision, Fecha, Estado, IDvendedor, IDentrada)
                          SELECT @DNIusuario, Precio, 0, GETDATE(), 'COMPLETADO', IDvendedor, ID
                          FROM dbo.Entrada
                          WHERE ID = @IDEntrada",
                        new { IDEntrada = idEntrada, DNIusuario = dniUsuario },
                        transaction);
                }

                transaction.Commit();
            }
        }
    }

    public void RegistrarComprasPendientes(int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(
                @"INSERT INTO dbo.Transaccion
                  (DNIusuario, MontoTotal, Comision, Fecha, Estado, IDvendedor, IDentrada)
                  SELECT e.DNIusuario, e.Precio, 0, GETDATE(), 'COMPLETADO', e.IDvendedor, e.ID
                  FROM dbo.Entrada e
                  WHERE e.Vendida = 1
                    AND e.DNIusuario = @DNI
                    AND NOT EXISTS (
                        SELECT 1 FROM dbo.Transaccion t WHERE t.IDentrada = e.ID
                    )",
                new { DNI = dni });
        }
    }

    public List<Entrada> GetEntradasCompradas(int dni)
{
    List<Entrada> entradasCompradas = new List<Entrada>();
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string storedProcedure = "GetEntradasCompradas";

        entradasCompradas = connection.Query<Entrada>(
            storedProcedure,
            new { DNI = dni },
            commandType: CommandType.StoredProcedure
        ).ToList();

        foreach (Entrada entrada in entradasCompradas)
        {
            entrada.Evento = connection.QueryFirstOrDefault<Evento>(
                "SELECT * FROM dbo.Evento WHERE ID = @ID",
                new { ID = entrada.IDEvento });

            if (entrada.Evento != null)
            {
                var estadio = connection.QueryFirstOrDefault<Estadio>(
                    "SELECT * FROM dbo.Estadio WHERE ID = @ID",
                    new { ID = entrada.Evento.IDEstadio });

                entrada.Evento.Lugar = estadio?.NombreLugar ?? estadio?.Nombre ?? "";
            }
        }
    }
    return entradasCompradas;
}

    public List<Entrada> GetEntradasPublicadas(int dni)
    {
        List<Entrada> entradasPublicadas = new List<Entrada>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "GetEntradasPublicadas";

            entradasPublicadas = connection.Query<Entrada>(
                storedProcedure,
                new { DNI = dni },
                commandType: CommandType.StoredProcedure
            ).ToList();

            foreach (Entrada entrada in entradasPublicadas)
            {
                entrada.Evento = connection.QueryFirstOrDefault<Evento>(
                    "SELECT * FROM dbo.Evento WHERE ID = @ID",
                    new { ID = entrada.IDEvento });

                if (entrada.Evento != null)
                {
                    var estadio = connection.QueryFirstOrDefault<Estadio>(
                        "SELECT * FROM dbo.Estadio WHERE ID = @ID",
                        new { ID = entrada.Evento.IDEstadio });

                    entrada.Evento.Lugar = estadio?.NombreLugar ?? estadio?.Nombre ?? "";
                }
            }
        }
        return entradasPublicadas;
    }

    public int GetCantidadEntradasPorEvento(int idEvento)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "GetCantidadEntradasPorEvento";

            return connection.QuerySingleOrDefault<int>(
                storedProcedure,
                new { IDEvento = idEvento },
                commandType: CommandType.StoredProcedure
            );
        }
    }

    public decimal GetPrecioMaximoEntrada()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT MAX(Precio) FROM dbo.Entrada WHERE Vendida = 0";
            return connection.ExecuteScalar<decimal?>(query) ?? 0m;
        }
    }

    public List<Entrada> GetProximosEventosComprados(int dni)
    {
        List<Entrada> entradas = new List<Entrada>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT e.*
                FROM dbo.Entrada e
                INNER JOIN dbo.Evento ev ON ev.ID = e.IDEvento
                WHERE e.DNIusuario = @DNI
                  AND e.Vendida = 1
                  AND ev.Fecha >= @Hoy
                ORDER BY ev.Fecha ASC";

            entradas = connection.Query<Entrada>(query, new { DNI = dni, Hoy = DateTime.Now }).ToList();

            foreach (Entrada entrada in entradas)
            {
                entrada.Evento = connection.QueryFirstOrDefault<Evento>(
                    "SELECT * FROM dbo.Evento WHERE ID = @ID",
                    new { ID = entrada.IDEvento });

                if (entrada.Evento != null)
                {
                    var estadio = connection.QueryFirstOrDefault<Estadio>(
                        "SELECT * FROM dbo.Estadio WHERE ID = @ID",
                        new { ID = entrada.Evento.IDEstadio });

                    entrada.Evento.Lugar = estadio?.NombreLugar ?? estadio?.Nombre ?? "";
                }
            }
        }

        return entradas;
    }

    public void ActualizarUbicacionEntrada(int idUbicacion, int idEntrada)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE dbo.Entrada SET IDubicacion = @IDUbicacion WHERE ID = @IDEntrada";
            connection.Execute(query, new { IDEntrada = idEntrada, IDUbicacion = idUbicacion });
        }
    }

    public int InsertarUbicacion(string nombreSector, int? fila, int? asiento)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"INSERT INTO dbo.Ubicacion (NombreSector, Fila, Asiento)
                             VALUES (@NombreSector, @Fila, @Asiento);
                             SELECT CAST(SCOPE_IDENTITY() AS int);";

            return connection.ExecuteScalar<int>(query, new
            {
                NombreSector = nombreSector,
                Fila = fila,
                Asiento = asiento
            });
        }
    }

    public List<Estadio> GetEstadios() {
        List<Estadio> estadios = new List<Estadio>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {
            string query = "SELECT * FROM dbo.Estadio";
            estadios = connection.Query<Estadio>(query).ToList();
        }
        return estadios;
    }

    public Provincia? GetProvinciaPorId(int id)
    {
        if (id <= 0)
            return null;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM dbo.Provincia WHERE ID = @ID";
            return connection.QueryFirstOrDefault<Provincia>(query, new { ID = id });
        }
    }

    public List<string> GetCategoriasDisponibles()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.Query<string>(
                "SELECT DISTINCT Nombre FROM dbo.Categoria ORDER BY Nombre"
            ).ToList();
        }
    }

    public bool ExisteUbicacionDuplicada(int idEvento, string nombreSector, int? fila, int? asiento)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"SELECT COUNT(1)
                             FROM dbo.Entrada e
                             INNER JOIN dbo.Ubicacion u ON u.ID = e.IDubicacion
                             INNER JOIN dbo.Evento ev ON ev.ID = e.IDevento
                             WHERE e.Vendida = 0
                               AND ev.IDestadio = (SELECT IDEstadio FROM dbo.Evento WHERE ID = @IDEvento)
                               AND CAST(ev.Fecha AS date) = (SELECT CAST(Fecha AS date) FROM dbo.Evento WHERE ID = @IDEvento)
                               AND u.NombreSector = @NombreSector
                               AND u.Fila = @Fila
                               AND u.Asiento = @Asiento;";

            return connection.ExecuteScalar<int>(query, new
            {
                IDEvento = idEvento,
                NombreSector = nombreSector,
                Fila = fila,
                Asiento = asiento
            }) > 0;
        }
    }

    public List<HistorialTransaccion> GetHistorialTransacciones(int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT t.ID,
                       t.Fecha,
                       t.MontoTotal,
                       t.Comision,
                       t.Estado,
                       t.IDentrada AS IDEntrada,
                       'Compra' AS Tipo,
                       uv.NombreUsuario AS Contraparte,
                       ev.Nombre AS NombreEvento,
                       ev.Protagonista,
                       ev.Fecha AS FechaEvento,
                       ev.Imagen,
                       es.NombreLugar AS Lugar,
                       CAST(1 AS bit) AS Vendida
                FROM dbo.Transaccion t
                INNER JOIN dbo.Vendedor v ON v.ID = t.IDvendedor
                INNER JOIN dbo.Usuario uv ON uv.DNI = v.DNIusuario
                INNER JOIN dbo.Entrada e ON e.ID = t.IDentrada
                INNER JOIN dbo.Evento ev ON ev.ID = e.IDevento
                LEFT JOIN dbo.Estadio es ON es.ID = ev.IDestadio
                WHERE t.DNIusuario = @DNI

                UNION ALL

                SELECT e.ID AS ID,
                       COALESCE(t.Fecha, '19000101') AS Fecha,
                       e.Precio AS MontoTotal,
                       CAST(0 AS decimal(18,2)) AS Comision,
                       CASE WHEN e.Vendida = 1 THEN 'Vendida' ELSE 'Publicada' END AS Estado,
                       e.ID AS IDEntrada,
                       'Publicacion' AS Tipo,
                       CASE WHEN e.Vendida = 1 THEN ub.NombreUsuario ELSE '' END AS Contraparte,
                       ev.Nombre AS NombreEvento,
                       ev.Protagonista,
                       ev.Fecha AS FechaEvento,
                       ev.Imagen,
                       es.NombreLugar AS Lugar,
                       e.Vendida AS Vendida
                FROM dbo.Entrada e
                INNER JOIN dbo.Vendedor v ON v.ID = e.IDvendedor
                LEFT JOIN dbo.Transaccion t ON t.IDentrada = e.ID
                LEFT JOIN dbo.Usuario ub ON ub.DNI = e.DNIusuario
                INNER JOIN dbo.Evento ev ON ev.ID = e.IDevento
                LEFT JOIN dbo.Estadio es ON es.ID = ev.IDestadio
                WHERE v.DNIusuario = @DNI

                ORDER BY Fecha DESC, ID DESC";

        return connection.Query<HistorialTransaccion>(query, new { DNI = dni }).ToList();
    }
    }

    public bool ExisteNombreUsuarioExceptoDNI(string nombreUsuario, int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM [dbo].[Usuario] WHERE [NombreUsuario] = @NombreUsuario AND [DNI] <> @DNI";
            return connection.ExecuteScalar<int>(query, new { NombreUsuario = nombreUsuario, DNI = dni }) > 0;
        }
    }

    public bool ExisteMailExceptoDNI(string mail, int dni)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM [dbo].[Usuario] WHERE [Mail] = @Mail AND [DNI] <> @DNI";
            return connection.ExecuteScalar<int>(query, new { Mail = mail, DNI = dni }) > 0;
        }
    }

    public void ActualizarPerfil(int dni, string nombre, string apellido, string nombreUsuario, DateTime fechaNacimiento, string ciudad, string mail)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"UPDATE [dbo].[Usuario]
                            SET [Nombre] = @Nombre,
                                [Apellido] = @Apellido,
                                [NombreUsuario] = @NombreUsuario,
                                [FechaNacimiento] = @FechaNacimiento,
                                [Ciudad] = @Ciudad,
                                [Mail] = @Mail
                            WHERE [DNI] = @DNI";

            connection.Execute(query, new
            {
                DNI = dni,
                Nombre = nombre,
                Apellido = apellido,
                NombreUsuario = nombreUsuario,
                FechaNacimiento = fechaNacimiento,
                Ciudad = ciudad,
                Mail = mail
            });
        }
    }

    public bool ExisteCalificacion(int idVendedor, int dniUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"SELECT COUNT(*)
                             FROM dbo.VendedorXComentario vxc
                             INNER JOIN dbo.Comentario c ON c.ID = vxc.IDcomentario
                             WHERE vxc.IDvendedor = @IDVendedor
                               AND c.DNIusuario = @DNIUsuario";

            int cantidad = connection.ExecuteScalar<int>(query, new
            {
                IDVendedor = idVendedor,
                DNIUsuario = dniUsuario
            });

            return cantidad > 0;
        }
    }

    public void GuardarCalificacion(int idVendedor, int dniUsuario, decimal calificacion, string comentario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                int idComentario = connection.ExecuteScalar<int>(
                    @"INSERT INTO dbo.Comentario (Calificacion, Comentario, DNIusuario)
                      VALUES (@Calificacion, @Comentario, @DNIUsuario);
                      SELECT CAST(SCOPE_IDENTITY() AS int);",
                    new
                    {
                        Calificacion = calificacion,
                        Comentario = comentario,
                        DNIUsuario = dniUsuario
                    },
                    transaction);

                connection.Execute(
                    @"INSERT INTO dbo.VendedorXComentario (IDvendedor, IDcomentario)
                      VALUES (@IDVendedor, @IDComentario)",
                    new
                    {
                        IDVendedor = idVendedor,
                        IDComentario = idComentario
                    },
                    transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public decimal GetPromedioCalificacion(int idVendedor)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"SELECT AVG(CAST(c.Calificacion AS decimal(10, 2)))
                             FROM dbo.VendedorXComentario vxc
                             INNER JOIN dbo.Comentario c ON c.ID = vxc.IDcomentario
                             WHERE vxc.IDvendedor = @IDVendedor";

            decimal? promedio = connection.ExecuteScalar<decimal?>(query, new
            {
                IDVendedor = idVendedor
            });

            return promedio ?? 0m;
        }
    }

    public int GetCantidadCalificaciones(int idVendedor)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"SELECT COUNT(*)
                             FROM dbo.VendedorXComentario vxc
                             WHERE vxc.IDvendedor = @IDVendedor";

            return connection.ExecuteScalar<int>(query, new
            {
                IDVendedor = idVendedor
            });
        }
    }
}

