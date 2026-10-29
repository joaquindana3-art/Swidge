namespace Swidge.Models;

public class Entrada
{
    public int ID { get; set; }
    public int IDEvento { get; set; }
    public int IDVendedor { get; set; }
    public int? DNIusuario { get; set; }
    public decimal Precio { get; set; }
    public string QR { get; set; } = "";
    public bool Vendida { get; set; }
    public Evento? Evento { get; set; }
    public Usuario? Vendedor { get; set; }
    public Usuario? Comprador { get; set; }
    public string? Imagen { get; set; }
    public int IDubicacion { get; set; }
    
}