using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplePeopleApi.Models
{
    [Table("ContasPagas")]
    public class ContaPaga
    {
        [Key]
        [Column("Numero")]
        public long Numero { get; set; }

        [Required]
        [Column("CodigoFornecedor")]
        public int CodigoFornecedor { get; set; }

        [Required]
        [Column("DataVencimento")]
        public DateTime DataVencimento { get; set; }

        [Required]
        [Column("DataPagamento")]
        public DateTime DataPagamento { get; set; }

        [Required]
        [Column("Valor", TypeName = "decimal(18,6)")]
        public decimal Valor { get; set; }

        [Column("Acrescimo", TypeName = "decimal(18,6)")]
        public decimal? Acrescimo { get; set; }

        [Column("Desconto", TypeName = "decimal(18,6)")]
        public decimal? Desconto { get; set; }

        // Navigation
        public Pessoa? Fornecedor { get; set; }

        [MaxLength(250)]
        [Column("InseridoPor", TypeName = "varchar(250)")]
        public string? InseridoPor { get; set; }
    }
}
