using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

[Table("EncabezadoTicket")]
public partial class EncabezadoTicket
{
    [Key]
    [Column("id_Encabezado_Ticket")]
    public int IdEncabezadoTicket { get; set; }

    [Column("Nombre_Biblioteca")]
    [StringLength(255)]
    [Unicode(false)]
    public string? NombreBiblioteca { get; set; }

    [Column("Numero_Telefono")]
    public int? NumeroTelefono { get; set; }

    [Column("id_direccion")]
    public int? IdDireccion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column("id_Usuario")]
    public int? IdUsuario { get; set; }

    [ForeignKey("IdDireccion")]
    [InverseProperty("EncabezadoTickets")]
    public virtual Direccion? IdDireccionNavigation { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("EncabezadoTickets")]
    public virtual Usuario? IdUsuarioNavigation { get; set; }

    [InverseProperty("IdEncabezadoTicketNavigation")]
    public virtual ICollection<TicketMultum> TicketMulta { get; set; } = new List<TicketMultum>();

    [InverseProperty("IdEncabezadoTicketNavigation")]
    public virtual ICollection<TicketPrestamo> TicketPrestamos { get; set; } = new List<TicketPrestamo>();
}
