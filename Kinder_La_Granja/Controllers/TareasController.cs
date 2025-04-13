using Kinder_La_Granja.Interface;
using Kinder_La_Granja.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kinder_La_Granja.Controllers
{
    public class TareasController : Controller
    {
      
            private readonly ITareas _tareaService;
            private readonly IUsuarios _usuarioService;

            public TareasController(ITareas tareaService, IUsuarios usuarioService)
            {
                _tareaService = tareaService;
                _usuarioService = usuarioService;
            }

            public IActionResult Index()
            {
                var tareas = _tareaService.GetAllAsync().Result;
                var docentes = _usuarioService.GetAllUsuarios()
                               .Where(u => u.IdRol == 2)
                               .ToDictionary(d => d.Id.ToString(), d => d.Nombre);

                ViewBag.Docentes = docentes;
                return View(tareas);
            }

            public IActionResult Detalle(string id)
            {
                var tarea = _tareaService.GetByIdAsync(id).Result;
                var docentes = _usuarioService.GetAllUsuarios();
                ViewBag.DocenteNombre = docentes.FirstOrDefault(d => d.Id == tarea.id_usuarioDocente)?.Nombre;
                return View(tarea);
            }

            public IActionResult Create()
            {
                var docentes = _usuarioService.GetAllUsuarios().Where(d => d.IdRol == 2).ToList();
                ViewBag.Usuarios = new SelectList(docentes, "Id", "Nombre");
                return View();
            }

            [HttpPost]
            public IActionResult Create(Tareas tarea)
            {
                var tareasExistentes = _tareaService.GetAllAsync().Result;

                int maxId = tareasExistentes.Any() ? tareasExistentes.Max(t => t.id_tarea) : 2;
                tarea.id_tarea = maxId + 1;

                if (!ModelState.IsValid)
                {
                    var docentes = _usuarioService.GetAllUsuarios().Where(d => d.IdRol == 2).ToList();
                    ViewBag.Usuarios = new SelectList(docentes, "Id", "Nombre", tarea.id_usuarioDocente.ToString());
                    return View(tarea);
                }

                _tareaService.CreateAsync(tarea).Wait();
                return RedirectToAction("Index");
            }

        public IActionResult Editar(string id)
            {
                var tarea = _tareaService.GetByIdAsync(id).Result;
                var docentes = _usuarioService.GetAllUsuarios().Where(d => d.IdRol == 2).ToList();
                ViewBag.Usuarios = new SelectList(docentes, "Id", "Nombre", tarea.id_usuarioDocente.ToString());
                return View(tarea);
            }

            [HttpPost]
            public IActionResult Editar(string id, Tareas tarea)
            {
                if (!ModelState.IsValid)
                {
                    var docentes = _usuarioService.GetAllUsuarios().Where(d => d.IdRol == 2).ToList();
                    ViewBag.Usuarios = new SelectList(docentes, "Id", "Nombre", tarea.id_usuarioDocente.ToString());
                    return View(tarea);
                }

                _tareaService.UpdateAsync(id, tarea).Wait();
                return RedirectToAction("Index");
            }
        [HttpGet]
        public IActionResult Eliminar(string id)
        {
            var tarea = _tareaService.GetByIdAsync(id).Result;
            var docentes = _usuarioService.GetAllUsuarios();
            ViewBag.DocenteNombre = docentes.FirstOrDefault(d => d.Id == tarea.id_usuarioDocente)?.Nombre;

            return View(tarea);
        }

        [HttpPost]
        public IActionResult Eliminar(Tareas tarea)
        {
            _tareaService.DeleteAsync(tarea._id.ToString()).Wait();
            return RedirectToAction("Index");
        }

    }

}