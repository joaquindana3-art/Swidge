using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Swidge.Models;

namespace Swidge.Controllers;

public class SessionController : Controller
{
    BD bd = new BD();
    private static readonly string _hashFalso = BCrypt.Net.BCrypt.HashPassword("contrasena-falsa"); 

    public IActionResult IniciarSesion()
    {
        if (HaySesion()) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult IniciarSesion(Usuario? usuario, string? contrasena)
    {
        if (HaySesion()) return RedirectToAction("Index", "Home");

        string usuarioTexto = (usuario?.NombreUsuario ?? "").Trim();
        string mailTexto = (usuario?.Mail ?? "").Trim();
        contrasena ??= "";

        Usuario? encontrado = bd.ObtenerUsuarioParaLogin(usuarioTexto, mailTexto);
        bool correcta = VerificarContrasena(contrasena, encontrado?.Contraseña ?? _hashFalso);

        if (encontrado == null || !correcta)
        {
            ViewBag.ErrorMessage = "Usuario o contraseña incorrectos.";
            return View();
        }

        GuardarSesion(encontrado);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }


    public IActionResult Registrarse()
    {
        if (HaySesion()) return RedirectToAction("Index", "Home");
        return View();
    }

    public IActionResult Registrarse(int dni, string? nombre, string? apellido, string? nombreUsuario, DateTime fechaNacimiento, string? ciudad, string? mail, string? contrasena)
    {
        if (HaySesion()) return RedirectToAction("Index", "Home");

        Usuario datos = new Usuario
        {
            DNI = dni,
            Nombre = (nombre ?? "").Trim(),
            Apellido = (apellido ?? "").Trim(),
            NombreUsuario = (nombreUsuario ?? "").Trim(),
            FechaNacimiento = fechaNacimiento,
            Ciudad = (ciudad ?? "").Trim(),
            Mail = (mail ?? "").Trim().ToLowerInvariant()
        };

        // Lo único que se valida acá es lo que depende de la base: que no estén repetidos.
        List<string> errores = new List<string>();

        if (bd.ExisteDNI(datos.DNI)) errores.Add("Ya existe una cuenta con ese DNI.");
        if (bd.ExisteNombreUsuario(datos.NombreUsuario)) errores.Add("El nombre de usuario ya está en uso. Elegí otro.");
        if (bd.ExisteMail(datos.Mail)) errores.Add("Ya existe una cuenta con ese mail.");

        if (errores.Count == 0)
        {
            datos.Contraseña = BCrypt.Net.BCrypt.HashPassword(contrasena ?? "");
            try
            {
                bd.AgregarUsuario(datos);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Dos registros simultáneos con los mismos datos: la base lo frena por los UNIQUE / PK.
                errores.Add("Ya existe una cuenta con esos datos.");
            }
        }

        if (errores.Count > 0)
        {
            datos.Contraseña = "";
            ViewBag.Errores = errores;
            return View(datos);
        }

        GuardarSesion(datos);
        return RedirectToAction("Index", "Home");
    }


    public IActionResult MiPerfil()
    {
        int? dni = HttpContext.Session.GetInt32("DNI");
        if (dni == null) return RedirectToAction(nameof(IniciarSesion));

        Usuario? usuario = bd.ObtenerUsuarioPorDNI(dni.Value);
        if (usuario == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(IniciarSesion));
        }

        usuario.Contraseña = ""; // el hash nunca llega a la vista
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

    public IActionResult IniciarSesionGoogle()
    {
        return View();
    }

    public IActionResult IniciarSesionApple()
    {
        return View();
    }

    private bool HaySesion()
    {
        return HttpContext.Session.GetInt32("DNI") != null;
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