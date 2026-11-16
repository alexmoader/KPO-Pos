using Pos.Models;

namespace Pos.UseCases.Ports;

public interface IRubricRepository
{
    Rubric Get();
    void Save(Rubric rubric);

}
