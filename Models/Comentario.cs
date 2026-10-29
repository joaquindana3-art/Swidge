namespace Swidge.Models;

public class Comentario
{
    public int ID { get; set; }
    public decimal Calificacion { get; set; }
    public string ComentarioTexto { get; set; } = "";
}