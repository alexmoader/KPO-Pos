namespace Pos.Infrastructure.Persistence.Dtos;

internal sealed record StudentDto(Guid Id, string Name, List<EvaluationDto> Evaluations);
