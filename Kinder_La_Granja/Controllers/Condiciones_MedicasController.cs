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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kinder_La_Granja.Controllers;

public class Condiciones_MedicasController : Controller
{
    private readonly ICondiciones_Medicas _icontext;
   

    public Condiciones_MedicasController(ICondiciones_Medicas icontext)
    {
        _icontext = icontext;
        
    }

    // Mostrar Usuarios
    public IActionResult Index()
    {
        return View(_icontext.GetAllCondiciones_Medicas());

    }



    [HttpGet]
    public async Task<IActionResult> create()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(Condiciones_Medicas Condiciones_Medicas)
    {


        _icontext.CreateCondiciones_Medicas(Condiciones_Medicas);

        //    return RedirectToAction("login", "Usuarios");
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Editar(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var Condiciones_Medicas = _icontext.GetCondiciones_MedicasById(id);

        if (Condiciones_Medicas == null)
            return NotFound();

        return View(Condiciones_Medicas);
    }

    // POST: Guardar cambios de edición
    [HttpPost]
    public IActionResult Editar(Condiciones_Medicas Condiciones_Medicas)
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

            return View(Condiciones_Medicas);
        }

        try
        {
            Console.WriteLine($"🟢 Editando usuario con ID: {Condiciones_Medicas.Id}");
            _icontext.UpdateCondiciones_Medicas(Condiciones_Medicas.Id.ToString(), Condiciones_Medicas);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en Editar: {ex.Message}");
            ModelState.AddModelError("", "Error al actualizar el usuario.");
            return View(Condiciones_Medicas);
        }
    }




    // GET: Eliminar Usuario (Confirmación)
    public IActionResult Eliminar(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var Condiciones_Medicas = _icontext.GetCondiciones_MedicasById(id);
        if (Condiciones_Medicas == null)
            return NotFound();

        return View(Condiciones_Medicas);
    }

    //// POST: Confirmar eliminación
    [HttpPost]
    public IActionResult ConfirmarEliminar(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        try
        {
            _icontext.DeleteCondiciones_Medicas(id);
            return RedirectToAction("Index"); // Redirigir después de eliminar
        }
        catch
        {
            ModelState.AddModelError("", "Error al eliminar el usuario.");
            return View("Eliminar", _icontext.GetCondiciones_MedicasById(id));
        }
    }




}




