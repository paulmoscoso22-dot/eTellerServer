using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class Transaction
    {
        [Column("TRX_ID")]
        public int TrxId { get; set; }

        [Column("TRX_BRA_ID")]
        public string TrxBraId { get; set; }

        [Column("TRX_CASSA")]
        public string TrxCassa { get; set; }

        [Column("TRX_DATOPE")]
        public DateTime? TrxDatope { get; set; }

        [Column("TRX_DAILYSEQUENCE")]
        public string TrxDailySequence { get; set; }

        [Column("TRX_APT_ID")]
        public string TrxAptId { get; set; }

        [Column("TRX_OPT_ID")]
        public string TrxOptId { get; set; }

        [Column("TRX_CUT_ID")]
        public string TrxCutId { get; set; }

        [Column("TRX_CASH")]
        public bool? TrxCash { get; set; }

        [Column("TRX_USR_ID")]
        public string TrxUsrId { get; set; }

        [Column("TRX_REVERSE")]
        public bool? TrxReverse { get; set; }

        [Column("TRX_REVTRX_ID")]
        public int? TrxRevtrxId { get; set; }

        [Column("TRX_ONLINE")]
        public bool? TrxOnline { get; set; }

        [Column("TRX_EXCRAT")]
        public decimal? TrxExcrat { get; set; }

        [Column("TRX_EXROPEBAS")]
        public decimal? TrxExropebas { get; set; }

        [Column("TRX_EXRCTPBAS")]
        public decimal? TrxExrctpbas { get; set; }

        [Column("TRX_DIVOPE")]
        public string TrxDivope { get; set; }

        [Column("TRX_IMPOPE")]
        public decimal? TrxImpope { get; set; }

        [Column("TRX_IMPCTV")]
        public decimal? TrxImpctv { get; set; }

        [Column("TRX_DIVCTP")]
        public string TrxDivctp { get; set; }

        [Column("TRX_IMPCTP")]
        public decimal? TrxImpctp { get; set; }

        [Column("TRX_CTPCTV")]
        public decimal? TrxCtpctv { get; set; }

        [Column("TRX_IMPAGIO")]
        public decimal? TrxImpagio { get; set; }

        [Column("TRX_IMPAGIOCTV")]
        public decimal? TrxImpagioctv { get; set; }

        [Column("TRX_IMPCOM")]
        public decimal? TrxImpcom { get; set; }

        [Column("TRX_IMPCOMCTV")]
        public decimal? TrxImpcomctv { get; set; }

        [Column("TRX_IMPSPE")]
        public decimal? TrxImpspe { get; set; }

        [Column("TRX_IMPSPECTV")]
        public decimal? TrxImpspectv { get; set; }

        [Column("TRX_IMPIVA")]
        public decimal? TrxImpiva { get; set; }

        [Column("TRX_IMPIVACTV")]
        public decimal? TrxImpivactv { get; set; }

        [Column("TRX_IMPRESTO")]
        public decimal? TrxImpresto { get; set; }

        [Column("TRX_IMPRESTOCTV")]
        public decimal? TrxImprestoctv { get; set; }

        [Column("TRX_DATVAL")]
        public DateTime? TrxDatval { get; set; }

        [Column("TRX_FLG_STAMPA_AVVISO")]
        public bool? TrxFlgStampaAvviso { get; set; }

        [Column("TRX_FLG_STAMPA_SALDO")]
        public bool? TrxFlgStampaSaldo { get; set; }

        [Column("TRX_FLG_STAMPA_INDIRIZZO")]
        public bool? TrxFlgStampaIndirizzo { get; set; }

        [Column("TRX_FLG_EMAIL")]
        public bool? TrxFlgEmail { get; set; }

        [Column("TRX_FINEZZA_ID")]
        public decimal? TrxFinezzaId { get; set; }

        [Column("TRX_TEXT1")]
        public string TrxText1 { get; set; }

        [Column("TRX_TEXT2")]
        public string TrxText2 { get; set; }

        [Column("TRX_TEXT3")]
        public string TrxText3 { get; set; }

        [Column("TRX_TEXT4")]
        public string TrxText4 { get; set; }

        [Column("TRX_MSG_SENT")]
        public string TrxMsgSent { get; set; }

        [Column("TRX_STATUS")]
        public int? TrxStatus { get; set; }

        [Column("TRX_CTOOPE")]
        public string TrxCtoope { get; set; }

        [Column("TRX_CTOOPETIP")]
        public string TrxCtoopetp { get; set; }

        [Column("TRX_CTOCTP")]
        public string TrxCtoctp { get; set; }

        [Column("TRX_CTOCTPTIP")]
        public string TrxCtoctptip { get; set; }

        [Column("TRX_METNET")]
        public decimal? TrxMetnet { get; set; }

        [Column("TRX_PRCMETOPE")]
        public decimal? TrxPrcmetope { get; set; }

        [Column("TRX_PRCMETCTP")]
        public decimal? TrxPrcmetctp { get; set; }

        [Column("TRX_IVAPER")]
        public decimal? TrxIvaper { get; set; }

        [Column("TRX_EXCRAT_PRT")]
        public decimal? TrxExcratPrt { get; set; }

        [Column("TRX_BEFHOST")]
        public string TrxBefhost { get; set; }

        [Column("TRX_BEFSTATUS")]
        public int? TrxBefstatus { get; set; }

        [Column("TRX_SALDO")]
        public decimal? TrxSaldo { get; set; }

        [Column("TRX_RUBRICA")]
        public string TrxRubrica { get; set; }

        [Column("TRX_ASSEGNO")]
        public string TrxAssegno { get; set; }

        [Column("TRX_CENCOS")]
        public string TrxCencos { get; set; }

        [Column("TRX_FLG_FORCED")]
        public bool? TrxFlgForced { get; set; }

        [Column("TRX_INT_COMMENT")]
        public string TrxIntComment { get; set; }

        [Column("TRX_FLG_PRINTED")]
        public bool? TrxFlgPrinted { get; set; }

        [Column("TRX_FLG_IS107_CUSTOMER")]
        public bool? TrxFlgIs107Customer { get; set; }

        [Column("TRX_FLG_IS107_OVERLIMIT")]
        public bool? TrxFlgIs107Overlimit { get; set; }

        [Column("TRX_NUMREL")]
        public string TrxNumrel { get; set; }
    }
}
