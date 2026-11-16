using Pos.Models;
using Pos.UseCases.Ports;
using Pos.UseCases.Results;
using Xunit;

namespace Pos.Tests;

public sealed class ExportResultsHandlerTests
{
    // Только выбранная стратегия получает итог и путь к файлу.
    [Theory]
    [InlineData(ResultExportFormat.Csv)]
    [InlineData(ResultExportFormat.Json)]
    public void SendsResultAndPathOnlyToSelectedStrategy(ResultExportFormat format)
    {
        var student = new Student(Guid.NewGuid(), "Иван");
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var criterion = new Criterion(Guid.NewGuid(), "Практика", 10, 1);
        rubric.AddCriterion(criterion);
        Assert.True(student.SetEvaluation(rubric, criterion.Id, 8));

        var data = new TestRepository(student, rubric);
        var results = new GetFinalResultHandler(data, data, new WeightedResultCalculator());
        var csv = new RecordingStrategy(ResultExportFormat.Csv);
        var json = new RecordingStrategy(ResultExportFormat.Json);
        var handler = new ExportResultsHandler(results, [csv, json]);
        string path = format == ResultExportFormat.Csv ? "result.csv" : "result.json";

        handler.Handle(student.Id, format, path);

        var selected = format == ResultExportFormat.Csv ? csv : json;
        var other = format == ResultExportFormat.Csv ? json : csv;
        var write = Assert.Single(selected.Writes);
        Assert.Equal(student.Id, write.Result.StudentId);
        Assert.Equal(80m, write.Result.Percent);
        Assert.Equal(path, write.Path);
        Assert.Empty(other.Writes);
    }

    private sealed class RecordingStrategy(ResultExportFormat format) : IResultExportStrategy
    {
        public ResultExportFormat Format => format;
        public List<(FinalResult Result, string Path)> Writes { get; } = [];

        public void Write(FinalResult result, string path) => Writes.Add((result, path));
    }

    private sealed class TestRepository(Student student, Rubric rubric) : IStudentRepository, IRubricRepository
    {
        public Student? GetById(Guid id) => id == student.Id ? student : null;
        public IReadOnlyList<Student> List() => [student];
        public Rubric Get() => rubric;
        public void Save(Student changed) => throw new NotSupportedException();
        public void Save(Rubric changed) => throw new NotSupportedException();
    }
}
