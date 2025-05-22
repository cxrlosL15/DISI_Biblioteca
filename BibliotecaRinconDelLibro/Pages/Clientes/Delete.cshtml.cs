using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Clientes
{
    public class DeleteModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public DeleteModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cliente = await _context.Clientes
                            .Include(c => c.IdDireccionNavigation)
                            .FirstOrDefaultAsync(m => m.IdClientes == id) ?? throw new InvalidOperationException("Cliente no encontrado.");
            if (Cliente == null)
            {
                return NotFound();
            }
        
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.Clientes
                .Include(c => c.IdDireccionNavigation)
                .FirstOrDefaultAsync(c => c.IdClientes == id);

            if (cliente == null)
                return NotFound();

            cliente.EstadoCliente = 0;
            _context.Clientes.Update(cliente);

            var direccion = Cliente.IdDireccionNavigation;

            

            // Verifica si la dirección no está siendo usada por otros clientes ni por encabezados de tickets
            if (direccion != null)
            {
                bool esDireccionCompartida = await _context.Clientes
                    .AnyAsync(c => c.IdDireccion == direccion.IdDireccion && c.IdClientes != Cliente.IdClientes);

                if (!esDireccionCompartida)
                {
                    _context.Direccions.Remove(direccion);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
