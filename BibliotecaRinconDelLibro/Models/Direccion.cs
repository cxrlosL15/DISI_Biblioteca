using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("Direccion")]
public partial class Direccion
{
    [Key]
    [Column("id_direccion")]
    public int IdDireccion { get; set; }

    [Column("calle")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Calle { get; set; }

    [Column("colonia")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Colonia { get; set; }

    [Column("codigo_postal")]
    [StringLength(10)]
    [Unicode(false)]
    public string? CodigoPostal { get; set; }

    [Column("ciudad")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Ciudad { get; set; }

    [Column("estado")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Estado { get; set; }

    [InverseProperty("IdDireccionNavigation")]
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    [InverseProperty("IdDireccionNavigation")]
    public virtual ICollection<EncabezadoTicket> EncabezadoTickets { get; set; } = new List<EncabezadoTicket>();
}
