using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("FUNZIONISCEDULE")]
    public class FUNZIONISCEDULE
    {
        [Column("FUT_ID")]
        public string FutId { get; set; }

        [Column("FUT_ACTIVE")]
        public bool FutActive { get; set; }

        [Column("FUT_DES")]
        public string FutDes { get; set; }

        [Column("FUT_FUNNAME")]
        public string FutFunname { get; set; }

        [Column("FUT_SCRIPTNAME")]
        public string FutScriptname { get; set; }

        [Column("FUT_TIMEOUT")]
        public int FutTimeout { get; set; }

        [Column("FUT_AUTATT")]
        public bool? FutAutatt { get; set; }

        [Column("FUT_OFFLINE")]
        public bool? FutOffline { get; set; }

        [Column("FUT_HOSVAL")]
        public bool? FutHosval { get; set; }

        [Column("FUT_ONETIMERUN")]
        public bool? FutOnetimerun { get; set; }

        [Column("FUT_PERIODTYP")]
        public string? FutPeriodtyp { get; set; }

        [Column("FUT_PERIOD")]
        public int? FutPeriod { get; set; }

        [Column("FUT_START")]
        public string? FutStart { get; set; }

        [Column("FUT_END")]
        public string? FutEnd { get; set; }

        [Column("FUT_LASTRUN")]
        public DateTime? FutLastrun { get; set; }

        [Column("FUT_LASTRUNOK")]
        public bool? FutLastrunok { get; set; }

        [Column("FUT_LOOP")]
        public bool? FutLoop { get; set; }

        [Column("FUT_NAMEDLL")]
        public string? FutNamedll { get; set; }

        [Column("FUT_CLASSNAME")]
        public string? FutClassname { get; set; }

        [Column("FUT_DATMOD")]
        public DateTime? FutDatmod { get; set; }

        [Column("FUT_DATINS")]
        public DateTime? FutDatins { get; set; }

        [Column("FUT_ERRCOUNT")]
        public int? FutErrcount { get; set; }

        [Column("FUT_TRACE")]
        public bool? FutTrace { get; set; }
    }
}
