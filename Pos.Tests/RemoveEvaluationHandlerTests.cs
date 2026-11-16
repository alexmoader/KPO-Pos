using Pos.Models;
using Pos.UseCases.Ports;
using Xunit;

namespace Pos.Tests;

public sealed class RemoveEvaluationHandlerTests
{
    // Первая отмена сохраняет студента без оценки. Повторная отмена ничего не записывает.
    [Fact]
    public void RemovesEvaluationAndSavesOnlyOnce()
    {
        var student = new Student(Guid.NewGuid(), "Иван");
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var criterion = new Criterion(Guid.NewGuid(), "Практика", 10, 1);
        rubric.AddCriterion(criterion);
        Assert.True(student.SetEvaluation(rubric, criterion.Id, 8));
        var repository = new RecordingRepository(student);
        var handler = new RemoveEvaluationHandler(repository);

        Assert.True(handler.Handle(student.Id, criterion.Id));
        Assert.False(handler.Handle(student.Id, criterion.Id));

        Assert.Null(student.GetEvaluation(criterion.Id));
        Assert.Equal(0, Assert.Single(repository.SavedEvaluationCounts));
        Assert.Equal(0m, new WeightedResultCalculator().Calculate(student, rubric).Percent);
    }

    private sealed class RecordingRepository(Student student) : IStudentRepository
    {
        public List<int> SavedEvaluationCounts { get; } = [];

        public Student? GetById(Guid id) => id == student.Id ? student : null;
        public IReadOnlyList<Student> List() => [student];
        public void Save(Student saved) => SavedEvaluationCounts.Add(saved.Evaluations.Count);
    }
}
