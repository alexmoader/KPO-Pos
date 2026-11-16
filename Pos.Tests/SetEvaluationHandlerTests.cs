using Pos.Models;
using Pos.UseCases.Evaluations;
using Pos.UseCases.Ports;
using Xunit;

namespace Pos.Tests;

public sealed class SetEvaluationHandlerTests
{
    private readonly Student _student = new(Guid.NewGuid(), "Иван");
    private readonly Criterion _criterion = new(Guid.NewGuid(), "Практика", 10, 1);
    private readonly RecordingStudentRepository _students;
    private readonly SetEvaluationHandler _handler;

    public SetEvaluationHandlerTests()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(_criterion);
        _students = new RecordingStudentRepository(_student);
        _handler = new SetEvaluationHandler(_students, new FixedRubricRepository(rubric));
    }

    // Репозиторий получает студента с уже выставленной оценкой.
    [Fact]
    public void SavesStudentAfterAcceptedEvaluation()
    {
        Assert.True(_handler.Handle(_student.Id, _criterion.Id, 8));

        Assert.Equal((_student.Id, (int?)8), Assert.Single(_students.Saves));
    }

    // Когда модель отклоняет балл, сценарий пропускает сохранение.
    [Fact]
    public void DoesNotSaveRejectedEvaluation()
    {
        Assert.False(_handler.Handle(_student.Id, _criterion.Id, 12));

        Assert.Empty(_students.Saves);
    }

    // Отсутствующий студент обрабатывается отдельно от недопустимого балла.
    [Fact]
    public void ThrowsWhenStudentIsMissing()
    {
        Assert.Throws<KeyNotFoundException>(
            () => _handler.Handle(Guid.NewGuid(), _criterion.Id, 8));

        Assert.Empty(_students.Saves);
    }

    private sealed class RecordingStudentRepository(Student student) : IStudentRepository
    {
        public List<(Guid StudentId, int? Score)> Saves { get; } = [];

        public Student? GetById(Guid id) => id == student.Id ? student : null;

        public IReadOnlyList<Student> List() => [student];

        public void Save(Student saved)
        {
            int? score = Assert.Single(saved.Evaluations).Score;
            Saves.Add((saved.Id, score));
        }
    }

    private sealed class FixedRubricRepository(Rubric rubric) : IRubricRepository
    {
        public Rubric Get() => rubric;

        public void Save(Rubric changed) => throw new NotSupportedException();
    }
}
