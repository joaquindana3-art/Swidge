using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Swidge.Models;

namespace Swidge.Controllers;

public class SessionController : Controller
{
    BD bd = new BD();
    private static readonly string _hashFalso = BCrypt.Net.BCrypt.HashPassword("contrasena-falsa"); 

    [HttpPost]
    public IActionResult IniciarSesion(string nombreUsuario, string contrasena)
    {
        if (HaySesion()) {

            return RedirectToAction("Index", "Home");

        }
        Usuario encontrado = bd.ObtenerUsuarioParaLogin(nombreUsuario, contrasena);
        if (encontrado == null)
        {
            // Se hace un hash falso para que el tiempo de respuesta sea similar al de un login válido.
            BCrypt.Net.BCrypt.Verify(contrasena, _hashFalso);
            TempData["ErrorMessage"] = "Usuario o contraseña incorrectos.";
            return RedirectToAction("IniciarSesion", "Home");
        }
        GuardarSesion(encontrado);
        return RedirectToAction("Index", "Home");

    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    public IActionResult Registrarse(int dni, string nombre, string apellido, string nombreUsuario, DateTime fechaNacimiento, string ciudad, string mail, string contrasena)
    {
        if (HaySesion()) {
            return RedirectToAction("Index", "Home");
        }

        Usuario datos = new Usuario
        {
            DNI = dni,
            Nombre = nombre,
            Apellido = apellido,
            NombreUsuario = nombreUsuario,
            FechaNacimiento = fechaNacimiento,
            Ciudad = ciudad,
            Mail = mail,
            Contraseña = contrasena
        };

        List<string> errores = new List<string>();
        if (bd.ExisteDNI(datos.DNI)) errores.Add("Ya existe una cuenta con ese DNI.");
        if (bd.ExisteNombreUsuario(datos.NombreUsuario)) errores.Add("El nombre de usuario ya está en uso. Elegí otro.");
        if (bd.ExisteMail(datos.Mail)) errores.Add("Ya existe una cuenta con ese mail.");
        if (errores.Count > 0)
        {
            datos.Contraseña = "";
            ViewBag.Errores = errores;
            return View(datos);
        }
        bd.AgregarUsuario(datos);
        GuardarSesion(datos);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult MiPerfil()
    {
        int? dni = HttpContext.Session.GetInt32("DNI");
        if (dni == null)
        {
            return RedirectToAction("IniciarSesion", "Home");
        }

        Usuario? usuario = bd.ObtenerUsuarioPorDNI(dni.Value);
        if (usuario == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("IniciarSesion", "Home");
        }

        usuario.Contraseña = "";
        return View(usuario);
    }

    public IActionResult CambiarContraseña()
    {
        if (!HaySesion()) return RedirectToAction(nameof(IniciarSesion));
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarContraseña(string? contrasenaActual, string? contrasenaNueva)
    {
        int? dni = HttpContext.Session.GetInt32("DNI");
        if (dni == null) return RedirectToAction(nameof(IniciarSesion));

        Usuario? usuario = bd.ObtenerUsuarioPorDNI(dni.Value);
        if (usuario == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(IniciarSesion));
        }

        // Lo único que se valida acá es que la contraseña actual sea la correcta.
        if (!VerificarContrasena(contrasenaActual ?? "", usuario.Contraseña))
        {
            ViewBag.Errores = new List<string> { "La contraseña actual es incorrecta." };
            return View();
        }

        bd.CambiarContraseña(dni.Value, BCrypt.Net.BCrypt.HashPassword(contrasenaNueva ?? ""));
        TempData["Mensaje"] = "Tu contraseña se cambió correctamente.";
        return RedirectToAction(nameof(MiPerfil));
    }

    public IActionResult OlvideMiContraseña()
    {
        return View();
    }

    public bool HaySesion()
    {
        if (HttpContext.Session.GetInt32("DNI") == null)
        {
            return false;
        }
        return true;
    }

    private void GuardarSesion(Usuario usuario)
    {
        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32("DNI", usuario.DNI);
        HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
        HttpContext.Session.SetString("Nombre", usuario.Nombre);
        HttpContext.Session.SetString("Apellido", usuario.Apellido);
    }

    private static bool VerificarContrasena(string contrasena, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // El valor guardado no es un hash BCrypt válido (por ejemplo, una clave vieja en texto plano).
            return false;
        }
    }
}