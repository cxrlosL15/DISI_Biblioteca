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
    public class IndexModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public IndexModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public IList<Cliente> Cliente { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public async Task OnGetAsync()
        {
            var query = _context.Clientes
                .Include(c => c.IdDireccionNavigation)
                .Where(c => c.EstadoCliente == 1) // Solo clientes activos
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(c =>
                    c.Nombre!.Contains(SearchString) ||
                    c.ApellidoP!.Contains(SearchString) ||
                    c.ApellidoM!.Contains(SearchString) ||
                    c.Correo!.Contains(SearchString));
            }

            Cliente = await query.ToListAsync();
        }

        // Handler para SweetAlert2 desde el Index
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            // 1) Cargo cliente + dirección
            var cliente = await _context.Clientes
                .Include(c => c.IdDireccionNavigation)
                .FirstOrDefaultAsync(c => c.IdClientes == id);

            if (cliente == null)
                return NotFound();

            var direccion = cliente.IdDireccionNavigation;
            if (direccion != null)
            {
                // 2) Compruebo uso compartido
                bool laUsaOtroCliente = await _context.Clientes
                    .AnyAsync(c => c.IdDireccion == direccion.IdDireccion && c.IdClientes != id);
                bool laUsaTicket = await _context.EncabezadoTickets
                    .AnyAsync(t => t.IdDireccion == direccion.IdDireccion);

                if (laUsaOtroCliente || laUsaTicket)
                {
                    // 3) Si está en uso, aviso y salgo SIN borrar nada
                    TempData["WarningDireccion"] =
                        "La direccion sigue en uso por otro cliente o por tickets y no se realizo ningun borrado.";
                    return RedirectToPage();
                }
            }

            // 4) Sólo llego aquí si la dirección NO está en uso (o es null)
            //    Aplico el borrado lógico del cliente...
            cliente.EstadoCliente = 0;
            _context.Clientes.Update(cliente);

            //    ...y elimino la dirección (porque sé que no hay referencias)
            if (direccion != null)
                _context.Direccions.Remove(direccion);

            // 5) Guardo TODO en una única transacción implícita
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }


    }
}
