using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("Editorial")]
public partial class Editorial
{
    [Key]
    [Column("ID_Editorial")]
    public int IdEditorial { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Nombre { get; set; }

    [InverseProperty("IdEditorialNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
