using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Application.Mappings
{
    public class AccountVm
    {
        public int IacId { get; set; }

        public string IacAccId { get; set; }

        public string IacCutId { get; set; }

        public string IacCurId { get; set; }

        public string IacActId { get; set; }

        public string IacBraId { get; set; }

        public string IacDes { get; set; }

        public string IacCliCassa { get; set; }

        public string IacHostprefix { get; set; }
    }
}
