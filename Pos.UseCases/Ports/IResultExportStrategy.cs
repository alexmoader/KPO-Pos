using Pos.Models;
using Pos.UseCases.Results;

namespace Pos.UseCases.Ports;

public interface IResultExportStrategy
{
    ResultExportFormat Format { get; }
    void Write(FinalResult result, string path);
}
