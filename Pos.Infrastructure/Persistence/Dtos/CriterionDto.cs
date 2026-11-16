namespace Pos.Infrastructure.Persistence.Dtos;

internal sealed record CriterionDto(Guid Id, string Name, int Max, int Weight);
