namespace Swidge.Models;
public class Comentario
{
 
    public int ID { get; set; }
    public int DNIComprador { get; set; }
    public int IDVendedor { get; set; }
    public int IDTransaccion { get; set; }
    public string Motivo { get; set; }
    public string Evidencia { get; set; }
    public string Estado { get; set; }
    public DateTime Fecha { get; set; }
 
}