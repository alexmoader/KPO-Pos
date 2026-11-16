namespace Pos.Models;

// Фабрика получает рубрикатор, ID критерия и балл. 
// Она ищет критерий и создаёт оценку, 
// только если балл в диапазоне от 0 до максимума включительно.

public static class EvaluationFactory
{
    public static Evaluation? CreateEvaluation(Rubric rubric, Guid criterionId, int score)
    {
        if (rubric == null)
            throw new ArgumentNullException(nameof(rubric));

        var criterion = rubric.GetCriterionById(criterionId); // Получаем критерий из рубрикатора по ID или null, если не найден
        if (criterion == null)
            return null; // Критерий не найден в рубрикаторе

        if (score < 0 || score > criterion.Max)
            return null; // Балл вне допустимого диапазона

        return new Evaluation(criterionId, score);
    }
}