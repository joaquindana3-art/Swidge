namespace Swidge.Models;
public class Entrada
{
    public int ID { get; set; }
    public int IDEvento { get; set; }
    public int IDVendedor { get; set; }
    public int? DNIComprador { get; set; }
    public double Precio { get; set; }
    public string QR { get; set; } = "";
    public bool Vendida { get; set; }
}
