using CinemaAPI.Databases.CinemaDB;
using Quartz;

public class BillCleanupJob : IJob
{
    private readonly DBContext _context;

    public BillCleanupJob(DBContext context)
    {
        _context = context;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.Now;
        var unpaidBills = _context.Bill
            .Where(b => b.State == 0 && b.TimeCreated.AddMinutes(3) <= now)
            .ToList();

        foreach (var bill in unpaidBills)
        {
            bill.State = 2;
        }
        
        _context.SaveChanges();

        return Task.CompletedTask;
    }
}