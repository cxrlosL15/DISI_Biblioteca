using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class ImgLibro
{
    [Required(ErrorMessage = "La imagen es obligatoria")]
    [Key]
    [Column("id_imgLibros")]
    public int? IdImgLibros { get; set; }

    [Display(Name = "Imagen")]
    [Column("imgLibros")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ImgLibros { get; set; }

    [Required(ErrorMessage = "La imagen es obligatoria.")]
    [InverseProperty("IdImgLibrosNavigation")]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
