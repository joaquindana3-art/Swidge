namespace Swidge.Models;

public class Ubicacion
{
    public int ID { get; set; }
    public int IDEvento { get; set; }
    public string NombreSector { get; set; } = "";
    public string NombreLugar { get; set; } = "";
    public int? Fila { get; set; }
    public int? Asiento { get; set; }
}