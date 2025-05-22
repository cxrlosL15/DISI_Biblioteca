using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("Disponibilidad")]
public partial class Disponibilidad
{
    [Required(ErrorMessage = "La disponibilidad es obligatoria")]
    [Key]
    [Column("id_Disponibilidad")]
    public int IdDisponibilidad { get; set; }

    
    [Column("id_Libro")]
    public int? IdLibro { get; set; }
    

    [Required(ErrorMessage = "El total de libros es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El total de libros debe ser mayor que 0.")]
    [Display(Name = "Total de libros")]
    [Column("Total_Libros")]
    public int? TotalLibros { get; set; }

    [Display(Name = "Copias prestadas")]
    public int? CopiasPrestadas { get; set; }

    [Display(Name = "Total después de préstamos")]
    public int? TotalDespuesPrestamos { get; set; }

    // Clave foránea hacia la tabla Prestamo
    [Column("id_prestamo")]
    public int? IdPrestamo { get; set; }
    
    [ForeignKey("IdPrestamo")]
    [InverseProperty("Disponibilidads")]
    public virtual Prestamo? IdPrestamoNavigation { get; set; }

    [InverseProperty("IdDisponibilidadNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
