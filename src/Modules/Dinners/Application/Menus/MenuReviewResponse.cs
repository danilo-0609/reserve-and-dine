namespace Dinners.Application.Menus;

public sealed record MenuReviewResponse(Guid MenuReviewId,
    decimal Rate,
    Guid ClientId,
    string Comment,
    DateTime ReviewedAt,
    DateTime? UpdatedAt);
