namespace Swidge.Models;
using Microsoft.Data.SqlClient;
using Dapper;
public class BD
{
    
    private string _connectionString = @"Server=localhost;Database=Swidge;Integrated Security=True;TrustServerCertificate=True;";

    public List<Usuario> GetUsuarios() {

        List<Usuario> usuarios = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Usuarios";
            usuarios = connection.Query<Usuario>(query).ToList();

        }
        return usuarios;

    }
    public List<Usuario> GetVendedores() {

        List<Usuario> vendedores = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Usuarios";
            vendedores = connection.Query<Usuario>(query).ToList();

        }
        return vendedores;

    }

    public Usuario? ObtenerUsuarioParaLogin(string usuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT TOP 1 *
                FROM [dbo].[Usuario]
                WHERE [NombreUsuario] = @Usuario OR [Mail] = @Usuario";

            return connection.QueryFirstOrDefault<Usuario>(query, new { Usuario = usuario });
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
