using Pos.Infrastructure.Persistence.Dtos;
using Pos.Models;
using Pos.UseCases.Ports;

namespace Pos.Infrastructure.Persistence;

public sealed class JsonStudentRepository(JsonDataStore store) : IStudentRepository
{
    public Student? GetById(Guid id)
    {
        var data = store.Load();
        var saved = data.Students.Find(student => student.Id == id);
        if (saved is null)
            return null;

        return Restore(saved, JsonRubricRepository.Restore(data));
    }

    public IReadOnlyList<Student> List()
    {
        var data = store.Load();
        var rubric = JsonRubricRepository.Restore(data);
        return data.Students.Select(saved => Restore(saved, rubric)).ToList();
    }

    private static Student Restore(StudentDto saved, Rubric rubric)
    {
        var student = new Student(saved.Id, saved.Name);
        foreach (var evaluation in saved.Evaluations)
        {
            if (student.GetEvaluation(evaluation.CriterionId) is not null ||
                !student.SetEvaluation(rubric, evaluation.CriterionId, evaluation.Score))
            {
                throw new InvalidDataException(
                    $"Invalid stored evaluation for student {saved.Id} and criterion {evaluation.CriterionId}.");
            }
        }

        return student;
    }

    public void Save(Student student)
    {
        var data = store.Load();
        var evaluations = student.Evaluations
            .Select(evaluation => new EvaluationDto(evaluation.CriterionId, evaluation.Score))
            .ToList();

        var saved = new StudentDto(student.Id, student.Name, evaluations);
        int index = data.Students.FindIndex(candidate => candidate.Id == student.Id);
        if (index < 0)
            data.Students.Add(saved);
        else
            data.Students[index] = saved;

        store.Save(data);
    }
}
