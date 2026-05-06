using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.InsertBookingRc
{
    public record InsertBookingRcCommand(
        string BrcCutId,
        string BrcOptId,
        string BrcActId,
        string BrcCodcau,
        string BrcCodcausto,
        string? BrcText1,
        string? BrcText2,
        string TraUser,
        string TraStation) : IRequest<bool>;
}
