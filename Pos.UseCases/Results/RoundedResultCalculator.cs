using Pos.Models;


//RoundedResultCalculator оборачивает базовый калькулятор
//паттерн Декоратор, чтобы округлять результат до 2 знаков после запятой

public sealed class RoundedResultCalculator(IResultCalculator baseCalculator) : IResultCalculator
{
    public FinalResult Calculate(Student student, Rubric rubric)
    {
        var result = baseCalculator.Calculate(student, rubric);
        return new FinalResult(student.Id, Math.Round(result.Percent, 2));
    }
}