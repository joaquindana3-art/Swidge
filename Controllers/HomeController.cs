using System.Diagnostics;
using System.Globalization;
using System.Text;
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

    private static string NormalizarCiudad(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var texto = valor.Trim();
        var normalizado = texto.Normalize(System.Text.NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalizado)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(System.Text.NormalizationForm.FormC).Trim();
    }

    public IActionResult Index(string ciudad = "Todos", string categoria = "Todos", decimal? precioMaximo = null)
    {
        BD bd = new BD();
        List<Entrada> listaEntradas = bd.GetEntradas();
        decimal precioMaximoBD = bd.GetPrecioMaximoEntrada();

        if (precioMaximo.HasValue && precioMaximo.Value > precioMaximoBD)
        {
            precioMaximo = precioMaximoBD;
        }

        if (!precioMaximo.HasValue)
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

        var ciudadesDisponibles = listaEntradas
            .Where(e => e.Vendedor != null && !string.IsNullOrWhiteSpace(e.Vendedor.Ciudad))
            .Select(e => e.Vendedor.Ciudad.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToList();

        ciudadesDisponibles.Insert(0, "Todos");

        if (!string.IsNullOrWhiteSpace(ciudad) && ciudad != "Todos")
        {
            listaEntradas = listaEntradas
                .Where(e => e.Vendedor != null &&
                    !string.IsNullOrWhiteSpace(e.Vendedor.Ciudad) &&
                    NormalizarCiudad(e.Vendedor.Ciudad).Equals(NormalizarCiudad(ciudad), StringComparison.OrdinalIgnoreCase))
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

        ViewBag.listaEntradas = listaEntradas;
        ViewBag.ciudadSeleccionada = ciudad;
        ViewBag.categoriaSeleccionada = categoria;
        ViewBag.precioMaximo = precioMaximo ?? precioMaximoBD;
        ViewBag.precioMaximoBD = precioMaximoBD;
        ViewBag.Ciudades = ciudadesDisponibles;

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
    public IActionResult ConfirmacionEntradaPublicada(int IDevento, string nombreTitular, string NombreSector, int? Fila, int? Asiento, decimal precioVenta, string comprobante)
    {

        BD bd = new BD();
        Usuario usuarioPublicador = bd.ObtenerUsuarioPorDNI(HttpContext.Session.GetInt32("DNI").Value);

        if (usuarioPublicador == null)
        {
            return RedirectToAction("IniciarSesion", "Home");
        }

        int idVendedor = 0;
        Vendedor vendedor = bd.BuscarVendedorPorDNIusuario(usuarioPublicador.DNI);
        if (vendedor == null)
        {
            idVendedor = bd.InsertarVendedor(usuarioPublicador.DNI);
        }
        else
        {
            idVendedor = vendedor.ID;
        }

        int idUbicacion = bd.InsertarUbicacion(IDevento, NombreSector, Fila, Asiento);

        string qr = !string.IsNullOrWhiteSpace(comprobante)
            ? comprobante
            : $"QR_{DateTime.Now:yyyyMMddHHmmss}_{IDevento}_{idVendedor}";

        int idEntrada = bd.PublicarEntrada(IDevento, idVendedor, precioVenta, qr, idUbicacion);
        ViewBag.Entrada = bd.BuscarEntradaPorId(idEntrada);
        ViewBag.Evento = bd.GetEventoPorId(IDevento);
        ViewBag.Ubicacion = bd.BuscarUbicacionPorId(idUbicacion);
        ViewBag.Vendedor = bd.BuscarVendedorPorID(idVendedor);
        return View();

    }
    public IActionResult PagarEntrada(int id)
    {
        BD bd = new BD();
        var entrada = bd.GetEntradaPorId(id);

        if (entrada == null)
            return RedirectToAction("Index", "Home");

        return View(entrada);
    }

    public IActionResult CompraExitosa(int id)
    {
        BD bd = new BD();
        int? dniSesion = HttpContext.Session.GetInt32("DNI");

        if (dniSesion == null)
        {
            return RedirectToAction("IniciarSesion", "Home");
        }

        Entrada? entrada = bd.GetEntradaPorId(id);
        if (entrada == null)
        {
            entrada = bd.BuscarEntradaPorId(id);
        }

        if (entrada == null)
            return RedirectToAction("Index", "Home");

        if (entrada.DNIusuario != dniSesion.Value)
        {
            bd.MarcarEntradaVendida(id, dniSesion.Value);
            entrada = bd.BuscarEntradaPorId(id);
        }

        if (entrada == null)
            return RedirectToAction("Index", "Home");

        ViewBag.Entrada = entrada;
        ViewBag.Evento = entrada.Evento;
        ViewBag.Ubicacion = entrada.Ubicacion;
        ViewBag.Vendedor = entrada.Vendedor;
        return View(entrada);
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

        var proximosEventos = bd.GetProximosEventosComprados(dniSesion.Value);
        ViewBag.ProximosEventos = proximosEventos;

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
        if (HttpContext.Session.GetInt32("DNI") == null)
        {
            return RedirectToAction("IniciarSesion", "Home");
        }
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
