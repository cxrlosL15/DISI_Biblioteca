using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("nombre")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Nombre { get; set; }

    [Column("apellidoP")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoP { get; set; }

    [Column("apellidoM")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ApellidoM { get; set; }

    [Column("correo")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Correo { get; set; }

    [Column("contrasena")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Contrasena { get; set; }

    [Column("id_rol")]
    public int? IdRol { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<EncabezadoTicket> EncabezadoTickets { get; set; } = new List<EncabezadoTicket>();

    [ForeignKey("IdRol")]
    [InverseProperty("Usuarios")]
    public virtual Rol? IdRolNavigation { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {ApellidoP} {ApellidoM}";

}
