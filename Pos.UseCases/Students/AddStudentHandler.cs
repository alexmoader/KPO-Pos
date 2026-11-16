using Pos.Models;
using Pos.UseCases.Ports;

namespace Pos.UseCases.Students;

public sealed class AddStudentHandler(IStudentRepository students)
{
    public Student Handle(string name)
    {
        var student = new Student(Guid.NewGuid(), name);
        students.Save(student);
        return student;
    }
}
