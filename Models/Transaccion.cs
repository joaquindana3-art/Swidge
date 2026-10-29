namespace Swidge.Models;

public class Transaccion
{
    public int ID { get; set; }
    public int DNIusuario { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal Comision { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "";
    public int IDvendedor { get; set; }
    public int IDentrada { get; set; }
}