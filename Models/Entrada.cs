namespace Swidge.Models;
public class Entrada
{
 
    public int ID { get; set; }
    public Evento Evento { get; set; } = null!;
    public Usuario Vendedor { get; set; } = null!;
    public Usuario? Comprador { get; set; }
    public double Precio { get; set; }
    public string QR { get; set; } = "";
 
}
 