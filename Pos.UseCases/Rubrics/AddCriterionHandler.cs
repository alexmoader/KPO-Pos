using Pos.Models;
using Pos.UseCases.Ports;

namespace Pos.UseCases.Rubrics;

public sealed class AddCriterionHandler(IRubricRepository rubrics)
{
    public Criterion Handle(string name, int max, int weight)
    {
        var rubric = rubrics.Get();
        var criterion = new Criterion(Guid.NewGuid(), name, max, weight);
        rubric.AddCriterion(criterion);
        rubrics.Save(rubric);
        return criterion;
    }
}
