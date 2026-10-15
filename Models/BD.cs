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
            usuarios = connection.query<Usuario>(query).ToList();

        }
        return usuarios;

    }
    public List<Usuario> GetVendedores() {

        List<Usuario> vendedores = new List<Usuario>();
        using (SqlConnection connection = new SqlConnection(_connectionString)) {

            string query = "SELECT * FROM Usuarios";
            usuarios = connection.query<Usuario>(query).ToList();

        }
        return usuarios;

    }

}
