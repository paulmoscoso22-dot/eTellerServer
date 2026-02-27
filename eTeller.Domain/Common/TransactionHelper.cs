namespace eTeller.Domain.Common
{
    public static class TransactionHelper
    {
        public static string GetHostTrace(string cassa, DateTime datOpe, string dailySeq)
        {
            return string.Concat(cassa, datOpe.ToString("yyMMddHHmm"), dailySeq).ToUpper();
        }
    }
}
