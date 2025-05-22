using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("EstadoLibro")]
public partial class EstadoLibro
{
    [Key]
    [Column("id_Estado_Libro")]
    public int IdEstadoLibro { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? DescripcionEstado { get; set; }

    [InverseProperty("IdEstadoLibroNavigation")]
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
