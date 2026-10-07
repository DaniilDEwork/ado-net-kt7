using System.ComponentModel.DataAnnotations;

namespace Kt7Api.Models;

public class TaskRequest
{
    [Required(ErrorMessage = "Укажите название задачи")]
    [StringLength(100, ErrorMessage = "Название не должно быть длиннее 100 символов")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Укажите описание задачи")]
    [StringLength(1000, ErrorMessage = "Описание не должно быть длиннее 1000 символов")]
    public string Description { get; set; } = "";
}

public class TaskStatusRequest
{
    [Required(ErrorMessage = "Укажите статус задачи")]
    [RegularExpression("^(new|in-progress|completed)$", ErrorMessage = "Допустимые статусы: new, in-progress, completed")]
    public string Status { get; set; } = "";
}
