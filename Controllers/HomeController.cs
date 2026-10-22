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
        if (HttpContext.Session.GetInt32("DNI") != null)
        {
            ViewBag.EntradasCompradas = bd.GetEntradasCompradas(HttpContext.Session.GetInt32("DNI").Value);
        }
        else
        {
            ViewBag.EntradasCompradas = new List<Entrada>();
        }
        return View();
    }

    public IActionResult EntradasPublicadas()
    {
        BD bd = new BD();
        if (HttpContext.Session.GetInt32("DNI") != null) {
            ViewBag.EntradasPublicadas = bd.GetEntradasPublicadas(HttpContext.Session.GetInt32("DNI").Value);
        }
        else
        {
            ViewBag.EntradasPublicadas = new List<Entrada>();
        }
        return View();
    }
    
    public IActionResult InformacionVenta()
    {
        return View();
    }

    public IActionResult IniciarSesion()
    {
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

        return View();
    }

    public IActionResult Registrarse()
    {
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
