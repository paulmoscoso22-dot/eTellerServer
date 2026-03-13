namespace eTeller.Application.Share;

public enum TipoOperazioneEnum
{
    DEP,    // Versamento
    WITH    // Prelevamento
}

public enum TipoAggioEnum
{
    Percentuale = 0,
    Importo = 1
}

public enum SegnoAggioEnum
{
    Aggio = 1,
    Disaggio = -1
}
