using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class ImgLibro
{
    [Key]
    [Column("id_imgLibros")]
    public int IdImgLibros { get; set; }

    [Column("imgLibros")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ImgLibros { get; set; }

    [InverseProperty("IdImgLibrosNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
