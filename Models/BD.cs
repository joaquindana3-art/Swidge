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
        List<Entrada> entradas = new();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            entradas = connection.Query<Entrada>("SELECT * FROM Entrada WHERE Vendida = 0").ToList();

            foreach (var entrada in entradas)
            {
                entrada.Evento = connection.QueryFirstOrDefault<Evento>(
                    "SELECT * FROM Evento WHERE ID = @ID",
                    new { ID = entrada.IDEvento });

                if (entrada.Evento != null)
                {
                    entrada.Evento.Lugar = connection.QueryFirstOrDefault<string>(
                        "SELECT TOP 1 NombreLugar FROM Ubicacion WHERE IDevento = @IDevento",
                        new { IDevento = entrada.IDEvento }) ?? "";
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

            string query = "SELECT * FROM Ubicaciones";
            ubicaciones = connection.Query<Ubicacion>(query).ToList();

        }
        return ubicaciones;

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

    public int InsertarUsuario(Usuario usuario)
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

}
