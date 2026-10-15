namespace Swidge.Models;
public class Transaccion
{
 
    public int ID { get; set; }
    public int DNIcomprador { get; set; }
    public float MontoTotal { get; set; }
    public float Comision { get; set; }
    public int IDvendedor { get; set;}
    public int IDentrada { get; set;}
    public string Estado { get; set; }
    public DateTime Fecha { get; set; }
 
}