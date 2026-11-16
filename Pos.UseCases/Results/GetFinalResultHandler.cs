using Pos.Models;
using Pos.UseCases.Ports;

namespace Pos.UseCases.Results;

public sealed class GetFinalResultHandler(
    IStudentRepository students,
    IRubricRepository rubrics,
    IResultCalculator calculator)
{
    public FinalResult Handle(Guid studentId)
    {
        var student = students.GetById(studentId)
            ?? throw new KeyNotFoundException($"Student with ID {studentId} was not found.");

        return calculator.Calculate(student, rubrics.Get());
    }
}
