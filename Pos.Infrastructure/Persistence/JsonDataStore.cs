using System.Text.Json;
using Pos.Infrastructure.Persistence.Dtos;

namespace Pos.Infrastructure.Persistence;

public sealed class JsonDataStore(string path)
{
    private readonly string _path = Path.GetFullPath(path);
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    internal StorageDto Load()
    {
        try
        {
            using var file = File.OpenRead(_path);
            return JsonSerializer.Deserialize<StorageDto>(file, Options)
                ?? throw new InvalidDataException("The data file does not contain an application state.");
        }
        catch (FileNotFoundException)
        {
            return new StorageDto(Guid.NewGuid(), "КПО", [], []);
        }
        catch (DirectoryNotFoundException)
        {
            return new StorageDto(Guid.NewGuid(), "КПО", [], []);
        }
    }

    internal void Save(StorageDto data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temporaryPath = _path + "." + Guid.NewGuid() + ".tmp";
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(data, Options));
            File.Move(temporaryPath, _path, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }
}
