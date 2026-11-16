using Pos.Models;
using Xunit;

namespace Pos.Tests;

public sealed class WeightedResultCalculatorTests
{
    [Theory]
    // С проектом: (8/10 × 1 + 10/20 × 3) / 4 × 100 = 57,5%.
    [InlineData(10, 57.5)]
    // Без проекта: его вклад равен нулю, а вес остаётся в знаменателе. (8/10 × 1) / 4 × 100 = 20%.
    [InlineData(null, 20.0)]
    public void NormalizesScoresAndIncludesWeightsOfMissingEvaluations(int? projectScore, double expectedPercent)
    {
        var student = new Student(Guid.NewGuid(), "Иван");
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var practice = new Criterion(Guid.NewGuid(), "Практика", 10, 1);
        var project = new Criterion(Guid.NewGuid(), "Проект", 20, 3);
        rubric.AddCriterion(practice);
        rubric.AddCriterion(project);
        Assert.True(student.SetEvaluation(rubric, practice.Id, 8));
        if (projectScore is int score)
            Assert.True(student.SetEvaluation(rubric, project.Id, score));

        var result = new WeightedResultCalculator().Calculate(student, rubric);

        Assert.Equal(student.Id, result.StudentId);
        Assert.Equal((decimal)expectedPercent, result.Percent);
    }

    // У пустого рубрикатора сумма весов равна нулю, поэтому итог вычислить нельзя.
    [Fact]
    public void RejectsEmptyRubric()
    {
        var student = new Student(Guid.NewGuid(), "Иван");
        var rubric = new Rubric(Guid.NewGuid(), "КПО");

        Assert.Throws<InvalidOperationException>(() => new WeightedResultCalculator().Calculate(student, rubric));
    }
}
