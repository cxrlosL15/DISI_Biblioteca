using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models; // Asegúrate que este sea tu namespace de modelos

namespace BibliotecaRinconDelLibro.Pages.Multas
{
    public class EditModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public EditModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty] // Enlaza los datos del formulario a esta propiedad cuando se envía (POST)
        public BibliotecaRinconDelLibro.Models.Multa Multa { get; set; } = new BibliotecaRinconDelLibro.Models.Multa();

        // Para los dropdowns (listas desplegables)
        public SelectList? PrestamosList { get; set; }
        public SelectList? TiposMultaList { get; set; }
        // Para el estado de Pagado (si usas dropdown para bool?)
        public List<SelectListItem> EstadosPagado { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Pendiente" }, // Para representar null
            new SelectListItem { Value = "true", Text = "Sí" },
            new SelectListItem { Value = "false", Text = "No" }
        };


        private async Task CargarListasDesplegablesAsync(int? idPrestamoSeleccionado, int? idTipoMultaSeleccionado)
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.IdClientesNavigation)
                .Include(p => p.IdLibroNavigation)
                .Select(p => new {
                    p.IdPrestamo,
                    DisplayText = $"ID: {p.IdPrestamo} (Cliente: {(p.IdClientesNavigation != null ? p.IdClientesNavigation.Nombre : "")} {(p.IdClientesNavigation != null ? p.IdClientesNavigation.ApellidoP : "")}, Libro: {(p.IdLibroNavigation != null ? p.IdLibroNavigation.Titulo : "")})"
                })
                .OrderBy(p => p.IdPrestamo) // Es bueno tener un orden
                .ToListAsync();
            PrestamosList = new SelectList(prestamos, "IdPrestamo", "DisplayText", idPrestamoSeleccionado);

            var tipoMultas = await _context.TipoMulta // O _context.TipoMultas o _context.TipoMultums
                .OrderBy(t => t.Tipo)
                .Select(t => new {
                    Id = t.IdTipomulta,
                    Texto = $"{t.Tipo} (Monto base: {(t.MontoBase ?? 0).ToString("C")})" // Asumiendo MontoBase
                })
                .ToListAsync();
            TiposMultaList = new SelectList(tipoMultas, "Id", "Texto", idTipoMultaSeleccionado);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Multas == null)
            {
                return NotFound();
            }

            // Cargar la multa específica que se va a editar
            // Incluye las navegaciones si necesitas mostrar información relacionada que no es directamente editable
            Multa = await _context.Multas
                // .Include(m => m.IdPrestamoNavigation) // Opcional, si lo muestras en la vista de edición
                // .Include(m => m.IdTipomultaNavigation) // Opcional
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (Multa == null)
            {
                return NotFound();
            }

            // Cargar las listas para los dropdowns, preseleccionando los valores actuales de la Multa
            await CargarListasDesplegablesAsync(Multa.IdPrestamo, Multa.IdTipomulta);
            return Page();
        }

        // Para guardar los cambios (POST)
        public async Task<IActionResult> OnPostAsync()
        {
            // Recalcular el Monto basado en el TipoMulta seleccionado, si esa es tu lógica
            // (similar a como lo hiciste en CreateModel.OnPostAsync)
            var tipoMultaSeleccionado = await _context.TipoMulta.FindAsync(Multa.IdTipomulta);
            if (tipoMultaSeleccionado != null)
            {
                if (tipoMultaSeleccionado.Tipo == "Libro no regresado" && Multa.IdPrestamo.HasValue)
                {
                    var prestamoConLibro = await _context.Prestamos
                        .Include(p => p.IdLibroNavigation)
                        .FirstOrDefaultAsync(p => p.IdPrestamo == Multa.IdPrestamo);
                    if (prestamoConLibro?.IdLibroNavigation != null)
                    {
                        Multa.Monto = prestamoConLibro.IdLibroNavigation.ValorReposicion;
                    }
                    else
                    {
                        ModelState.AddModelError("Multa.IdPrestamo", "No se pudo encontrar el libro para calcular el monto de 'Libro no regresado'.");
                    }
                }
                else
                {
                    Multa.Monto = tipoMultaSeleccionado.MontoBase; // Asumiendo MontoBase o PrecioMulta
                }
            }
            else
            {
                ModelState.AddModelError("Multa.IdTipomulta", "Tipo de multa inválido.");
            }


            if (!ModelState.IsValid)
            {
                await CargarListasDesplegablesAsync(Multa.IdPrestamo, Multa.IdTipomulta);
                return Page();
            }

            _context.Attach(Multa).State = EntityState.Modified; // Marca la entidad como modificada

            try
            {
                await _context.SaveChangesAsync(); // Guarda los cambios en la BD
            }
            catch (DbUpdateConcurrencyException) // Manejo de concurrencia (si otro usuario modificó mientras editabas)
            {
                if (!MultaExists(Multa.IdMulta))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Relanza la excepción si no sabes cómo manejarla específicamente
                }
            }

            TempData["SuccessMessage"] = "Multa actualizada exitosamente!"; // Mensaje de éxito opcional
            return RedirectToPage("./Index"); // Redirige a la lista de multas
        }

        private bool MultaExists(int id)
        {
            return (_context.Multas?.Any(e => e.IdMulta == id)).GetValueOrDefault();
        }
    }
}
