using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaRinconDelLibro.Models;


public partial class Prestamo
{

    [Key]
    [Column("id_prestamo")]
    public int IdPrestamo { get; set; }

    [Column("id_clientes")]
    public int? IdClientes { get; set; }

    [Column("id_libro")]
    public int? IdLibro { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Column("id_EstadoPrestamo")]
    public string? IdEstadoPrestamo { get; set; } = "Prestado";

    [Required(ErrorMessage = "La fecha de préstamo es obligatoria")]
    [Column("fecha_prestamo")]
    [DataType(DataType.Date)]
    public DateTime FechaPrestamo { get; set; }=DateTime.Now;

    [Required(ErrorMessage = "La fecha de devolución es obligatoria")]
    [Column("fecha_devolucion")]
    [DataType(DataType.Date)]
    public DateTime FechaDevolucion { get; set; } = DateTime.Now.AddDays(7);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaDevolucion < FechaPrestamo)
        {
            yield return new ValidationResult(
                "La fecha de devolución no puede ser anterior a la fecha de préstamo.",
                new[] { nameof(FechaDevolucion) });
        }
    }

    [Column("id_Estado_Libro")]
    public int? IdEstadoLibro { get; set; }

    [Column("id_usuario")]
    public int? IdUsuario { get; set; }

  
   /* [Column("id_Devoluciones")]
    public int? IdDevoluciones { get; set; }*/

    [InverseProperty("IdPrestamoNavigation")]
    public virtual ICollection<Devolucione> Devoluciones { get; set; } = new List<Devolucione>();

    [InverseProperty("IdPrestamoNavigation")]
    public virtual ICollection<Disponibilidad> Disponibilidads { get; set; } = new List<Disponibilidad>();

    [ForeignKey("IdClientes")]
    [InverseProperty("Prestamos")]
    public virtual Cliente? IdClientesNavigation { get; set; }

    [ForeignKey("IdEstadoLibro")]
    [InverseProperty("Prestamos")]
    public virtual EstadoLibro? IdEstadoLibroNavigation { get; set; }

    [ForeignKey("IdLibro")]
    [InverseProperty("Prestamos")]
    public virtual Libro? IdLibroNavigation { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("Prestamos")]
    public virtual Usuario? IdUsuarioNavigation { get; set; }

    [InverseProperty("IdPrestamoNavigation")]
    public virtual ICollection<Multa> Multa { get; set; } = new List<Multa>();

    [InverseProperty("IdPrestamoNavigation")]
    public virtual ICollection<TicketMultum> TicketMulta { get; set; } = new List<TicketMultum>();

    [InverseProperty("IdPrestamoNavigation")]
    public virtual ICollection<TicketPrestamo> TicketPrestamos { get; set; } = new List<TicketPrestamo>();
}
