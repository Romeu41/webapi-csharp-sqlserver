using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplePeopleApi.Models
{
    [Table("pessoas")]
    public class Pessoa
    {
        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [Required]
        [MaxLength(250)]
        [Column("nome", TypeName = "varchar(250)")]
        public string Nome { get; set; } = null!;

        // CPF: stored as string to preserve leading zeros and avoid integer overflow (11 digits)
        [Required]
        [MaxLength(11)]
        [Column("cpf", TypeName = "varchar(11)")]
        public string CPF { get; set; } = null!;

        [Required]
        [Column("dataDeNascimento")]
        public DateTime DataDeNascimento { get; set; }

        [Required]
        [Column("dataDeCriacao")]
        public DateTime DataDeCriacao { get; set; }

        [MaxLength(2)]
        [Column("uf", TypeName = "varchar(2)")]
        public string? UF { get; set; }
    }
}
