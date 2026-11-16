namespace Pos.Models;

// реализация интерфейса
// перебирает критерии, получает оценку через 
// student.GetEvaluation(criterion.Id) и 
// возвращает взвешенный процент

public class WeightedResultCalculator : IResultCalculator
{
    public FinalResult Calculate(Student student, Rubric rubric)
    {
        if (student is null)
            throw new ArgumentNullException(nameof(student));

        if (rubric is null)
            throw new ArgumentNullException(nameof(rubric));

        decimal totalWeightedScore = 0;
        decimal totalWeight = 0;

        foreach (var criterion in rubric.Criteria)
        {
            int score = student.GetEvaluation(criterion.Id)?.Score ?? 0;
            totalWeightedScore += (decimal)score / criterion.Max * criterion.Weight;
            totalWeight += criterion.Weight;
        }

        if (totalWeight == 0)
            throw new InvalidOperationException("Cannot calculate a result without criteria.");

        return new FinalResult(student.Id, totalWeightedScore / totalWeight * 100m);
    }
}
