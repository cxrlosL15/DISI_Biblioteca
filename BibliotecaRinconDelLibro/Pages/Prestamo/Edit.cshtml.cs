using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Prestamo
{
    public class EditModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public EditModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BibliotecaRinconDelLibro.Models.Prestamo Prestamo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos.FirstOrDefaultAsync(m => m.IdPrestamo == id);
            if (prestamo == null)
            {
                return NotFound();
            }

            Prestamo = prestamo;

            // Asignación de listas con valores legibles
            ViewData["IdClientes"] = new SelectList(_context.Clientes, "IdClientes", "NombreCompleto", Prestamo.IdClientes);
            ViewData["IdLibro"] = new SelectList(_context.Libros, "IdLibro", "Titulo", Prestamo.IdLibro);
            ViewData["IdEstadoLibro"] = new SelectList(_context.EstadoLibros, "IdEstadoLibro", "DescripcionEstado", Prestamo.IdEstadoLibro);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "NombreCompleto", Prestamo.IdUsuario);


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Recargar listas para la vista si falla validación
                ViewData["IdClientes"] = new SelectList(_context.Clientes, "IdClientes", "NombreCompleto", Prestamo.IdClientes);
                ViewData["IdLibro"] = new SelectList(_context.Libros, "IdLibro", "Titulo", Prestamo.IdLibro);
                ViewData["IdEstadoLibro"] = new SelectList(_context.EstadoLibros, "IdEstadoLibro", "DescripcionEstado", Prestamo.IdEstadoLibro);
                ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "NombreCompleto", Prestamo.IdUsuario);



                return Page();
            }

            _context.Attach(Prestamo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrestamoExists(Prestamo.IdPrestamo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PrestamoExists(int id)
        {
            return _context.Prestamos.Any(e => e.IdPrestamo == id);
        }
    }
}
