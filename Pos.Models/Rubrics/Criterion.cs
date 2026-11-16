namespace Pos.Models;

// Todo:
// Criterion хранит ID, название, максимум и вес. 
// В конструкторе проверить, что название непустое, 
// а максимум и вес больше нуля. 

public sealed class Criterion
{
    public Guid Id { get;}
    public string Name { get; }
    public int Max { get; }
    public int Weight { get; }  
    public Criterion(Guid id, string name, int max, int weight)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Criterion ID cannot be empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Criterion name cannot be empty", nameof(name));

        if (max <= 0)
            throw new ArgumentException("Criterion max must be greater than zero", nameof(max));

        if (weight <= 0)
            throw new ArgumentException("Criterion weight must be greater than zero", nameof(weight));

        Id = id;
        Name = name.Trim();
        Max = max;
        Weight = weight;
    }
}
