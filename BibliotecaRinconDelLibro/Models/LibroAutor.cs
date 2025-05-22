using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaRinconDelLibro.Models
{
    public class LibroAutor
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Libro")]
        public int IdLibro { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Autor")]
        public int IdAutor { get; set; }

        [ForeignKey("IdLibro")]
        public virtual Libro Libro { get; set; }

        [ForeignKey("IdAutor")]
        public virtual Autore Autor { get; set; }
    }
}