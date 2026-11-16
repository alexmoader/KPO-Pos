using System.Text.Json;
using Pos.Models;
using Pos.UseCases.Ports;
using Pos.UseCases.Results;

namespace Pos.Infrastructure.Export;

public sealed class JsonExportStrategy : IResultExportStrategy
{
    public ResultExportFormat Format => ResultExportFormat.Json;

    public void Write(FinalResult result, string path)
    {
        using var file = new FileStream(path, FileMode.CreateNew);
        JsonSerializer.Serialize(file, result, new JsonSerializerOptions { WriteIndented = true });
    }
}
