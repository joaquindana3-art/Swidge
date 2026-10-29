using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Swidge.Models;

namespace Swidge.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string ciudad = "Todos", string categoria = "Todos", decimal? precioMaximo = null, bool soloVerificados = false)
    {
        BD bd = new BD();
        List<Entrada> listaEntradas = bd.GetEntradas();
        decimal precioMaximoBD = bd.GetPrecioMaximoEntrada();

        if (precioMaximo.HasValue && precioMaximo.Value > precioMaximoBD)
        {
            precioMaximo = precioMaximoBD;
        }

        foreach (var entrada in listaEntradas)
        {
            var vendedor = bd.GetVendedores().FirstOrDefault(v => v.ID == entrada.IDVendedor);
            if (vendedor != null)
            {
                entrada.Vendedor = bd.GetUsuarios().FirstOrDefault(u => u.DNI == vendedor.DNIusuario);
            }
        }

        if (!string.IsNullOrWhiteSpace(ciudad) && ciudad != "Todos")
        {
            listaEntradas = listaEntradas
                .Where(e => e.Vendedor != null && !string.IsNullOrWhiteSpace(e.Vendedor.Ciudad) && e.Vendedor.Ciudad.Equals(ciudad, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(categoria) && categoria != "Todos")
        {
            listaEntradas = listaEntradas
                .Where(e => e.Evento != null && !string.IsNullOrWhiteSpace(e.Evento.Categoria) && e.Evento.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (precioMaximo.HasValue)
        {
            listaEntradas = listaEntradas
                .Where(e => e.Precio <= precioMaximo.Value)
                .ToList();
        }

        if (soloVerificados)
        {
            listaEntradas = listaEntradas
                .Where(e => e.Vendedor != null && !string.IsNullOrWhiteSpace(e.Vendedor.NumeroTarjeta))
                .ToList();
        }

        ViewBag.listaEntradas = listaEntradas;
        ViewBag.ciudadSeleccionada = ciudad;
        ViewBag.categoriaSeleccionada = categoria;
        ViewBag.precioMaximo = precioMaximo ?? precioMaximoBD;
        ViewBag.precioMaximoBD = precioMaximoBD;
        ViewBag.soloVerificados = soloVerificados;

        ViewBag.Ciudades = new List<string>
        {
            "Todos",
            "Buenos Aires",
            "Córdoba",
            "Rosario",
            "Mendoza",
            "CABA"
        };

        ViewBag.Categorias = new List<string>
        {
            "Todos",
            "Musica",
            "Teatro",
            "Deportes",
            "Festivales"
        };

        return View();
    }

    public IActionResult Ayuda()
    {
        return View();
    }

    public IActionResult CambiarContraseña()
    {
        return View();
    }

    public IActionResult _Layout() {

        SessionController SessionController = new SessionController();
        if (SessionController.HaySesion()) {
            ViewBag.nombreUsuario = HttpContext.Session.GetString("NombreUsuario");
        }
        return View();

    }
    public IActionResult ConfirmacionEntradaPublicada(Evento evento, string nombreTitular, string sector, int? fila, int? asiento, decimal precio, string imagen)
    {
        if (HttpContext.Session.GetInt32("DNI") == null)
        {
            return RedirectToAction("IniciarSesion", "Home");

        }
        else {

            BD bd = new BD();
            Usuario usuarioPublicador = bd.ObtenerUsuarioPorDNI(HttpContext.Session.GetInt32("DNI").Value);
            if (bd.BuscarVendedorPorDNI(usuarioPublicador.DNI) == null)
            {
                bd.InsertarVendedor(usuarioPublicador.DNI);
                bd.InsertarUbicacion(evento.id, sector, fila, asiento);
                bd.PublicarEntrada(usuarioPublicador.DNI, nombreTitular, sector, fila, asiento, precio, imagen);
                return View();
            }
            else
            {
                ViewBag.nombreTitular = nombreTitular;
                ViewBag.sector = sector;
                ViewBag.fila = fila;
                ViewBag.asiento = asiento;
                ViewBag.precio = precio;
                ViewBag.imagen = imagen;
            }


        }
        return View();
    }
    public IActionResult CompraExitosa()
    {
        return View();
    }

    public IActionResult DetalleEntrada(int id)
    {
        BD bd = new BD();
        var entrada = bd.GetEntradaPorId(id);

        if (entrada == null)
            return RedirectToAction("Index");

        return View(entrada);
    }

    public IActionResult EntradasCompradas()
    {
        BD bd = new BD();
        int? dniSesion = HttpContext.Session.GetInt32("DNI");

        if (dniSesion == null)
        {
            ViewBag.mensaje = "Registráte o iniciá sesión para poder comprar o vender entradas.";
            ViewBag.EntradasCompradas = new List<Entrada>();
            return View();
        }

        List<Entrada> entradasCompradas = bd.GetEntradasCompradas(dniSesion.Value);
        ViewBag.EntradasCompradas = entradasCompradas;

        ViewBag.mensaje = entradasCompradas.Count == 0
            ? "No tienes entradas compradas registradas."
            : "";

        return View();
    }

    public IActionResult EntradasPublicadas()
    {
        BD bd = new BD();
        int? dniSesion = HttpContext.Session.GetInt32("DNI");

        if (dniSesion == null) {
            ViewBag.mensaje = "Registráte o iniciá sesión para poder comprar o vender entradas.";
            ViewBag.EntradasPublicadas = new List<Entrada>();
            return View();
        }

        List<Entrada> entradasPublicadas = bd.GetEntradasPublicadas(dniSesion.Value);
        ViewBag.EntradasPublicadas = entradasPublicadas;

        ViewBag.mensaje = entradasPublicadas.Count == 0
            ? "No tienes entradas publicadas registradas."
            : "";

        return View();
    }
    
    public IActionResult InformacionVenta()
    {
        return View();
    }

    public IActionResult IniciarSesion()
    {

        int? dniSesion = HttpContext.Session.GetInt32("DNI");
        if (dniSesion != null) {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    public IActionResult IniciarSesionApple()
    {
        return View();
    }

    public IActionResult IniciarSesionGoogle()
    {
        return View();
    }

    public IActionResult MiPerfil()
    {
        int? dniSesion = HttpContext.Session.GetInt32("DNI");

        if (dniSesion == null)
        {
            return RedirectToAction("IniciarSesion", "Home");
        }

        BD bd = new BD();
        Usuario usuario = bd.ObtenerUsuarioPorDNI(dniSesion.Value);

        if (usuario == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("IniciarSesion", "Home");
        }

        usuario.Contraseña = "";
        ViewBag.Usuario = usuario;
        return View();
    }

    public IActionResult OlvideMiContraseña()
    {
        return View();
    }

    public IActionResult PagoEntrada()
    {
        return View();
    }

    public IActionResult PublicarEntrada()
    {
        BD bd = new BD();
        ViewBag.Eventos = bd.GetEventos();
        return View();
    }

    public IActionResult Registrarse()
    {

        int? dniSesion = HttpContext.Session.GetInt32("DNI");
        if (dniSesion != null) {
            return RedirectToAction("Index", "Home");
        }
        return View();

    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
