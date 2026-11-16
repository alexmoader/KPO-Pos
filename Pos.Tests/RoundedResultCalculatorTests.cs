using Pos.Models;
using Xunit;

namespace Pos.Tests;

public sealed class RoundedResultCalculatorTests
{
    // Базовый калькулятор возвращает 66,666%, декоратор округляет до 66,67%.
    [Fact]
    public void RoundsPercentToTwoDecimalPlacesAndKeepsStudentId()
    {
        var student = new Student(Guid.NewGuid(), "Иван");
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var calculator = new RoundedResultCalculator(new FixedCalculator());

        var result = calculator.Calculate(student, rubric);

        Assert.Equal(66.67m, result.Percent);
        Assert.Equal(student.Id, result.StudentId);
    }

    private sealed class FixedCalculator : IResultCalculator
    {
        public FinalResult Calculate(Student student, Rubric rubric) => new(student.Id, 66.666m);
    }
}
