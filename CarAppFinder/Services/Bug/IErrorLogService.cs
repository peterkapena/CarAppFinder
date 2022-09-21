using CarAppFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CarAppFinder.Models.Pub.Pub;

namespace CarAppFinder.Services.Bug
{
    public interface IErrorLogService
    {
        public Task RegisterError(Exception ex, User user = null);
    }
    public class ErrorLogService : IErrorLogService
    {
        public ErrorLogService(DatabaseContext   context)
        {
            Context = context;
        }

        public DatabaseContext Context { get; }

        public async Task RegisterError(Exception ex, User user = null)
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

            var error = new ErrorLog { StackTrace = ex.StackTrace, HelpLink = ex.HelpLink, HResult = ex.HResult, InnerException = ex.InnerException?.ToString() ?? ex.StackTrace, Message = ex.Message, Source = ex.Source, Date = DateTime.Now };

            if (user != null)
            {
                error.UserId = user.Id;
                error.UserName = user.UserName;
            }

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
