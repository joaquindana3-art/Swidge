namespace Swidge.Models;
using Microsoft.Data.SqlClient;
using Dapper;
public class BD
{
    
    private string _connectionString = @"Server=localhost;Database=Swidge;Integrated Security=True;TrustServerCertificate=True;";

    public List<Usuario> GetUsuarios() {

        List<Usuario> usuarios = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Usuario";
            usuarios = connection.query<Usuario>(query).ToList();

        }
        return usuarios;

    }
    public List<Vendedor> GetVendedores() {

        List<Vendedor> vendedores = new List<Vendedor>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Vendedor";
            vendedores = connection.query<Vendedor>(query).ToList();

        }
        return vendedores;

    }

    public List<Comentario> GetComentarios() {

        List<Comentario> comentarios = new List<Comentario>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Comentario";
            comentarios = connection.query<Comentario>(query).ToList();

        }
        return comentarios;

    }

    public List<Entrada> GetEntradas() {

        List<Entrada> entradas = new List<Entrada>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Entrada";
            entradas = connection.query<Entrada>(query).ToList();

        }
        return entradas;

    }

    public List<Evento> GetEventos() {

        List<Evento> eventos = new List<Evento>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Evento";
            eventos = connection.query<Evento>(query).ToList();

        }
        return eventos;

    }

    public List<Reclamo> GetReclamos() {

        List<Reclamo> reclamos = new List<Reclamo>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Reclamo";
            reclamos = connection.query<Reclamo>(query).ToList();

        }
        return reclamos;

    }

    public List<Transaccion> GetTransacciones() {

        List<Transaccion> transacciones = new List<transacciones>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Transaccion";
            transacciones = connection.query<Transaccion>(query).ToList();

        }
        return transacciones;

    }

    public List<Ubicacion> GetUbicacion() {

        List<Ubicacion> ubicaciones = new List<Ubicacion>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Ubicaciones";
            ubicaciones = connection.query<Ubicacion>(query).ToList();

        }
        return ubicaciones;

    }

    public Usuario? ObtenerUsuarioParaLogin(string usuario)
    {

        Usuario usuarioParaLogin = new Usuario();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {

            string query = @"SELECT TOP 1 * FROM Usuario WHERE NombreUsuario = @NombreUsuario OR Mail = @Usuario";
            usuarioParaLogin = connection.QueryFirstOrDefault<Usuario>(query, new { NombreUsuario = usuario.Nombre });

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

    public void AgregarUsuario(Usuario usuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                INSERT INTO [dbo].[Usuario]
                ([DNI], [Nombre], [Apellido], [NombreUsuario], [FechaNacimiento], [Ciudad], [Contraseña], [Mail], [NumeroTarjeta], [CodigoSeguridad])
                VALUES
                (@DNI, @Nombre, @Apellido, @NombreUsuario, @FechaNacimiento, @Ciudad, @Contraseña, @Mail, NULL, NULL)";

            connection.Execute(query, usuario);
        }
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
