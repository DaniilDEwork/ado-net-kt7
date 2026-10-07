using System.ComponentModel.DataAnnotations;

namespace Kt7Api.Models;

public class BookingRequest : IValidatableObject
{
    [Required(ErrorMessage = "Выберите ресурс")]
    [Range(1, int.MaxValue, ErrorMessage = "ID ресурса должен быть положительным")]
    public int? ResourceId { get; set; }

    [Required(ErrorMessage = "Укажите имя клиента")]
    [StringLength(100, ErrorMessage = "Имя не должно быть длиннее 100 символов")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Укажите начало бронирования")]
    public DateTimeOffset? Start { get; set; }

    [Required(ErrorMessage = "Укажите окончание бронирования")]
    public DateTimeOffset? End { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start.HasValue && End.HasValue && Start >= End)
        {
            yield return new ValidationResult("Окончание должно быть позже начала", [nameof(End)]);
        }
    }
}
