using Microsoft.Extensions.DependencyInjection;
using Pos.Infrastructure.Export;
using Pos.Infrastructure.Persistence;
using Pos.Models;
using Pos.Presentation;
using Pos.UseCases.Evaluations;
using Pos.UseCases.Ports;
using Pos.UseCases.Results;
using Pos.UseCases.Rubrics;
using Pos.UseCases.Students;

// Настройка пути к файлу с данными.
string dataPath = args.Length < 2 ? "pos-data.json" : args[1];

// Создание контейнера зависимостей.
var registrations = new ServiceCollection();

// Регистрация хранилища и репозиториев.
registrations.AddSingleton(new JsonDataStore(dataPath));
registrations.AddSingleton<IStudentRepository, JsonStudentRepository>();
registrations.AddSingleton<IRubricRepository, JsonRubricRepository>();

// Регистрация сервисов для расчёта результатов.
registrations.AddSingleton<WeightedResultCalculator>();
registrations.AddSingleton<IResultCalculator>(services =>
    new RoundedResultCalculator(services.GetRequiredService<WeightedResultCalculator>()));

// Регистрация стратегий экспорта.
registrations.AddSingleton<IResultExportStrategy, CsvExportStrategy>();
registrations.AddSingleton<IResultExportStrategy, JsonExportStrategy>();

// Регистрация обработчиков команд приложения.
registrations.AddTransient<AddStudentHandler>();
registrations.AddTransient<ListStudentsHandler>();
registrations.AddTransient<AddCriterionHandler>();
registrations.AddTransient<ListCriteriaHandler>();
registrations.AddTransient<SetEvaluationHandler>();
registrations.AddTransient<ListEvaluationsHandler>();
registrations.AddTransient<RemoveEvaluationHandler>();
registrations.AddTransient<GetFinalResultHandler>();
registrations.AddTransient<ExportResultsHandler>();

// Регистрация консольного интерфейса.
registrations.AddTransient<ConsoleUi>();

// Построение контейнера и запуск приложения.
using var services = registrations.BuildServiceProvider();
return services.GetRequiredService<ConsoleUi>().Run(args, dataPath);
