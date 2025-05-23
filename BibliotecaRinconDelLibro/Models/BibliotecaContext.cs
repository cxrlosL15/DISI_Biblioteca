using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRinconDelLibro.Models;

public partial class BibliotecaContext : DbContext
{
    public BibliotecaContext()
    {
    }

    public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Autore> Autores { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Devolucione> Devoluciones { get; set; }

    public virtual DbSet<Direccion> Direccions { get; set; }

    public virtual DbSet<Disponibilidad> Disponibilidads { get; set; }

    public virtual DbSet<Editorial> Editorials { get; set; }

    public virtual DbSet<EncabezadoTicket> EncabezadoTickets { get; set; }

    public virtual DbSet<EstadoLibro> EstadoLibros { get; set; }

    public virtual DbSet<EstadoRegreso> EstadoRegresos { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<ImagenesTicket> ImagenesTickets { get; set; }

    public virtual DbSet<ImgLibro> ImgLibros { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Multa> Multas { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TicketMultum> TicketMulta { get; set; }

    public virtual DbSet<TicketPrestamo> TicketPrestamos { get; set; }

    public virtual DbSet<TipoMultum> TipoMulta { get; set; }

    public virtual DbSet<Ubicacione> Ubicaciones { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<LibroAutor> LibroAutores { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Elise\\MSSQLSERVER02;Database=BibliotecaRinconDelLibro;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autore>(entity =>
        {
            entity.HasKey(e => e.IdAutores).HasName("PK__Autores__205B59277FD0D3FB");

            entity.Property(e => e.BorradoLogico)
            .HasColumnName("BorradoLogico")
            .HasDefaultValue(false)
            .IsRequired();
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategorias).HasName("PK__Categori__14E48B7B586C309B");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdClientes).HasName("PK__Clientes__64F1B5C570DB90ED");

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.Clientes).HasConstraintName("FK__Clientes__id_dir__4D94879B");
        });

        modelBuilder.Entity<Devolucione>(entity =>
        {
            entity.HasKey(e => e.IdDevoluciones).HasName("PK__Devoluci__5E5883C18F562A36");

            entity.HasOne(d => d.IdEstadoRegresoNavigation).WithMany(p => p.Devoluciones).HasConstraintName("FK__Devolucio__id_Es__656C112C");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.Devoluciones).HasConstraintName("FK__Devolucio__id_pr__6477ECF3");
        });

        modelBuilder.Entity<Direccion>(entity =>
        {
            entity.HasKey(e => e.IdDireccion).HasName("PK__Direccio__25C35D07B90704E5");
        });

        modelBuilder.Entity<Disponibilidad>(entity =>
        {
            entity.HasKey(e => e.IdDisponibilidad).HasName("PK__Disponib__16970CE1FEFEE68D");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.Disponibilidads).HasConstraintName("FK__Disponibi__id_pr__619B8048");
        });

        modelBuilder.Entity<Editorial>(entity =>
        {
            entity.HasKey(e => e.IdEditorial).HasName("PK__Editoria__BCB52C78D55B6B81");

            entity.Property(e => e.BorradoLogico)
            .HasColumnName("BorradoLogico")
            .HasDefaultValue(false)
            .IsRequired();
        });

        modelBuilder.Entity<EncabezadoTicket>(entity =>
        {
            entity.HasKey(e => e.IdEncabezadoTicket).HasName("PK__Encabeza__B625213B0F71DB00");

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.EncabezadoTickets).HasConstraintName("FK__Encabezad__id_di__6E01572D");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.EncabezadoTickets).HasConstraintName("FK__Encabezad__id_Us__6EF57B66");
        });

        modelBuilder.Entity<EstadoLibro>(entity =>
        {
            entity.HasKey(e => e.IdEstadoLibro).HasName("PK__EstadoLi__113161B48D1A76AB");
        });

        modelBuilder.Entity<EstadoRegreso>(entity =>
        {
            entity.HasKey(e => e.IdEstadoRegreso).HasName("PK__EstadoRe__70B3D36FEA1C8D34");
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.IdGeneros).HasName("PK__Generos__514744118BB5E1A4");
        });

        modelBuilder.Entity<ImagenesTicket>(entity =>
        {
            entity.HasKey(e => e.IdImagenes).HasName("PK__Imagenes__646A4D6ABDE8DBE7");
        });

        modelBuilder.Entity<ImgLibro>(entity =>
        {
            entity.HasKey(e => e.IdImgLibros).HasName("PK__ImgLibro__82B8193B3AF69025");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.IdLibro).HasName("PK__Libros__7813660F7E4FB851");

            //entity.HasOne(d => d.IdAutores1).WithMany(p => p.Libros).HasConstraintName("FK__Libros__id_Autor__5070F446");

            entity.HasOne(d => d.IdCategoriasNavigation).WithMany(p => p.Libros).HasConstraintName("FK__Libros__id_Categ__534D60F1");

            entity.HasOne(d => d.IdEditorialNavigation).WithMany(p => p.Libros).HasConstraintName("FK__Libros__ID_Edito__52593CB8");

            entity.HasOne(d => d.IdGenerosNavigation).WithMany(p => p.Libros).HasConstraintName("FK__Libros__id_Gener__5165187F");

            entity.HasOne(l => l.IdImgLibrosNavigation)
                 .WithMany(i => i.Libros)
                 .HasForeignKey(l => l.IdImgLibros)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK__Libros__id_imgLi__5441852A");

            entity.HasOne(d => d.IdUbicacionNavigation).WithMany(p => p.Libros).HasConstraintName("FK__Libros__id_Ubica__5535A963");

            entity.HasOne(d => d.IdDisponibilidadNavigation).WithMany(p => p.Libros).HasForeignKey(d => d.IdDisponibilidad).HasConstraintName("FK_Libros_Disponibilidad");

            entity.Property(e => e.BorradoLogico)
            .HasColumnName("BorradoLogico")
            .HasDefaultValue(false)
            .IsRequired();
        });

        modelBuilder.Entity<Multa>(entity =>
        {
            entity.HasKey(e => e.IdMulta).HasName("PK__Multas__295650BB45A04993");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.Multa).HasConstraintName("FK__Multas__id_prest__6A30C649");

            entity.HasOne(d => d.IdTipomultaNavigation).WithMany(p => p.Multa).HasConstraintName("FK__Multas__ID_Tipom__6B24EA82");
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.IdPrestamo).HasName("PK__Prestamo__5E87BE2715DD5D51");

            entity.HasOne(d => d.IdClientesNavigation).WithMany(p => p.Prestamos).HasConstraintName("FK__Prestamos__id_cl__5BE2A6F2");

            entity.HasOne(d => d.IdEstadoLibroNavigation).WithMany(p => p.Prestamos).HasConstraintName("FK__Prestamos__id_Es__5DCAEF64");

            entity.HasOne(d => d.IdLibroNavigation).WithMany(p => p.Prestamos).HasConstraintName("FK__Prestamos__id_li__5CD6CB2B");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Prestamos).HasConstraintName("FK__Prestamos__id_us__5EBF139D");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__6ABCB5E0B739ACF5");
        });

        modelBuilder.Entity<TicketMultum>(entity =>
        {
            entity.HasKey(e => e.IdTicketMulta).HasName("PK__TicketMu__C7DB3F577E1D3FF0");

            entity.HasOne(d => d.IdEncabezadoTicketNavigation).WithMany(p => p.TicketMulta).HasConstraintName("FK__TicketMul__id_En__73BA3083");

            entity.HasOne(d => d.IdImagenesNavigation).WithMany(p => p.TicketMulta).HasConstraintName("FK__TicketMul__id_Im__76969D2E");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.TicketMulta).HasConstraintName("FK__TicketMul__id_Pr__75A278F5");

            entity.HasOne(d => d.IdTipoMultaNavigation).WithMany(p => p.TicketMulta).HasConstraintName("FK__TicketMul__id_Ti__74AE54BC");
        });

        modelBuilder.Entity<TicketPrestamo>(entity =>
        {
            entity.HasKey(e => e.IdTicketPrestamo).HasName("PK__Ticket_P__1B558CEEA2CB1BCA");

            entity.HasOne(d => d.IdEncabezadoTicketNavigation).WithMany(p => p.TicketPrestamos).HasConstraintName("FK__Ticket_Pr__id_En__797309D9");

            entity.HasOne(d => d.IdImagenesNavigation).WithMany(p => p.TicketPrestamos).HasConstraintName("FK__Ticket_Pr__id_Im__7B5B524B");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.TicketPrestamos).HasConstraintName("FK__Ticket_Pr__id_Pr__7A672E12");
        });

        modelBuilder.Entity<TipoMultum>(entity =>
        {
            entity.HasKey(e => e.IdTipomulta).HasName("PK__TipoMult__A33A05C5DA4D2EFB");
        });

        modelBuilder.Entity<Ubicacione>(entity =>
        {
            entity.HasKey(e => e.IdUbicacion).HasName("PK__Ubicacio__81BAA5915BB0B892");

            entity.Property(e => e.BorradoLogico)
           .HasColumnName("BorradoLogico")
           .HasDefaultValue(false)
           .IsRequired();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__4E3E04ADFCB2806E");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios).HasConstraintName("FK__Usuarios__id_rol__4AB81AF0");
        });

        modelBuilder.Entity<LibroAutor>(entity =>
        {
            entity.HasKey(e => new { e.IdLibro, e.IdAutor }); // Clave compuesta

            entity.ToTable("Autores_Libros");

            entity.Property(e => e.IdLibro).HasColumnName("id_Libro");
            entity.Property(e => e.IdAutor).HasColumnName("Id_Autores");

            entity.HasOne(d => d.Libro)
                .WithMany(p => p.LibroAutores)
                .HasForeignKey(d => d.IdLibro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Autores_L__id_Li__59063A47");

            entity.HasOne(d => d.Autor)
                .WithMany(p => p.LibroAutores)
                .HasForeignKey(d => d.IdAutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Autores_L__Id_Au__5812160E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
