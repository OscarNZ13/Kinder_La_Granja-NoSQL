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
    
    // GET: Editar
    public async Task<IActionResult> Editar(string id)
    {
        var objectId = ObjectId.Parse(id);
        var nino = await _ninoService.GetByIdAsync(objectId);

        if (nino == null)
            return NotFound();

        var niveles = await _nivelService.GetAllAsync();
        ViewBag.Niveles = new SelectList(niveles, "id", "nombre", nino.nivel);

        // 🔽 Asegurarse que no sea null antes de asignar al MultiSelectList
        if (nino.CondicionesMedicas == null)
        {
            nino.CondicionesMedicas = new List<ObjectId>();
        }

        var condicionesMedicas = _icontext.GetAllCondiciones_Medicas();

        // 🔽 Cargar las condiciones médicas con las seleccionadas marcadas
        ViewBag.CondicionesMedicas = new MultiSelectList(condicionesMedicas, "Id", "nombre", nino.CondicionesMedicas);

        return View(nino);
    }


    [HttpPost]
    public async Task<IActionResult> Editar(Ninos nino, List<string> selectedCondicionesMedicas, string removeCondicionId)
    {
        var actual = await _ninoService.GetByIdAsync(nino._id);
        if (actual == null)
            return NotFound();
        
        // ✅ Evitar error si la propiedad es null
        if (actual.CondicionesMedicas == null)
            actual.CondicionesMedicas = new List<ObjectId>();
        
        if (actual.Referencias == null)
            actual.Referencias = new List<Referencia>();


        // Si se quiere eliminar una condición
        if (!string.IsNullOrEmpty(removeCondicionId))
        {
            var idEliminar = new ObjectId(removeCondicionId);
            actual.CondicionesMedicas.Remove(idEliminar);

            await _ninoService.UpdateAsync(actual._id.ToString(), actual);
            return RedirectToAction("Editar", new { id = actual._id.ToString() });
        }

        // Agregar nuevas condiciones médicas (evita duplicados)
        if (selectedCondicionesMedicas != null)
        {
            foreach (var id in selectedCondicionesMedicas)
            {
                var objId = new ObjectId(id);
                if (!actual.CondicionesMedicas.Contains(objId))
                    actual.CondicionesMedicas.Add(objId);
            }
        }

        // Actualizar otros campos editables
        actual.nombre = nino.nombre;
        actual.direccion = nino.direccion;
        actual.poliza = nino.poliza;
        actual.nivel = nino.nivel;

        await _ninoService.UpdateAsync(actual._id.ToString(), actual);
        return RedirectToAction("Index");
    }


    // GET: Eliminar
    public async Task<IActionResult> Eliminar(string id)
    {
        var objectId = ObjectId.Parse(id);
        var nino = await _ninoService.GetByIdAsync(objectId);

        if (nino == null)
            return NotFound();

        return View(nino);
    }

    [HttpPost, ActionName("Eliminar")]
    public async Task<IActionResult> ConfirmarEliminar(string id)
    {
        var objectId = ObjectId.Parse(id);
        await _ninoService.DeleteAsync(objectId); // Implementar este método
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> VerTareas(string id)
    {
        var objectId = ObjectId.Parse(id);
        var nino = await _ninoService.GetByIdAsync(objectId);

        if (nino == null)
        {
            return NotFound();
        }

        ViewBag.NinoId = id;

        // Obtener todas las tareas
        var todasLasTareas = await _tareasService.GetAllAsync();

        // Filtrar las tareas ya asignadas al niño
        var tareasAsignadas = await _tareasService.GetTareasFromIdsAsync(nino.tareas);

        // Eliminar las tareas asignadas de la lista de tareas disponibles
        var tareasDisponibles = todasLasTareas
            .Where(t => !nino.tareas.Contains(t._id)) // Filtra las tareas que ya están asignadas
            .ToList();

        ViewBag.TodasLasTareas = tareasDisponibles;

        return View(tareasAsignadas);
    }
    
    [HttpPost]
    public async Task<IActionResult> AsignarTareas(string ninoId, List<string> selectedTareas)
    {
        var objectIdNino = ObjectId.Parse(ninoId);
        var nino = await _ninoService.GetByIdAsync(objectIdNino);

        if (nino == null)
        {
            return NotFound();
        }

        // Asignar las tareas seleccionadas al niño
        if (selectedTareas != null)
        {
            var tareasIds = selectedTareas.Select(t => ObjectId.Parse(t)).ToList();
            nino.tareas.AddRange(tareasIds);

            // Actualizar el niño con las nuevas tareas
            await _ninoService.UpdateAsync(ninoId, nino);
        }

        return RedirectToAction("VerTareas", new { id = ninoId });
    }

    [HttpPost]
    public async Task<IActionResult> EliminarTareaAsignada(string ninoId, string tareaId)
    {
        var objectIdNino = ObjectId.Parse(ninoId);
        var objectIdTarea = ObjectId.Parse(tareaId);

        var nino = await _ninoService.GetByIdAsync(objectIdNino);

        if (nino == null)
        {
            return NotFound();
        }

        // Eliminar la tarea de la lista de tareas asignadas al niño
        if (nino.tareas.Contains(objectIdTarea))
        {
            nino.tareas.Remove(objectIdTarea);

            // Actualizar al niño en la base de datos
            await _ninoService.UpdateAsync(ninoId, nino);
        }

        return RedirectToAction("VerTareas", new { id = ninoId });
    }
}