using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Autores
{
    public class IndexModel : PageModel
    {
        private readonly BibliotecaContext _context;
        public IndexModel(BibliotecaContext context) => _context = context;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public IList<Autore> Autores { get; set; } = new List<Autore>();

        [BindProperty] public Autore CreateAutore { get; set; } = new Autore();
        [BindProperty] public Autore EditAutore { get; set; } = new Autore();

        public bool ShowCreateModal { get; set; }
        public bool ShowEditModal { get; set; }

        private bool IsAjaxRequest() =>
            Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        public async Task OnGetAsync()
        {
            var q = _context.Autores.Where(a => !a.BorradoLogico);
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                q = q.Where(a =>
                    a.Nombres!.Contains(SearchString) ||
                    a.ApellidoP!.Contains(SearchString) ||
                    a.ApellidoM!.Contains(SearchString));
            }
            Autores = await q
                .OrderBy(a => a.ApellidoP)
                .ThenBy(a => a.ApellidoM)
                .ThenBy(a => a.Nombres)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (string.IsNullOrWhiteSpace(CreateAutore.Nombres))
                ModelState.AddModelError("CreateAutore.Nombres", "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(CreateAutore.ApellidoP))
                ModelState.AddModelError("CreateAutore.ApellidoP", "El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(CreateAutore.ApellidoM))
                ModelState.AddModelError("CreateAutore.ApellidoM", "El apellido materno es obligatorio.");

            if (await _context.Autores.AnyAsync(a =>
                    a.Nombres == CreateAutore.Nombres &&
                    a.ApellidoP == CreateAutore.ApellidoP &&
                    a.ApellidoM == CreateAutore.ApellidoM &&
                    !a.BorradoLogico))
            {
                ModelState.AddModelError("CreateAutore.Nombres",
                    "Ya existe un autor con ese nombre y apellidos.");
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

            CreateAutore.BorradoLogico = false;
            _context.Autores.Add(CreateAutore);
            await _context.SaveChangesAsync();

            if (IsAjaxRequest())
            {
                return new JsonResult(new
                {
                    success = true,
                    autor = new
                    {
                        id = CreateAutore.IdAutores,
                        nombres = CreateAutore.Nombres,
                        apellidoP = CreateAutore.ApellidoP,
                        apellidoM = CreateAutore.ApellidoM
                    }
                });
            }

            return RedirectToPage(new { SearchString });
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (string.IsNullOrWhiteSpace(EditAutore.Nombres))
                ModelState.AddModelError("EditAutore.Nombres", "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(EditAutore.ApellidoP))
                ModelState.AddModelError("EditAutore.ApellidoP", "El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(EditAutore.ApellidoM))
                ModelState.AddModelError("EditAutore.ApellidoM", "El apellido materno es obligatorio.");

            if (await _context.Autores.AnyAsync(a =>
                    a.Nombres == EditAutore.Nombres &&
                    a.ApellidoP == EditAutore.ApellidoP &&
                    a.ApellidoM == EditAutore.ApellidoM &&
                    a.IdAutores != EditAutore.IdAutores &&
                    !a.BorradoLogico))
            {
                ModelState.AddModelError("EditAutore.Nombres",
                    "Ya existe otro autor con ese nombre y apellidos.");
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

            var toUpdate = await _context.Autores.FindAsync(EditAutore.IdAutores);
            if (toUpdate != null)
            {
                toUpdate.Nombres = EditAutore.Nombres;
                toUpdate.ApellidoP = EditAutore.ApellidoP;
                toUpdate.ApellidoM = EditAutore.ApellidoM;
                await _context.SaveChangesAsync();
            }

            if (IsAjaxRequest())
            {
                return new JsonResult(new
                {
                    success = true,
                    autor = new
                    {
                        id = EditAutore.IdAutores,
                        nombres = EditAutore.Nombres,
                        apellidoP = EditAutore.ApellidoP,
                        apellidoM = EditAutore.ApellidoM
                    }
                });
            }

            return RedirectToPage(new { SearchString });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor != null)
            {
                autor.BorradoLogico = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { SearchString });
        }
    }
}