using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class ExpenseService(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    // Read (Get user expenses)
    public async Task<List<Expense>> GetExpensesAsync(string userId)
    {
        using var context = dbFactory.CreateDbContext();
        return await context.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    // Create and read 
    public async Task SaveExpenseAsync(Expense expense, string userId)
    {
        using var context = dbFactory.CreateDbContext();

        if (expense.Id == 0)
        {
            expense.UserId = userId;
            context.Expenses.Add(expense);
        }
        else
        {
            var existing = await context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expense.Id && e.UserId == userId);

            if (existing is null)
                return; 

            existing.Title    = expense.Title;
            existing.Amount   = expense.Amount;
            existing.Category = expense.Category;
            existing.Date     = expense.Date;
            existing.Note     = expense.Note;
        }

        await context.SaveChangesAsync();
    }

    // Delete an expense
    public async Task DeleteExpenseAsync(int id, string userId)
    {
        using var context = dbFactory.CreateDbContext();
        var item = await context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        
        if (item != null)
        {
            context.Expenses.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}