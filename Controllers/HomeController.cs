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
        return View(listaEntradas);
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

    public IActionResult DetalleEntrada()
    {
        return View();
    }

    public IActionResult EntradasCompradas()
    {
        return View();
    }

    public IActionResult EntradasPublicadas()
    {
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
