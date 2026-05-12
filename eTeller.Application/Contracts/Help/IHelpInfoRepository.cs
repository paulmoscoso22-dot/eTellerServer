namespace eTeller.Application.Contracts.Help
{
    public interface IHelpInfoRepository
    {
        Task<string?> GetFichesPrinterAsync(string cliId);
        Task<bool> ExistsTwinSafeAsync(string cliId);
    }
}
