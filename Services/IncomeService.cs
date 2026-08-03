using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class IncomeService(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    // Return only the income records that belong to the authenticated user.
    public async Task<List<IncomeTransaction>> GetIncomeAsync(string userId)
    {
        await using var context = await dbFactory.CreateDbContextAsync();

        return await context.IncomeTransactions
            .Where(income => income.UserId == userId)
            .OrderByDescending(income => income.Date)
            .ThenByDescending(income => income.Id)
            .ToListAsync();
    }

    // Create a new income record or update an existing one for the authenticated user.
    public async Task<bool> SaveIncomeAsync(IncomeTransaction income, string userId)
    {
        await using var context = await dbFactory.CreateDbContextAsync();

        if (income.Id == 0)
        {
            income.UserId = userId;
            context.IncomeTransactions.Add(income);
        }
        else
        {
            var existing = await context.IncomeTransactions
                .FirstOrDefaultAsync(item => item.Id == income.Id && item.UserId == userId);

            if (existing is null)
            {
                return false;
            }

            existing.Amount = income.Amount;
            existing.Date = income.Date;
            existing.Source = income.Source;
            existing.Notes = income.Notes;
        }

        await context.SaveChangesAsync();
        return true;
    }

    // Delete an income record only if it belongs to the authenticated user.
    public async Task<bool> DeleteIncomeAsync(int id, string userId)
    {
        await using var context = await dbFactory.CreateDbContextAsync();
        var income = await context.IncomeTransactions
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

        if (income is null)
        {
            return false;
        }

        context.IncomeTransactions.Remove(income);
        await context.SaveChangesAsync();
        return true;
    }
}