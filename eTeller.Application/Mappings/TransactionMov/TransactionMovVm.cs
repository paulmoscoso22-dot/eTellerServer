namespace eTeller.Application.Mappings
{
    public class TransactionMovVm
    {
        public int TrmId { get; set; }

        public int TrmTrxId { get; set; }

        public string TrmMovtyp { get; set; }

        public string TrmBranch { get; set; }

        public string TrmAcctype { get; set; }

        public string TrmAccount { get; set; }

        public string TrmAcccur { get; set; }

        public decimal? TrmAmount { get; set; }

        public decimal? TrmAmtctv { get; set; }

        public DateTime? TrmValdat { get; set; }

        public DateTime? TrmDatcre { get; set; }

        public string TrmReacod { get; set; }

        public string TrmFreetx1 { get; set; }

        public string TrmFreetx2 { get; set; }

        public string TrmCoscen { get; set; }

        public string TrmHostcod { get; set; }

        public bool? TrmUpdpos { get; set; }
    }
}
