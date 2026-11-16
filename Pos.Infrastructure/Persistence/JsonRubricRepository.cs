using Pos.Infrastructure.Persistence.Dtos;
using Pos.Models;
using Pos.UseCases.Ports;

namespace Pos.Infrastructure.Persistence;

public sealed class JsonRubricRepository(JsonDataStore store) : IRubricRepository
{
    //загружает данные из хранилища, восстанавливает из них объект типа Rubric и возвращает его вызывающему коду
    public Rubric Get() => Restore(store.Load());

    public void Save(Rubric rubric)
    {
        var data = store.Load();
        var criteria = rubric.Criteria
            .Select(criterion => new CriterionDto(criterion.Id, criterion.Name, criterion.Max, criterion.Weight))
            .ToList();

        store.Save(data with { RubricId = rubric.Id, RubricName = rubric.Name, Criteria = criteria });
    }

    internal static Rubric Restore(StorageDto data)
    {
        var rubric = new Rubric(data.RubricId, data.RubricName);
        foreach (var criterion in data.Criteria)
            rubric.AddCriterion(new Criterion(criterion.Id, criterion.Name, criterion.Max, criterion.Weight));

        return rubric;
    }
}
