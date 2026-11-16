namespace Pos.Models;

//	Интерфейс с методом 
//  FinalResult Calculate(Student student, Rubric rubric);
//  Позже тот же интерфейс реализует декоратор округления

public interface IResultCalculator
{
    FinalResult Calculate(Student student, Rubric rubric);
}