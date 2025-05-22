// Pages/Ubicaciones/Index.cshtml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Ubicaciones
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaContext _context;
        public IndexModel(BibliotecaContext context) => _context = context;

        // Para la barra de búsqueda
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        // Lista filtrada de ubicaciones
        public IList<Ubicacione> Ubicaciones { get; set; } = new List<Ubicacione>();

        // BindProperties para los formularios de modal
        [BindProperty] public Ubicacione CreateUbicacione { get; set; } = new Ubicacione();
        [BindProperty] public Ubicacione EditUbicacione { get; set; } = new Ubicacione();

        // Flags para reabrir modales tras validación
        public bool ShowCreateModal { get; set; } = false;
        public bool ShowEditModal { get; set; } = false;

        private bool IsAjaxRequest() =>
            Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // GET: carga inicial + búsqueda
        public async Task OnGetAsync()
        {
            var q = _context.Ubicaciones.Where(u => !u.BorradoLogico);
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                q = q.Where(u =>
                    u.Pasillo.Contains(SearchString) ||
                    u.Estante.Contains(SearchString));
            }

            Ubicaciones = await q
                .OrderBy(u => u.Pasillo)
                .ThenBy(u => u.Estante)
                .ToListAsync();
        }

        // POST Create
        public async Task<IActionResult> OnPostCreateAsync()
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(CreateUbicacione.Pasillo))
                ModelState.AddModelError("CreateUbicacione.Pasillo", "El pasillo es obligatorio.");
            if (string.IsNullOrWhiteSpace(CreateUbicacione.Estante))
                ModelState.AddModelError("CreateUbicacione.Estante", "El estante es obligatorio.");
            if (await _context.Ubicaciones.AnyAsync(u =>
                    u.Pasillo == CreateUbicacione.Pasillo &&
                    u.Estante == CreateUbicacione.Estante &&
                    !u.BorradoLogico))
            {
                ModelState.AddModelError("CreateUbicacione.Pasillo",
                    "Ya existe una ubicación con ese pasillo y estante.");
            }

            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .Select(ms => new {
                            field = ms.Key,
                            messages = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        });
                    return new JsonResult(new { success = false, errors });
                }

                await OnGetAsync();
                ShowCreateModal = true;
                return Page();
            }

            // Guardar
            CreateUbicacione.BorradoLogico = false;
            _context.Ubicaciones.Add(CreateUbicacione);
            await _context.SaveChangesAsync();

            if (IsAjaxRequest())
            {
                return new JsonResult(new
                {
                    success = true,
                    ubicacion = new
                    {
                        id = CreateUbicacione.IdUbicacion,
                        pasillo = CreateUbicacione.Pasillo,
                        estante = CreateUbicacione.Estante
                    }
                });
            }

            return RedirectToPage(new { SearchString });
        }

        // POST Edit
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (string.IsNullOrWhiteSpace(EditUbicacione.Pasillo))
                ModelState.AddModelError("EditUbicacione.Pasillo", "El pasillo es obligatorio.");
            if (string.IsNullOrWhiteSpace(EditUbicacione.Estante))
                ModelState.AddModelError("EditUbicacione.Estante", "El estante es obligatorio.");
            if (await _context.Ubicaciones.AnyAsync(u =>
                    u.Pasillo == EditUbicacione.Pasillo &&
                    u.Estante == EditUbicacione.Estante &&
                    u.IdUbicacion != EditUbicacione.IdUbicacion &&
                    !u.BorradoLogico))
            {
                ModelState.AddModelError("EditUbicacione.Pasillo",
                    "Ya existe otra ubicación con ese pasillo y estante.");
            }

            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                {
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .Select(ms => new {
                            field = ms.Key,
                            messages = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        });
                    return new JsonResult(new { success = false, errors });
                }

                await OnGetAsync();
                ShowEditModal = true;
                return Page();
            }

            var toUpdate = await _context.Ubicaciones.FindAsync(EditUbicacione.IdUbicacion);
            if (toUpdate != null)
            {
                toUpdate.Pasillo = EditUbicacione.Pasillo;
                toUpdate.Estante = EditUbicacione.Estante;
                await _context.SaveChangesAsync();
            }

            if (IsAjaxRequest())
            {
                return new JsonResult(new
                {
                    success = true,
                    ubicacion = new
                    {
                        id = EditUbicacione.IdUbicacion,
                        pasillo = EditUbicacione.Pasillo,
                        estante = EditUbicacione.Estante
                    }
                });
            }

            return RedirectToPage(new { SearchString });
        }

        // POST Delete (lógico)
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var ubic = await _context.Ubicaciones.FindAsync(id);
            if (ubic != null)
            {
                ubic.BorradoLogico = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { SearchString });
        }
    }
}
