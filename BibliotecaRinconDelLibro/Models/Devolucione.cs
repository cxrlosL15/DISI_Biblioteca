using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Devolucione
{
    [Key]
    [Column("id_devoluciones")]
    public int IdDevoluciones { get; set; }

    [Column("id_prestamo")]
    public int? IdPrestamo { get; set; }

    [Column("motivo")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Motivo { get; set; }

    [Column("id_EstadoRegreso")]
    public int? IdEstadoRegreso { get; set; }

    public DateOnly? FechaDevuelto { get; set; }

    [ForeignKey("IdEstadoRegreso")]
    [InverseProperty("Devoluciones")]
    public virtual EstadoRegreso? IdEstadoRegresoNavigation { get; set; }

    [ForeignKey("IdPrestamo")]
    [InverseProperty("Devoluciones")]
    public virtual Prestamo? IdPrestamoNavigation { get; set; }
}
