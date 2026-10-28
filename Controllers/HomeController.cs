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

    public IActionResult Index()
    {
        BD bd = new BD();
        List<Entrada> listaEntradas = bd.GetEntradas();
        ViewBag.listaEntradas = listaEntradas;
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
        bd.
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
