namespace eTeller.Application.Mappings.Account
{
    public class CustomerAccountVm
    {
        public string AccId { get; set; }
        public string AccCliId { get; set; }
        public string AccNea { get; set; }
        public string AccType { get; set; }
        public string AccDivisa { get; set; }
        public string AccCategoria { get; set; }
        public string AccRubrica { get; set; }
        public decimal AccSaldo { get; set; }
        public string AccStatus { get; set; }
        public DateTime? AccDatape { get; set; }
        public DateTime? AccDatest { get; set; }
    }
}
