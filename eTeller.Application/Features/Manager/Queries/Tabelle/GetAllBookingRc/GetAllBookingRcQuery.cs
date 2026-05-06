using eTeller.Application.Mappings.BookingRc;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetAllBookingRc
{
    public record GetAllBookingRcQuery(string? BrcCutId, string? BrcOptId, string? BrcActId) : IRequest<IEnumerable<BookingRcVm>>;
}
