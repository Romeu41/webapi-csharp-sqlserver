using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplePeopleApi.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [Required]
        [MaxLength(250)]
        [Column("Nome", TypeName = "varchar(250)")]
        public string Nome { get; set; } = null!;

    [Required]
    [MaxLength(250)]
    [Column("Senha", TypeName = "varchar(250)")]
    public string Senha { get; set; } = null!;

        [Required]
        [Column("DataCriacao")]
        public DateTime DataCriacao { get; set; }
    }
}
