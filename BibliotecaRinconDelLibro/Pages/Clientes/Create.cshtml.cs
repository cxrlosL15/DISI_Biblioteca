using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BibliotecaRinconDelLibro.Models;

namespace BibliotecaRinconDelLibro.Pages.Clientes
{
    public class CreateModel : PageModel
    {
        private readonly BibliotecaRinconDelLibro.Models.BibliotecaContext _context;

        public CreateModel(BibliotecaRinconDelLibro.Models.BibliotecaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Estados = ObtenerEstados();
            return Page();
        }

        [BindProperty]
        public Cliente Cliente { get; set; } = default!;

        [BindProperty]
        public Direccion Direccion { get; set; } = default!;
        public List<string> Estados { get; set; } = new();
        public async Task<IActionResult> OnPostAsync()
        {
            Estados = ObtenerEstados();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Guardar la dirección primero
            _context.Direccions.Add(Direccion);
            await _context.SaveChangesAsync();

            // Asociar la dirección guardada al cliente
            Cliente.IdDireccion = Direccion.IdDireccion;

            //Guardar cliente
            _context.Clientes.Add(Cliente);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        private List<string> ObtenerEstados()
        {
            return new List<string>
            {
                "Aguascalientes", "Baja California", "Baja California Sur", "Campeche", "Chiapas",
                "Chihuahua", "Ciudad de México", "Coahuila", "Colima", "Durango", "Estado de México",
                "Guanajuato", "Guerrero", "Hidalgo", "Jalisco", "Michoacán", "Morelos", "Nayarit",
                "Nuevo León", "Oaxaca", "Puebla", "Querétaro", "Quintana Roo", "San Luis Potosí",
                "Sinaloa", "Sonora", "Tabasco", "Tamaulipas", "Tlaxcala", "Veracruz", "Yucatán", "Zacatecas"
            };
        }
    }
}
