using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class TipoMultum
{
    [Key]
    [Column("idTipoMulta")]
    public int IdTipomulta { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Tipo { get; set; }

    [Column("Descripcion_Multa")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Descripcion { get; set; }

    [Column("Precio_Multa")]
    public double? MontoBase { get; set; }

    [InverseProperty("IdTipomultaNavigation")]
    public virtual ICollection<Multa> Multa { get; set; } = new List<Multa>();

    [InverseProperty("IdTipoMultaNavigation")]
    public virtual ICollection<TicketMultum> TicketMulta { get; set; } = new List<TicketMultum>();
}
