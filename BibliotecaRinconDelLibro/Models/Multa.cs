using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Multa
{
    [Key]
    [Column("id_multa")]
    public int IdMulta { get; set; }

    [Column("id_prestamo")]
    public int? IdPrestamo { get; set; }

    [Column("ID_Tipomulta")]
    public int? IdTipomulta { get; set; }

    [Column("monto")]
    public double? Monto { get; set; }

    [Column("descripcion")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Descripcion { get; set; }

    public bool Pagado { get; set; }= false;

    [ForeignKey("IdPrestamo")]
    [InverseProperty("Multa")]
    public virtual Prestamo? IdPrestamoNavigation { get; set; }

    [ForeignKey("IdTipomulta")]
    [InverseProperty("Multa")]
    public virtual TipoMultum? IdTipomultaNavigation { get; set; }
}
