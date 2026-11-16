namespace Pos.Models;

// Неизменяемый результат с ID студента и процентом. 

public sealed record FinalResult
{
    public Guid StudentId { get; }
    public decimal Percent { get; }

    public FinalResult(Guid studentId, decimal percent)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));

        if (percent < 0 || percent > 100)
            throw new ArgumentOutOfRangeException(nameof(percent), "Percent must be between 0 and 100");

        StudentId = studentId;
        Percent = percent;
    }
}
