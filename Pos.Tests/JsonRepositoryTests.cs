using Pos.Infrastructure.Persistence;
using Pos.Models;
using Xunit;

namespace Pos.Tests;

public sealed class JsonRepositoryTests
{
    // Новые репозитории восстанавливают состояние из файла и получают тот же итог.
    [Fact]
    public void RestoresRubricAndStudentEvaluationsFromJson()
    {
        var directory = Directory.CreateTempSubdirectory("pos-repository-test-");
        try
        {
            string path = Path.Combine(directory.FullName, "data.json");
            var store = new JsonDataStore(path);
            var rubrics = new JsonRubricRepository(store);
            var students = new JsonStudentRepository(store);
            var rubric = rubrics.Get();
            var practice = new Criterion(Guid.NewGuid(), "Практика", 10, 1);
            var project = new Criterion(Guid.NewGuid(), "Проект", 20, 3);
            rubric.AddCriterion(practice);
            rubric.AddCriterion(project);
            rubrics.Save(rubric);
            var student = new Student(Guid.NewGuid(), "Иван");
            Assert.True(student.SetEvaluation(rubric, practice.Id, 8));
            Assert.True(student.SetEvaluation(rubric, project.Id, 10));
            students.Save(student);

            var reopened = new JsonDataStore(path);
            var restoredRubric = new JsonRubricRepository(reopened).Get();
            var restoredStudent = Assert.IsType<Student>(
                new JsonStudentRepository(reopened).GetById(student.Id));

            Assert.Equal(rubric.Id, restoredRubric.Id);
            Assert.Equal(2, restoredRubric.Criteria.Count);
            Assert.Equal(student.Name, restoredStudent.Name);
            Assert.Equal(8, restoredStudent.GetEvaluation(practice.Id)?.Score);
            Assert.Equal(10, restoredStudent.GetEvaluation(project.Id)?.Score);
            Assert.Equal(57.5m, new WeightedResultCalculator().Calculate(restoredStudent, restoredRubric).Percent);
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }
}
