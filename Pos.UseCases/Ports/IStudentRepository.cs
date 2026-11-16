using Pos.Models;

namespace Pos.UseCases.Ports;

public interface IStudentRepository
{
    Student? GetById(Guid id);
    void Save(Student student);

    IReadOnlyList<Student> List();
}
