using Pos.Models;
using Pos.UseCases.Ports;
using Pos.UseCases.Results;

namespace Pos.Infrastructure.Export;

public sealed class CsvExportStrategy : IResultExportStrategy
{
    public ResultExportFormat Format => ResultExportFormat.Csv;

    public void Write(FinalResult result, string path)
    {
        using var file = new FileStream(path, FileMode.CreateNew);
        using var writer = new StreamWriter(file);
        writer.WriteLine("StudentId,Percent");
        writer.WriteLine(FormattableString.Invariant($"{result.StudentId},{result.Percent}"));
    }
}
