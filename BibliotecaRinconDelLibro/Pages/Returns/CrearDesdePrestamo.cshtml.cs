using System;
using System.Threading.Tasks;
using BibliotecaRinconDelLibro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Pages.Returns
{
    public class CrearDesdePrestamoModel : PageModel
    {
        private readonly BibliotecaContext _context;

        public CrearDesdePrestamoModel(BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int IdPrestamo { get; set; }

        public string? Mensaje { get; set; }

        public List<SelectListItem> PrestamosDisponibles { get; set; } = new();

        public async Task OnGetAsync()
        {
            var prestamosConClientes = await _context.Prestamos
                .Include(p => p.IdClientesNavigation)
                .ToListAsync();

            var prestamosSinDevolucion = prestamosConClientes
                .Where(p => !_context.Devoluciones.Any(d => d.IdPrestamo == p.IdPrestamo))
                .ToList();

            PrestamosDisponibles = prestamosSinDevolucion
                .Select(p => new SelectListItem
                {
                    Value = p.IdPrestamo.ToString(),
                    Text = $"#{p.IdPrestamo} - {p.IdClientesNavigation.Nombre} {p.IdClientesNavigation.ApellidoP} {p.IdClientesNavigation.ApellidoM}"
                }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
             if (IdPrestamo <= 0)
            {
                TempData["Mensaje"] = "Por favor, ingresa un ID de prestamo valido.";
                return Page();
            }

            var prestamo = await _context.Prestamos
                .Include(p => p.IdClientesNavigation)
                .FirstOrDefaultAsync(p => p.IdPrestamo == IdPrestamo);

            if (prestamo == null)
            {
                TempData["Mensaje"] = $"No se encontro el prestamo con ID {IdPrestamo}.";
                return Page();
            }

            bool yaDevuelto = await _context.Devoluciones.AnyAsync(d => d.IdPrestamo == IdPrestamo);
            if (yaDevuelto)
            {
                TempData["Mensaje"] = "Este prestamo ya tiene una devolucion registrada.";
                return Page();
            }

            var devolucion = new Devolucione
            {
                IdPrestamo = IdPrestamo,
                FechaDevolucion = DateTime.Now
            };

            _context.Devoluciones.Add(devolucion);

            // Cambia el estado del pr�stamo a "Devuelto" 
            prestamo.IdEstadoPrestamo = "Devuelto";

            //Nuevo
            var libroId = prestamo.IdLibro; // Aseg�rate que esta propiedad existe en el modelo Prestamo

            // Buscar el registro de disponibilidad relacionado
            var disponibilidad = await _context.Disponibilidads.FirstOrDefaultAsync(d => d.IdLibro == libroId);

            if (disponibilidad != null &&
            disponibilidad.CopiasPrestadas.HasValue &&
            disponibilidad.TotalLibros.HasValue)
            {
                if (disponibilidad.CopiasPrestadas > 0)
                    disponibilidad.CopiasPrestadas--;

                disponibilidad.TotalDespuesPrestamos = disponibilidad.TotalLibros - disponibilidad.CopiasPrestadas;
            }

            //hasta aca

            await _context.SaveChangesAsync();

            // Verifica si se pas� la fecha de devoluci�n
            if (DateTime.Now.Date > prestamo.FechaDevolucion.Date)
            {
                TempData["Mensaje"] = " Devolucion registrada. El prestamo se devolvio tarde. Se debe generar una multa.";
            }
            else
            {
                TempData["Mensaje"] = $" Devolucion registrada correctamente para prestamo ID {IdPrestamo}.";
            }

            return RedirectToPage("./CrearDesdePrestamo"); // o "../Index" si prefieres volver al listado
        }
    }
}
