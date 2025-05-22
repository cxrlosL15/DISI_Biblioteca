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
    public class CreateModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public CreateModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BibliotecaRinconDelLibro.Models.Prestamo Prestamo { get; set; } = new BibliotecaRinconDelLibro.Models.Prestamo
        {
            FechaPrestamo = DateTime.Now,
            FechaDevolucion = DateTime.Now.AddDays(7)
        };

        public List<SelectListItem>? Clientes { get; set; }
        public List<SelectListItem>? Libros { get; set; }
        public List<SelectListItem>? EstadosLibro { get; set; }
        public List<SelectListItem>? Usuarios { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return Page();
            }

            var disponibilidad = await _context.Disponibilidads
                .FirstOrDefaultAsync(d => d.IdLibro == Prestamo.IdLibro);

            if (disponibilidad == null)
            {
                ModelState.AddModelError(string.Empty, "No se encontró disponibilidad para el libro seleccionado.");
                await CargarListasAsync();
                return Page();
            }

            int disponibles = disponibilidad.TotalDespuesPrestamos ?? 0;
            if (disponibles <= 0)

            {
                ModelState.AddModelError(string.Empty, "No hay ejemplares disponibles para prestar.");
                await CargarListasAsync();
                return Page();
            }

            _context.Prestamos.Add(Prestamo);

            disponibilidad.CopiasPrestadas = (disponibilidad.CopiasPrestadas ?? 0) + 1;
            disponibilidad.TotalDespuesPrestamos = (disponibilidad.TotalLibros ?? 0) - disponibilidad.CopiasPrestadas;

            _context.Disponibilidads.Update(disponibilidad);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task CargarListasAsync()
        {
            Clientes = await _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.IdClientes.ToString(),
                    Text = c.NombreCompleto
                }).ToListAsync();

            Libros = await _context.Libros
                .Select(l => new SelectListItem
                {
                    Value = l.IdLibro.ToString(),
                    Text = l.Titulo
                }).ToListAsync();

            EstadosLibro = await _context.EstadoLibros
                .Select(e => new SelectListItem
                {
                    Value = e.IdEstadoLibro.ToString(),
                    Text = e.DescripcionEstado
                }).ToListAsync();

            Usuarios = await _context.Usuarios
                .Select(u => new SelectListItem
                {
                    Value = u.IdUsuario.ToString(),
                    Text = u.NombreCompleto
                }).ToListAsync();
        }
    }
}
