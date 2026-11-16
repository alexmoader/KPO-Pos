namespace Pos.Models;

// Evaluation хранит ID критерия и балл.

public sealed class Evaluation
{
    public Guid CriterionId { get; }
    public int Score { get; }

    internal Evaluation(Guid criterionId, int score)
    {
        if (criterionId == Guid.Empty)
            throw new ArgumentException("Criterion ID cannot be empty", nameof(criterionId));

        if (score < 0)
            throw new ArgumentException("Score cannot be negative", nameof(score));

        CriterionId = criterionId;
        Score = score;
    }
}