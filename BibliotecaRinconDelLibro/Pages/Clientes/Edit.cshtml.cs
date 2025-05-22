using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Clientes
{
    public class EditModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public EditModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = default!;

        [BindProperty]
        public Direccion Direccion { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Obtener el cliente y su dirección asociada
            Cliente = await _context.Clientes
                .Include(c => c.IdDireccionNavigation)  
                .FirstOrDefaultAsync(m => m.IdClientes == id) ?? throw new InvalidOperationException("Cliente no encontrado.");

            if (Cliente == null)
            {
                return NotFound();
            }

            // Pasar la dirección al modelo para el formulario
            Direccion = Cliente.IdDireccionNavigation ?? new Direccion();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Buscar el cliente en la base de datos
            var clienteToUpdate = await _context.Clientes
                .Include(c => c.IdDireccionNavigation)
                .FirstOrDefaultAsync(c => c.IdClientes == Cliente.IdClientes);

            if (clienteToUpdate == null)
            {
                return NotFound();
            }

            // Actualizar los datos del cliente
            clienteToUpdate.Nombre = Cliente.Nombre;
            clienteToUpdate.ApellidoP = Cliente.ApellidoP;
            clienteToUpdate.ApellidoM = Cliente.ApellidoM;
            clienteToUpdate.Correo = Cliente.Correo;
            clienteToUpdate.Valoracion = Cliente.Valoracion;

            // Actualizar los datos de la dirección
            if (clienteToUpdate.IdDireccionNavigation != null)
            {
                clienteToUpdate.IdDireccionNavigation.Calle = Direccion.Calle;
                clienteToUpdate.IdDireccionNavigation.Colonia = Direccion.Colonia;
                clienteToUpdate.IdDireccionNavigation.CodigoPostal = Direccion.CodigoPostal;
                clienteToUpdate.IdDireccionNavigation.Ciudad = Direccion.Ciudad;
                clienteToUpdate.IdDireccionNavigation.Estado = Direccion.Estado;

                _context.Update(clienteToUpdate.IdDireccionNavigation);
            }
            _context.Update(clienteToUpdate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clientes.Any(e => e.IdClientes == Cliente.IdClientes))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.IdClientes == id);
        }
    }
}
