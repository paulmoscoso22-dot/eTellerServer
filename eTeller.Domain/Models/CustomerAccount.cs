using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("CUSTOMER_ACCOUNTS")]
    public class CustomerAccount
    {
        [Column("ACC_ID")]
        public string AccId { get; set; }

        [Column("ACC_CLI_ID")]
        public string AccCliId { get; set; }

        [Column("ACC_NEA")]
        public string AccNea { get; set; }

        [Column("ACC_TYPE")]
        public string AccType { get; set; }

        [Column("ACC_DIVISA")]
        public string AccDivisa { get; set; }

        [Column("ACC_CATEGORIA")]
        public string AccCategoria { get; set; }

        [Column("ACC_RUBRICA")]
        public string AccRubrica { get; set; }

        [Column("ACC_SALDO")]
        public decimal AccSaldo { get; set; }

        [Column("ACC_STATUS")]
        public string AccStatus { get; set; }

        [Column("ACC_DATAPE")]
        public DateTime? AccDatape { get; set; }

        [Column("ACC_DATEST")]
        public DateTime? AccDatest { get; set; }
    }
}
