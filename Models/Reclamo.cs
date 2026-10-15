namespace Swidge.Models;
public class Reclamo
{
 
    public int ID { get; set; }
    public int DNIcomprador { get; set; }
    public int IDvendedor { get; set; }
    public int IDtransaccion { get; set; }
    public string Motivo { get; set; }
    public string Evidencia { get; set; }
    public string Estado { get; set; }
    public DateTime Fecha { get; set; }
 
}