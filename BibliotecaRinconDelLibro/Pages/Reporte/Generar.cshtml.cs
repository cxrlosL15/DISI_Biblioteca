using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BibliotecaRinconDelLibro.Models; // Asegúrate también de incluir tus modelos

public class ReportesModel : PageModel
{
    private readonly BibliotecaContext _context;

    public ReportesModel(BibliotecaContext context)
    {
        _context = context;
    }

    // Datos para los gráficos
    public List<string> LibrosLabels { get; set; }
    public List<int> LibrosData { get; set; }
    public List<string> ClientesLabels { get; set; }
    public List<int> ClientesData { get; set; }
    public List<string> InventarioLabels { get; set; }
    public List<int> InventarioData { get; set; }

    public void OnGet()
    {
        CargarDatos();
    }

    private void CargarDatos()
    {
        LibrosLabels = _context.Libros
            .OrderByDescending(l => l.Prestamos.Count)
            .Take(5)
            .Select(l => l.Titulo)
            .ToList();

        LibrosData = _context.Libros
            .OrderByDescending(l => l.Prestamos.Count)
            .Take(5)
            .Select(l => l.Prestamos.Count)
            .ToList();

        ClientesLabels = _context.Clientes
            .Select(c => new
            {
                Nombre = c.Nombre + " " + c.ApellidoP,
                CantMultas = c.Prestamos.SelectMany(p => p.Multa).Count()
            })
            .Where(c => c.CantMultas > 0)
            .OrderByDescending(c => c.CantMultas)
            .Take(5)
            .Select(c => c.Nombre)
            .ToList();

        ClientesData = _context.Clientes
            .Select(c => c.Prestamos.SelectMany(p => p.Multa).Count())
            .Where(m => m > 0)
            .OrderByDescending(m => m)
            .Take(5)
            .ToList();

        int total = _context.Libros.Count();
        int prestados = _context.Prestamos.Count(p => p.FechaDevolucion == null);
        int disponibles = total - prestados;

        InventarioLabels = new List<string> { "Total", "Prestados", "Disponibles" };
        InventarioData = new List<int> { total, prestados, disponibles };
    }

    public async Task<IActionResult> OnPostGenerarPDF(string SeccionesSeleccionadas)
    {
        var secciones = SeccionesSeleccionadas?.Split(',') ?? Array.Empty<string>();

        var logoBytes = System.IO.File.ReadAllBytes("C:\\Logos\\name.png");

        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.Background("#FAFAF8"); // SEASALT

                page.Header().Row(row =>
                {
                    row.ConstantItem(80).Image("C:\\Logos\\logo.png"); // Ruta local o webroot

                    row.RelativeItem().AlignMiddle().Text("         REPORTE DE BIBLIOTECA")
                        .FontFamily("Montserrat")
                        .FontSize(20)
                        .Bold()
                        .FontColor("#0B0F27");
                });

                page.Content().PaddingTop(20).Column(column =>
                {
                    if (secciones.Contains("libros"))
                    {
                        var libros = _context.Libros
                            .OrderByDescending(l => l.Prestamos.Count)
                            .Take(5)
                            .Select(l => new { l.Titulo, Prestamos = l.Prestamos.Count })
                            .ToList();

                        column.Item().Element(e => e.PaddingBottom(5)).Text("Libros más prestados")
                            .FontSize(14)
                            .FontFamily("Montserrat")
                            .Bold()
                            .FontColor("#6F8249");

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Los Libros mas vistos son:")
                                    .FontFamily("Open Sans").Bold().FontColor("#0B0F27");
                                header.Cell().Text("Préstamos")
                                    .FontFamily("Open Sans").Bold().FontColor("#0B0F27");
                            });

                            foreach (var libro in libros)
                            {
                                table.Cell().Text(libro.Titulo)
                                    .FontFamily("Open Sans").FontColor("#0B0F27");
                                table.Cell().Text(libro.Prestamos.ToString())
                                    .FontFamily("Open Sans").FontColor("#0B0F27");
                            }
                        });
                    }

                    if (secciones.Contains("clientes"))
                    {
                        var clientes = _context.Multas
                            .Where(m => m.IdPrestamoNavigation != null && m.IdPrestamoNavigation.IdClientesNavigation != null)
                            .GroupBy(m => m.IdPrestamoNavigation.IdClientesNavigation)
                            .Select(g => new
                            {
                                Nombre = $"{g.Key.Nombre} {g.Key.ApellidoP}",
                                Multas = g.Count()
                            })
                            .OrderByDescending(g => g.Multas)
                            .Take(5)
                            .ToList();

                        column.Item().Element(e => e.PaddingBottom(5)).Text("Clientes con más multas")
                            .FontSize(14)
                            .FontFamily("Montserrat")
                            .Bold()
                            .FontColor("#765E49");

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Cliente")
                                    .FontFamily("Open Sans").Bold().FontColor("#0B0F27");
                                header.Cell().Text("Multas")
                                    .FontFamily("Open Sans").Bold().FontColor("#0B0F27");
                            });

                            foreach (var cliente in clientes)
                            {
                                table.Cell().Text(cliente.Nombre)
                                    .FontFamily("Open Sans").FontColor("#0B0F27");
                                table.Cell().Text(cliente.Multas.ToString())
                                    .FontFamily("Open Sans").FontColor("#0B0F27");
                            }
                        });
                    }

                    if (secciones.Contains("inventario"))
                    {
                        var inventario = _context.Libros
                            .Include(l => l.IdCategoriasNavigation)
                            .ToList() // ← fuerza la ejecución en memoria
                            .GroupBy(l => l.IdCategoriasNavigation?.Categoria1 ?? "Sin categoría")
                            .Select(g => new { Categoria = g.Key, Cantidad = g.Count() })
                            .ToList();

                        column.Item().Element(e => e.PaddingBottom(5)).Text("Inventario general")
                            .FontSize(14)
                            .FontFamily("Montserrat")
                            .Bold()
                            .FontColor("#6F8249");

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Categoría")
                                    .FontFamily("Open Sans").Bold().FontColor("#0B0F27");
                                header.Cell().Text("Cantidad")
                                    .FontFamily("Open Sans").Bold().FontColor("#0B0F27");
                            });

                            foreach (var item in inventario)
                            {
                                table.Cell().Text(item.Categoria)
                                    .FontFamily("Open Sans").FontColor("#0B0F27");
                                table.Cell().Text(item.Cantidad.ToString())
                                    .FontFamily("Open Sans").FontColor("#0B0F27");
                            }
                        });
                    }
                });

                page.Footer().Column(col =>
                {
                    col.Item().AlignCenter().MaxWidth(300).Image(logoBytes);
                    col.Item().AlignCenter().Text(txt =>
                    {
                        txt.Span("Página ").FontFamily("Open Sans");
                        txt.CurrentPageNumber().FontColor("#0B0F27");
                        txt.Span(" de ");
                        txt.TotalPages().FontColor("#0B0F27");
                    });
                });
            });
        });

        byte[] pdfBytes = documento.GeneratePdf();
        return File(pdfBytes, "application/pdf", "Reporte_Biblioteca.pdf");
    }
}

