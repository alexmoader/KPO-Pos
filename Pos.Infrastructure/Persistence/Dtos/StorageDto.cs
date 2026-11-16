namespace Pos.Infrastructure.Persistence.Dtos;

internal sealed record StorageDto(
    Guid RubricId, string RubricName, List<CriterionDto> Criteria, List<StudentDto> Students);
