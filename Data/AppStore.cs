using Kt7Api.Models;

namespace Kt7Api.Data;

public class AppStore
{
    public object SyncRoot { get; } = new();
    public int NextStudentId { get; set; } = 4;
    public int NextTaskId { get; set; } = 4;
    public int NextBookingId { get; set; } = 1;
    public List<Student> Students { get; } =
    [
        new() { Id = 1, Name = "Даниил Денисенко", Age = 20 },
        new() { Id = 2, Name = "Анна Иванова", Age = 21 },
        new() { Id = 3, Name = "Иван Петров", Age = 19 }
    ];
    public List<TaskItem> Tasks { get; } =
    [
        new() { Id = 1, Title = "Подготовить отчёт", Description = "Описать выполненные задания", Status = "new" },
        new() { Id = 2, Title = "Добавить API", Description = "Сделать методы для работы с данными", Status = "in-progress" },
        new() { Id = 3, Title = "Создать проект", Description = "Подготовить проект ASP.NET Core", Status = "completed" }
    ];
    public List<Resource> Resources { get; } =
    [
        new() { Id = 1, Name = "Номер 101", Type = "room" },
        new() { Id = 2, Name = "Номер 102", Type = "room" },
        new() { Id = 3, Name = "Стол 1", Type = "table" },
        new() { Id = 4, Name = "Стол 2", Type = "table" },
        new() { Id = 5, Name = "Зал 1", Type = "hall" },
        new() { Id = 6, Name = "Зал 2", Type = "hall" }
    ];
    public List<Booking> Bookings { get; } = [];

    public bool HasConflict(int resourceId, DateTimeOffset start, DateTimeOffset end, int? excludedId = null)
    {
        return Bookings.Any(booking => booking.ResourceId == resourceId && booking.Id != excludedId
            && booking.Start < end && booking.End > start);
    }
}
