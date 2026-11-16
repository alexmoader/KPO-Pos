using Pos.UseCases.Ports;
using Pos.Models;

//Для студентов добавлен метод List() в IStudentRepository 
public sealed class ListStudentsHandler
{
    private readonly IStudentRepository _students;

    public ListStudentsHandler(IStudentRepository students)
    {
        _students = students;
    }


    public IReadOnlyList<Student> Handle()
    {
        return _students.List();
    }
}