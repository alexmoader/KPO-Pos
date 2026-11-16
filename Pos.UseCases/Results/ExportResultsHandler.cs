using Pos.UseCases.Ports;

namespace Pos.UseCases.Results;

// Реализация паттерна стратегия

public sealed class ExportResultsHandler(
    GetFinalResultHandler results,
    IEnumerable<IResultExportStrategy> strategies)
{
    private readonly Dictionary<ResultExportFormat, IResultExportStrategy> _strategies =
        strategies.ToDictionary(strategy => strategy.Format);

    public void Handle(Guid studentId, ResultExportFormat format, string path)
    {
        if (!_strategies.TryGetValue(format, out var strategy))
            throw new ArgumentException("Доступные форматы экспорта: csv и json.", nameof(format));

        var result = results.Handle(studentId);
        strategy.Write(result, path);
    }
}
