using CarAppFinder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static CarAppFinder.Models.Pub.Pub;

namespace CarAppFinder.Services.Bug
{
    public interface IErrorLogService
    {
        public Task RegisterError(Exception ex, string userId = null);
    }
    public class ErrorLogService : IErrorLogService
    {
        public ErrorLogService(DatabaseContext context)
        {
            Context = context;
        }

        public DatabaseContext Context { get; }

        public async Task RegisterError(Exception ex, string userId = null)
        {
            foreach (EntityEntry entry in Context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }

            var error = new ErrorLog
            {
                UserId = userId,
                StackTrace = ex.StackTrace,
                HelpLink = ex.HelpLink,
                HResult = ex.HResult,
                InnerException = ex.InnerException?.ToString() ?? ex.StackTrace,
                Message = ex.Message,
                Source = ex.Source,
                Date = DateTime.Now
            };

            await Context.Errors.AddAsync(error);
            await Context.SaveChangesAsync();
            foreach (EntityEntry entry in Context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }
    }
}
