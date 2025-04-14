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
    private readonly ICondiciones_Medicas _icontext;
    private readonly ITareas _tareasService;

    public NinosController(INinos ninoService, INivel nivelService, ICondiciones_Medicas icontext,
        ITareas tareasService)
    {
        _ninoService = ninoService;
        _nivelService = nivelService;
        _icontext = icontext;
        _tareasService = tareasService;
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

        var condicionesMedicas = _icontext.GetAllCondiciones_Medicas();
        ViewBag.CondicionesMedicas = new SelectList(condicionesMedicas, "Id", "nombre");


        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Ninos nino, List<string> selectedCondicionesMedicas)
    {
        if (nino.Referencias == null)
        {
            nino.Referencias = new List<Referencia>();
        }

        //if (!ModelState.IsValid)
        //{
        //    var niveles = await _nivelService.GetAllAsync();
        //    ViewBag.Niveles = new SelectList(niveles, "id", "nombre");
        //    var condicionesMedicas = _icontext.GetAllCondiciones_Medicas();
        //    ViewBag.CondicionesMedicas = new SelectList(condicionesMedicas, "Id", "nombre");
        //    return View(nino);
        //}

        nino.CondicionesMedicas = selectedCondicionesMedicas
            .Select(id => new ObjectId(id))
            .ToList();

        await _ninoService.CreateAsync(nino);
        return RedirectToAction("Index");
    }


    //public async Task<IActionResult> Detalle(string id)
    //{
    //    var objectId = ObjectId.Parse(id);
    //    var nino = await _ninoService.GetByIdAsync(objectId);
    //    return View(nino);
    //}


    public async Task<IActionResult> Detalle(string id)
    {
        var objectId = ObjectId.Parse(id);
        var nino = await _ninoService.GetByIdAsync(objectId);

        if (nino == null)
        {
            return NotFound();
        }

        // Traer todas las condiciones médicas
        var todasLasCondiciones = _icontext.GetAllCondiciones_Medicas();

        // Filtrar las condiciones asociadas al niño
        if (nino.CondicionesMedicas != null && nino.CondicionesMedicas.Count > 0)
        {
            nino.CondicionesMedicasDetalles = todasLasCondiciones
                .Where(c => nino.CondicionesMedicas.Contains(c.Id))
                .ToList();
        }

        return View(nino);
    }

    [HttpGet]
    public async Task<IActionResult> VerTareas(string id)
    {
        var objectId = ObjectId.Parse(id);
        var nino = await _ninoService.GetByIdAsync(objectId);

        if (nino == null || nino.tareas == null || nino.tareas.Count == 0)
        {
            ViewBag.NinoId = id;
            return View(new List<Tareas>());
        }

        var tareas = await _tareasService.GetTareasFromIdsAsync(nino.tareas);
        ViewBag.NinoId = id;
        return View(tareas);
    }
}