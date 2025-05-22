using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Ubicacione
{
    [Key]
    [Column("id_ubicacion")]
    public int IdUbicacion { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Pasillo { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Estante { get; set; }

    [InverseProperty("IdUbicacionNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
