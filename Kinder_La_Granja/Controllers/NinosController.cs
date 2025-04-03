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

public class NinosController : Controller
{
    private readonly INinos _ninoService;
    private readonly INivel _nivelService;

    public NinosController(INinos ninoService, INivel nivelService)
    {
        _ninoService = ninoService;
        _nivelService = nivelService;
    }

    public async Task<IActionResult> Index(string nivelId)
    {
        var niveles = await _nivelService.GetAllAsync();
        ViewBag.NivelId = nivelId;
        ViewBag.Niveles = new SelectList(niveles, "id", "nombre", nivelId);

        List<Ninos> resultado;

        if (!string.IsNullOrEmpty(nivelId) && int.TryParse(nivelId, out int id))
        {
            var filtrados = await _ninoService.GetAllAsync();
            resultado = filtrados.Where(n => n.nivel == id).ToList();
        }
        else
        {
            resultado = await _ninoService.GetAllAsync();
        }

        return View(resultado ?? new List<Ninos>());
    }

    public async Task<IActionResult> Create()
    {
        var niveles = await _nivelService.GetAllAsync();
        ViewBag.Niveles = new SelectList(niveles, "id", "nombre");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Ninos nino)
    {
       if(nino.Referencias == null)
        {
            nino.Referencias = new List<Referencia>();
        }

        if (!ModelState.IsValid)
        {
            var niveles = await _nivelService.GetAllAsync();
            ViewBag.Niveles = new SelectList(niveles, "id", "nombre");
            return View(nino);
        }

        await _ninoService.CreateAsync(nino);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Detalle(string id)
    {
        var objectId = ObjectId.Parse(id);
        var nino = await _ninoService.GetByIdAsync(objectId);
        return View(nino);
    }
}