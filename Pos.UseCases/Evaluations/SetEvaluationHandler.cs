using Pos.UseCases.Ports;

namespace Pos.UseCases.Evaluations;

public sealed class SetEvaluationHandler
{
    private readonly IStudentRepository _students;
    private readonly IRubricRepository _rubrics;

    public SetEvaluationHandler(IStudentRepository students, IRubricRepository rubrics)
    {
        _students = students;
        _rubrics = rubrics;
    }

    public bool Handle(Guid studentId, Guid criterionId, int score)
    {
        var student = _students.GetById(studentId)
            ?? throw new KeyNotFoundException($"Student with ID {studentId} was not found.");

        var rubric = _rubrics.Get();
        if (!student.SetEvaluation(rubric, criterionId, score))
            return false;

        _students.Save(student);
        return true;
    }
}
