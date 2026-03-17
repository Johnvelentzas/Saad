using System.Diagnostics;

namespace Saad_Web_API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext _context)
        {
            if (!_context.Processes.Any())
            {
                var processes = new List<Models.Production.Processes>();
                foreach (var type in Enum.GetValues<Models.Production.ProcessesType>())
                {
                    processes.Add(new Models.Production.Processes { Type = type });
                }

                _context.Processes.AddRange(processes);
                _context.SaveChanges();
            }

            if (!_context.Users.Any())
            {
                var admin = new Models.Production.Users
                {
                    Id = 1,
                    Name = "Admin",
                };

                var user = new Models.Production.Users
                {
                    Id = 2,
                    Name = "Simple User",
                };

                var adminProcesses = new List<Models.Production.UserProcesses>();
                foreach (var process in _context.Processes)
                {
                    adminProcesses.Add(new Models.Production.UserProcesses
                    {
                        UserId = admin.Id,
                        ProcessId = process.Id
                    });
                }

                var userProcesses = new List<Models.Production.UserProcesses>();
                userProcesses.Add(new Models.Production.UserProcesses
                {
                    UserId = user.Id,
                    ProcessId = _context.Processes.First(p => p.Type == Models.Production.ProcessesType.FoamGel).Id
                });

                userProcesses.Add(new Models.Production.UserProcesses
                {
                    UserId = user.Id,
                    ProcessId = _context.Processes.First(p => p.Type == Models.Production.ProcessesType.Sew).Id
                });

                _context.Users.Add(admin);
                _context.UserProcesses.AddRange(adminProcesses);
                _context.Users.Add(user);
                _context.UserProcesses.AddRange(userProcesses);
                _context.SaveChanges();
            }
               
        }
    }
}
