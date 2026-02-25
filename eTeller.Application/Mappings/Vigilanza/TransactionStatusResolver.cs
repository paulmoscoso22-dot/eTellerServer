using AutoMapper;
using eTeller.Domain.Common;
using eTeller.Domain.Models.StoredProcedure;

namespace eTeller.Application.Mappings.Vigilanza
{
    public class TransactionStatusResolver : IValueResolver<SpTransactionGiornaleAntiriciclagio, SpTransactionGiornaleAntiriciclagioVm, string?>
    {
        public string? Resolve(SpTransactionGiornaleAntiriciclagio source, SpTransactionGiornaleAntiriciclagioVm destination, string? destMember, ResolutionContext context)
        {
            return GetStatus(source);
        }

        private static string GetStatus(SpTransactionGiornaleAntiriciclagio row)
        {
            var statusParts = new List<string>();
            
            // Determine base status
            var baseStatus = GetBaseStatus(row);
            if (!string.IsNullOrEmpty(baseStatus))
                statusParts.Add(baseStatus);
            
            // Add forced flags
            //Mantis 0000223
            if (row.TrxFlgForced == true)
                statusParts.Add("Forzata");
            
            // Mantis 389
            if (row.ArcForced)
                statusParts.Add("Vig. forzata");
            
            return string.Join(", ", statusParts);
        }

        private static string GetBaseStatus(SpTransactionGiornaleAntiriciclagio row)
        {
            if (row.TrxOnline == false)
                return "Offline";
            
            if (row.TrxReverse == true)
                return "Stornata";
            
            return row.TrxStatus switch
            {
                TransactionStatusConstants.StatoNonTrasmesso => "Non trasm.",
                TransactionStatusConstants.StatoAnnullato => "Annullata",
                TransactionStatusConstants.StatoAttesaBEF => "Attesa BEF",
                _ => string.Empty
            };
        }
    }
}
