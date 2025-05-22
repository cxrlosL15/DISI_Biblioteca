using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Editoriales
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public IndexModel(BibliotecaContext context) => _context = context;

        // Para la barra de búsqueda
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        // Lista filtrada de editoriales
        public IList<Editorial> Editoriales { get; set; } = default!;

        // BindProperties para los formularios de modal
        [BindProperty] public Editorial CreateEditorial { get; set; } = new Editorial();
        [BindProperty] public Editorial EditEditorial { get; set; } = new Editorial();

        // Bandera para indicar si debemos reabrir el modal de Crear
        public bool ShowCreateModal { get; set; } = false;
        public bool ShowEditModal { get; set; } = false;
        private bool IsAjaxRequest() =>
    Request.Headers["X-Requested-With"] == "XMLHttpRequest";


        // Carga inicial + búsqueda
        public async Task OnGetAsync()
        {
            var q = _context.Editorials.Where(e => !e.BorradoLogico);
            if (!string.IsNullOrWhiteSpace(SearchString))
                q = q.Where(e => e.Nombre.Contains(SearchString));
            Editoriales = await q.OrderBy(e => e.Nombre).ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (await _context.Editorials
                  .AnyAsync(e => e.Nombre == CreateEditorial.Nombre && !e.BorradoLogico))
            {
                ModelState.AddModelError("CreateEditorial.Nombre",
                    "Ya existe una editorial con ese nombre.");
            }

            if (string.IsNullOrWhiteSpace(CreateEditorial.Nombre))
            {
                ModelState.AddModelError("CreateEditorial.Nombre", "El nombre es obligatorio.");
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

            CreateEditorial.BorradoLogico = false;
            _context.Editorials.Add(CreateEditorial);
            await _context.SaveChangesAsync();

            if (IsAjaxRequest())
            {
                return new JsonResult(new
                {
                    success = true,
                    editorial = new
                    {
                        id = CreateEditorial.IdEditorial,
                        nombre = CreateEditorial.Nombre
                    }
                });
            }

            return RedirectToPage(new { SearchString });
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (await _context.Editorials.AnyAsync(e =>
                    e.Nombre == EditEditorial.Nombre &&
                    e.IdEditorial != EditEditorial.IdEditorial &&
                    !e.BorradoLogico))
            {
                ModelState.AddModelError("EditEditorial.Nombre",
                    "Ya existe otra editorial con ese nombre.");
            }

            if (string.IsNullOrWhiteSpace(EditEditorial.Nombre))
            {
                ModelState.AddModelError("EditEditorial.Nombre", "El nombre es obligatorio.");
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

            var toUpdate = await _context.Editorials.FindAsync(EditEditorial.IdEditorial);
            if (toUpdate != null)
            {
                toUpdate.Nombre = EditEditorial.Nombre;
                await _context.SaveChangesAsync();
            }

            if (IsAjaxRequest())
            {
                return new JsonResult(new
                {
                    success = true,
                    editorial = new
                    {
                        id = EditEditorial.IdEditorial,
                        nombre = EditEditorial.Nombre
                    }
                });
            }

            return RedirectToPage(new { SearchString });
        }


        // Borrado lógico
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var editorial = await _context.Editorials.FindAsync(id);
            if (editorial != null)
            {
                editorial.BorradoLogico = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { SearchString });
        }
    }
}
