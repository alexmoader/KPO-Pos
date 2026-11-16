using System.Text.Json;
using Pos.UseCases.Evaluations;
using Pos.UseCases.Results;
using Pos.UseCases.Rubrics;
using Pos.UseCases.Students;

namespace Pos.Presentation;

public sealed class ConsoleUi(
    AddStudentHandler addStudent,
    ListStudentsHandler listStudents,
    AddCriterionHandler addCriterion,
    ListCriteriaHandler listCriteria,
    SetEvaluationHandler setEvaluation,
    ListEvaluationsHandler listEvaluations,
    RemoveEvaluationHandler removeEvaluation,
    GetFinalResultHandler getResult,
    ExportResultsHandler export)
{
    public int Run(string[] args, string dataPath)
    {
        bool menu = args is [] or ["menu", _];
        bool initialize = args is ["init", _];
        bool show = args is ["show", _, _] && Guid.TryParse(args[2], out _);
        if (!menu && !initialize && !show)
        {
            Console.Error.WriteLine("Usage: menu <data.json> | init <data.json> | show <data.json> <student-id>");
            return 1;
        }

        try
        {
            if (menu)
            {
                RunMenu();
                return 0;
            }

            Guid studentId;
            if (initialize)
            {
                if (File.Exists(dataPath))
                    throw new InvalidOperationException("Укажите новый файл для демонстрации.");

                var student = addStudent.Handle("Иван");
                var practice = addCriterion.Handle("Практика", 10, 1);
                var project = addCriterion.Handle("Проект", 20, 3);

                if (!setEvaluation.Handle(student.Id, practice.Id, 8) ||
                    !setEvaluation.Handle(student.Id, project.Id, 10))
                {
                    throw new InvalidOperationException("Не удалось выставить демонстрационные оценки.");
                }

                studentId = student.Id;
            }
            else
            {
                studentId = Guid.Parse(args[2]);
            }

            var result = getResult.Handle(studentId);
            Console.WriteLine($"ID: {result.StudentId}");
            Console.WriteLine(FormattableString.Invariant($"Итог: {result.Percent:0.##}%"));
            return 0;
        }
        catch (Exception error) when (error is ArgumentException or InvalidOperationException
            or KeyNotFoundException or IOException or UnauthorizedAccessException or JsonException)
        {
            Console.Error.WriteLine(error.Message);
            return 1;
        }
    }

    private void RunMenu()
    {
        Console.WriteLine("""
            1. Добавить студента
            2. Показать студентов
            3. Добавить критерий
            4. Показать критерии
            5. Выставить или исправить оценку
            6. Показать оценки студента
            7. Отменить оценку
            8. Показать итог студента
            9. Экспортировать итог студента
            0. Выйти
            """);

        while (true)
        {
            Console.Write("Команда: ");
            string? command = Console.ReadLine()?.Trim();
            if (command is null or "0")
                return;

            try
            {
                switch (command)
                {
                    case "1":
                        var student = addStudent.Handle(ReadText("Имя"));
                        Console.WriteLine($"{student.Id} — {student.Name}");
                        break;
                    case "2":
                        foreach (var item in listStudents.Handle())
                            Console.WriteLine($"{item.Id} — {item.Name}");
                        break;
                    case "3":
                        var criterion = addCriterion.Handle(
                            ReadText("Название"), ReadInt("Максимальный балл"), ReadInt("Вес"));
                        Console.WriteLine($"{criterion.Id} — {criterion.Name}");
                        break;
                    case "4":
                        foreach (var item in listCriteria.Handle())
                            Console.WriteLine($"{item.Id} — {item.Name}, максимум {item.Max}, вес {item.Weight}");
                        break;
                    case "5":
                        bool accepted = setEvaluation.Handle(
                            ReadId("ID студента"), ReadId("ID критерия"), ReadInt("Балл"));
                        Console.WriteLine(accepted ? "Оценка сохранена." : "Критерий или балл недопустим.");
                        break;
                    case "6":
                        foreach (var evaluation in listEvaluations.Handle(ReadId("ID студента")))
                            Console.WriteLine($"{evaluation.CriterionId} — {evaluation.Score}");
                        break;
                    case "7":
                        bool removed = removeEvaluation.Handle(ReadId("ID студента"), ReadId("ID критерия"));
                        Console.WriteLine(removed ? "Оценка отменена." : "Оценка отсутствует.");
                        break;
                    case "8":
                        var result = getResult.Handle(ReadId("ID студента"));
                        Console.WriteLine($"Итог: {result.Percent:0.##}%");
                        break;
                    case "9":
                        export.Handle(ReadId("ID студента"), ReadExportFormat(), ReadText("Путь к новому файлу"));
                        Console.WriteLine("Итог экспортирован.");
                        break;
                    default:
                        Console.WriteLine("Выбери команду от 0 до 9.");
                        break;
                }
            }
            catch (EndOfStreamException)
            {
                return;
            }
            catch (Exception error) when (error is ArgumentException or InvalidOperationException
                or KeyNotFoundException or IOException or UnauthorizedAccessException or JsonException)
            {
                Console.Error.WriteLine(error.Message);
            }
        }
    }

    private static string ReadText(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine()?.Trim() ?? throw new EndOfStreamException();
    }

    private static ResultExportFormat ReadExportFormat() =>
        ReadText("Формат (csv/json)").ToLowerInvariant() switch
        {
            "csv" => ResultExportFormat.Csv,
            "json" => ResultExportFormat.Json,
            _ => throw new ArgumentException("Доступные форматы экспорта: csv и json.")
        };

    private static Guid ReadId(string prompt) =>
        Guid.TryParse(ReadText(prompt), out var id)
            ? id
            : throw new ArgumentException("Введи корректный ID.");

    private static int ReadInt(string prompt) =>
        int.TryParse(ReadText(prompt), out var value)
            ? value
            : throw new ArgumentException("Введи целое число.");
}
