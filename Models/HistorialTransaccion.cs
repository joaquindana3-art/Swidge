public class HistorialTransaccion
{
    public int ID { get; set; }
    public DateTime Fecha { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal Comision { get; set; }
    public string Estado { get; set; } = "";
    public int IDEntrada { get; set; }
    public string Tipo { get; set; } = "";
    public string Contraparte { get; set; } = "";
    public string NombreEvento { get; set; } = "";
    public string Protagonista { get; set; } = "";
    public DateTime FechaEvento { get; set; }
    public string? Imagen { get; set; }
    public string? Lugar { get; set; }
    public bool Vendida { get; set; }
}