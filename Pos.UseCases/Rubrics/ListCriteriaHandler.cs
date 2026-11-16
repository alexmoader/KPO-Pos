using Pos.Models;
using Pos.UseCases.Ports;

//Для критериев достаточно IRubricRepository.Get().Criteria
//Тк у нас один рубрикатор

public sealed class ListCriteriaHandler(IRubricRepository rubrics)
{
    public IReadOnlyList<Criterion> Handle()
    {
        return rubrics.Get().Criteria;
    }
}