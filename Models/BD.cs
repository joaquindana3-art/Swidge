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

            string query = "SELECT * FROM Comentario";
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

            foreach (var entrada in entradas)
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
    public Usuario? ObtenerUsuarioParaLogin(string nombreUsuario, string contrasena)
    {

        Usuario usuarioParaLogin = new Usuario();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {

            string query = @"SELECT TOP 1 * FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contraseña";
            usuarioParaLogin = connection.QueryFirstOrDefault<Usuario>(query, new { NombreUsuario = nombreUsuario, Contraseña = contrasena });

        }
        return usuarioParaLogin;
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
            return connection.QueryFirstOrDefault<Usuario>(query, new { DNI = dni });
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

            return entrada;
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
            connection.Execute(
                "dbo.MarcarEntradaVendida",
                new { IDEntrada = idEntrada, DNIusuario = dniUsuario },
                commandType: CommandType.StoredProcedure);
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

        foreach (var entrada in entradasCompradas)
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

            foreach (var entrada in entradasPublicadas)
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

            foreach (var entrada in entradas)
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

    public int InsertarUbicacion(int idEvento, string nombreSector, int? fila, int? asiento)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string storedProcedure = "InsertarUbicacion";

            return connection.ExecuteScalar<int>(storedProcedure, new
            {
                IDEvento = idEvento,
                NombreSector = nombreSector,
                Fila = fila,
                Asiento = asiento
            }, commandType: CommandType.StoredProcedure);
        }
    }
}