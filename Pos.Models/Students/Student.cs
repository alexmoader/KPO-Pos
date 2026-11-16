namespace Pos.Models;

// Aggregate root representing a Student.


// sealed <=> final (C++)
public sealed class Student
{
    public Guid Id { get; }
    public string Name { get; }

    // Коллекция оценок студента, где ключом является идентификатор критерия, а значением - оценка.
    private readonly Dictionary<Guid, Evaluation> _evaluations = new();
    public IReadOnlyCollection<Evaluation> Evaluations => _evaluations.Values;

    public Student(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Student name cannot be empty", nameof(name));

        Id = id;
        Name = name.Trim(); //в C# используется для удаления пробелов и других указанных символов с начала и конца строки
    }

    public bool SetEvaluation(Rubric rubric, Guid criterionId, int score)
    {
        var evaluation = EvaluationFactory.CreateEvaluation(rubric, criterionId, score);
        if (evaluation is null)
            return false;

        _evaluations[criterionId] = evaluation;
        return true;
    }

    public Evaluation? GetEvaluation(Guid criterionId)
    {
        return _evaluations.GetValueOrDefault(criterionId);
    }

    public bool RemoveEvaluation(Guid criterionId)
    {
        return _evaluations.Remove(criterionId);
    }
}
