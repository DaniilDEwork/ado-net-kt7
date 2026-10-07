using System.ComponentModel.DataAnnotations;

namespace Kt7Api.Models;

public class StudentRequest
{
    [Required(ErrorMessage = "Укажите имя студента")]
    [StringLength(100, ErrorMessage = "Имя не должно быть длиннее 100 символов")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Укажите возраст")]
    [Range(1, 120, ErrorMessage = "Возраст должен быть от 1 до 120")]
    public int? Age { get; set; }
}
