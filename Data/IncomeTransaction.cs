using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Data;

public class IncomeTransaction : IValidatableObject
{
    public int Id { get; set; }

    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Income source is required.")]
    [StringLength(100, ErrorMessage = "Income source cannot exceed 100 characters.")]
    public string Source { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Income date cannot be in the future.",
                [nameof(Date)]);
        }
    }
}
