using Pos.UseCases.Ports;
using Pos.Models;

namespace Pos.UseCases.Evaluations;

// Загружает студента по ID и возвращает его оценки. 
// Модель уже открывает коллекцию для чтения.

public sealed class ListEvaluationsHandler
{
    private readonly IStudentRepository _students;

    public ListEvaluationsHandler(IStudentRepository students)
    {
        _students = students;
    }

    public IReadOnlyCollection<Evaluation> Handle(Guid studentId)
    {
        var student = _students.GetById(studentId)
            ?? throw new KeyNotFoundException($"Student with ID {studentId} was not found.");

        return student.Evaluations;
    }
}