namespace Pos.Models;

//Rubric хранит критерии и умеет добавлять их и искать по ID.

public sealed class Rubric
{
    public Guid Id { get; }
    public string Name { get; }
    // Коллекция критериев, хранящаяся в виде списка.
    private readonly List<Criterion> _criteria = new List<Criterion>();

    // Возвращает список критериев в виде IReadOnlyList, чтобы предотвратить изменение коллекции извне.
    public IReadOnlyList<Criterion> Criteria => _criteria.AsReadOnly();

    public Rubric(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Rubric ID cannot be empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Rubric name cannot be empty", nameof(name));

        Id = id;
        Name = name.Trim();
    }

    public void AddCriterion(Criterion criterion)
    {
        if (criterion == null)
            throw new ArgumentNullException(nameof(criterion));

        if (GetCriterionById(criterion.Id) is not null)
            throw new InvalidOperationException("A criterion with this ID already exists.");

        _criteria.Add(criterion); // Критерий уже с инвариантами. 
    }

    // ? нужен для nullable, чтобы вернуть null, если критерий не найден
    public Criterion? GetCriterionById(Guid criterionId)
    {
        return _criteria.FirstOrDefault(c => c.Id == criterionId);
    }
}