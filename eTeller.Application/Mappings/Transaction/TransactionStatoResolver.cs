using AutoMapper;
using eTeller.Domain.Common;
using CurModel = eTeller.Domain.Models;

namespace eTeller.Application.Mappings.Transaction
{
    public class TransactionStatoResolver : IValueResolver<CurModel.Transaction, TransactionGiornaleCassaVm, string?>
    {
        public string? Resolve(CurModel.Transaction source, TransactionGiornaleCassaVm destination, string? destMember, ResolutionContext context)
        {
            var statusParts = new List<string>();

            if (source.TrxOnline == false)
                statusParts.Add("Offline");
            else if (source.TrxReverse == true)
                statusParts.Add("Stornata");
            else
            {
                var baseStatus = source.TrxStatus switch
                {
                    TransactionStatusConstants.StatoNonTrasmesso => "Non trasm.",
                    TransactionStatusConstants.StatoAnnullato => "Annullata",
                    TransactionStatusConstants.StatoAttesaBEF => "Attesa BEF",
                    _ => string.Empty
                };
                if (!string.IsNullOrEmpty(baseStatus))
                    statusParts.Add(baseStatus);
            }

            if (source.TrxFlgForced == true)
                statusParts.Add("Forzata");

            return statusParts.Count > 0 ? string.Join(", ", statusParts) : null;
        }
    }
}
