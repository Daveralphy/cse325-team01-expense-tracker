using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Data;

public class Expense
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Range(0.01, 1000000, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.Today;

    public string? Note { get; set; }
    public string Category { get; set; } = "General";

    //Benjamin -  Linked expense to the authenticated user 
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}