using Kinder_La_Granja.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Kinder_La_Granja.Models;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kinder_La_Granja.Controllers;

public class UsuariosController : Controller
{
    private readonly IUsuarios _icontext;
    private readonly PasswordHasher<Usuarios> _passwordHasher;

    public UsuariosController(IUsuarios icontext)
    {
        _icontext = icontext;
        _passwordHasher = new PasswordHasher<Usuarios>();
    }

    // Mostrar Usuarios
    public IActionResult Index()
    {
        return View(_icontext.GetAllUsuarios());

    }

    // GET: Editar Usuario
    public IActionResult Editar(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var usuario = _icontext.GetUsuarioById(id);
        
        if (usuario == null)
            return NotFound();

        return View(usuario);
    }

    // POST: Guardar cambios de edición
    [HttpPost]
    public IActionResult Editar(Usuarios usuario)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("❌ ModelState no válido. Errores:");

            foreach (var error in ModelState)
            {
                foreach (var subError in error.Value.Errors)
                {
                    Console.WriteLine($"⚠️ Campo: {error.Key} - Error: {subError.ErrorMessage}");
                }
            }

            return View(usuario);
        }

        try
        {
            Console.WriteLine($"🟢 Editando usuario con ID: {usuario.Id}");
            _icontext.UpdateUsuario(usuario.Id.ToString(), usuario);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en Editar: {ex.Message}");
            ModelState.AddModelError("", "Error al actualizar el usuario.");
            return View(usuario);
        }
    }


    // GET: Eliminar Usuario (Confirmación)
    public IActionResult Eliminar(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var usuario = _icontext.GetUsuarioById(id);
        if (usuario == null)
            return NotFound();

        return View(usuario);
    }

    // POST: Confirmar eliminación
    [HttpPost]
    public IActionResult ConfirmarEliminar(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        try
        {
            _icontext.DeleteUsuario(id);
            return RedirectToAction("Index"); // Redirigir después de eliminar
        }
        catch
        {
            ModelState.AddModelError("", "Error al eliminar el usuario.");
            return View("Eliminar", _icontext.GetUsuarioById(id));
        }
    }
    
    
    [HttpGet]
    public IActionResult login()
    {
        if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> login(string email, string password)
    {
        var buscarUsuario = _icontext.GetUsuarioByEmail(email);

        if (buscarUsuario == null)
        {
            ViewData["Mensaje"] = "Usuario no encontrado o inactivo";
            return View();
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(buscarUsuario,
            buscarUsuario.ContrasenaHash, password);

        if (verificationResult != PasswordVerificationResult.Success)
        {
            ViewData["Mensaje"] = "Contraseña incorrecta";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, buscarUsuario.Id.ToString()),
            new Claim(ClaimTypes.Name, buscarUsuario.Nombre),
            new Claim(ClaimTypes.Email, buscarUsuario.CorreoElectronico),
            new Claim(ClaimTypes.Role, buscarUsuario.IdRol.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties { AllowRefresh = true };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), properties);
        
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("login", "Usuarios");
    }

    [HttpGet]
    public async Task<IActionResult> register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> register(int cedula, string nombre, string correo, string password,
        int numTelefono, string direccion)
    {
        if (_icontext.GetUsuarioByEmail(correo) != null)
        {
            ViewData["Mensaje"] = "Correo ya registrado en el sistema";
            return View();
        }
        
        // Verificar cédula y teléfono
        if (cedula < 0 || numTelefono < 0 || cedula.ToString().Length < 9 || cedula.ToString().Length > 9 || numTelefono.ToString().Length <8 || numTelefono.ToString().Length > 8)
        {
            ViewData["Mensaje"] = "Verifique que ingresa una cédula o un número de teléfono válidos";
            return View();
        }

        // Verificar campos vacíos
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(direccion))
        {
            ViewData["Mensaje"] = "Verifique que el nombre, correo o dirección contengan valores válidos";
            return View();
        }

        // Verificar contraseña
        if (password.Length < 8)
        {
            ViewData["Mensaje"] = "La contraseña debe tener al menos 8 caracteres";
            return View();
        }

        // Expresión regular para verificar mayúsculas, minúsculas y números
        var passwordRegex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).+$");

        if (!passwordRegex.IsMatch(password))
        {
            ViewData["Mensaje"] =
                "La contraseña debe contener al menos una letra mayúscula, una minúscula y un número.";
            return View();
        }

        // Hasheamos la contrasena
        var ContrasenaHash = _passwordHasher.HashPassword(null, password);
        
        Usuarios nuevoUsuario = new Usuarios()
        {
            Cedula = cedula,
            Nombre = nombre,
            ContrasenaHash = ContrasenaHash,
            CorreoElectronico = correo,
            Direccion = direccion,
            NumTelefono = numTelefono,
            IdRol = 2
        };
        
        _icontext.CreateUsuario(nuevoUsuario);
        
        return RedirectToAction("login", "Usuarios");
    }
}