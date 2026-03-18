using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("CUSTOMERS")]
    public class Customers
    {
        [Column("CUS_CLI_ID")]
        public required string CusCliId { get; set; }

        [Column("CUS_NEA")]
        public string? CusNea { get; set; }

        [Column("CUS_NAME")]
        public required string CusName { get; set; }

        [Column("CUS_NAZIONE")]
        public required string CusNazione { get; set; }

        [Column("CUS_DOMICILIO")]
        public required string CusDomicilio { get; set; }

        [Column("CUS_LINGUA")]
        public required string CusLingua { get; set; }

        [Column("CUS_NATURA")]
        public required string CusNatura { get; set; }

        [Column("CUS_STATUS")]
        public required string CusStatus { get; set; }

        [Column("CUS_DATAPE")]
        public required DateTime CusDatape { get; set; }

        [Column("CUS_DATEST")]
        public DateTime? CusDatest { get; set; }

        [Column("CUS_RAPIMP")]
        public string? CusRapimp { get; set; }
    }
}
