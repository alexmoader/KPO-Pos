using Pos.UseCases.Ports;


// добавлен в Student метод bool RemoveEvaluation(Guid criterionId) (надо покрыть тестом) 
// обработчик загружает студента, вызывает метод и сохраняет изменения, 
// только если оценка действительно удалена.

public sealed class RemoveEvaluationHandler
{
    private readonly IStudentRepository _students;

    public RemoveEvaluationHandler(IStudentRepository students)
    {
        _students = students;
    }

    public bool Handle(Guid studentId, Guid criterionId)
    {
        var student = _students.GetById(studentId)
            ?? throw new KeyNotFoundException($"Student with ID {studentId} was not found.");

        if (!student.RemoveEvaluation(criterionId))
            return false;

        _students.Save(student);
        return true;
    }
}