namespace Swidge.Models;
public class Entrada
{

    public int ID {get; set; };
    public Evento Evento {get; set; };
    public Usuario Vendedor {get; set; };
    public Usuario? Comprador {get; set; };
    public double Precio {get; set; };
    public string QR {get; set; };

}