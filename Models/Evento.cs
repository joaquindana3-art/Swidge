namespace Swidge.Models;
public class Evento
{

    public int ID {get; set; };
    public string Nombre {get; set; };
    public string Protagonista {get; set; };
    public string Categoria {get; set; };
    public string Descripcion {get; set; };
    public string Lugar {get; set; };
    public DateTime Fecha {get; set; };
    public string Imagen {get; set; };
    public int CantidadEntradas {get; set; };

}