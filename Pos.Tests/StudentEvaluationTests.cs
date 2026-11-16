using Pos.Models;
using Xunit;

namespace Pos.Tests;

public sealed class StudentEvaluationTests
{
    private readonly Student _student = new(Guid.NewGuid(), "Иван");
    private readonly Rubric _rubric = new(Guid.NewGuid(), "КПО");
    private readonly Criterion _criterion = new(Guid.NewGuid(), "Практика", 10, 1);

    public StudentEvaluationTests()
    {
        _rubric.AddCriterion(_criterion);
    }

    // Ноль и максимум входят в допустимый диапазон. Начальные 8 баллов позволяют проверить замену оценки.
    [Theory] // позволяет запускать один и тот же тест с разными входными данными
    [InlineData(0)]
    [InlineData(10)]
    public void AcceptsBoundaryScoresAndReplacesPreviousScore(int score)
    {
        Assert.True(_student.SetEvaluation(_rubric, _criterion.Id, 8));

        Assert.True(_student.SetEvaluation(_rubric, _criterion.Id, score));

        Assert.Equal(score, _student.GetEvaluation(_criterion.Id)!.Score);
    }

    // Балл ниже нуля или выше максимума отклоняется, а ранее выставленные 8 баллов сохраняются.
    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void RejectsOutOfRangeScoreAndKeepsPreviousScore(int score)
    {
        Assert.True(_student.SetEvaluation(_rubric, _criterion.Id, 8));

        Assert.False(_student.SetEvaluation(_rubric, _criterion.Id, score));

        Assert.Equal(8, _student.GetEvaluation(_criterion.Id)!.Score);
    }

    // Даже допустимый по величине балл нельзя записать по критерию, которого нет в рубрикаторе.
    [Fact]
    public void RejectsCriterionOutsideTheRubric()
    {
        var unknownId = Guid.NewGuid();

        Assert.False(_student.SetEvaluation(_rubric, unknownId, 8));

        Assert.Null(_student.GetEvaluation(unknownId));
    }

    // Критерий с тем же ID и другими данными отклоняется. В рубрикаторе остаётся исходный критерий.
    [Fact]
    public void RubricRejectsDuplicateCriterionId()
    {
        var duplicate = new Criterion(_criterion.Id, "Проект", 20, 3);

        Assert.Throws<InvalidOperationException>(() => _rubric.AddCriterion(duplicate));

        Assert.Same(_criterion, Assert.Single(_rubric.Criteria));
    }
}
