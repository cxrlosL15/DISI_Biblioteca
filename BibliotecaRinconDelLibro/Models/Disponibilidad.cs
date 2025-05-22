using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("Disponibilidad")]
public partial class Disponibilidad
{
    [Key]
    [Column("id_Disponibilidad")]
    public int IdDisponibilidad { get; set; }

    [Column("id_Libro")]
    public int? IdLibro { get; set; }

    [Column("Total_Libros")]
    public int? TotalLibros { get; set; }

    public int? CopiasPrestadas { get; set; }

    public int? TotalDespuesPrestamos { get; set; }

    // Clave foránea hacia la tabla Prestamo
    [Column("id_prestamo")]
    public int? IdPrestamo { get; set; }

    [ForeignKey("IdPrestamo")]
    [InverseProperty("Disponibilidads")]
    public virtual Prestamo? IdPrestamoNavigation { get; set; }
}
