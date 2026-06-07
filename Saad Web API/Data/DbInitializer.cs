using Models.Attributes;
using Models.Management;
using Models.Production;

namespace Saad_Web_API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext _context)
        {
            // 1. Get all the types defined in your enum
            var allProcessTypes = Enum.GetValues<ProcessesType>();

            // 2. Query the database to see which types already exist
            var existingProcessTypes = _context.Processes.Select(p => p.Type).ToList();

            // 3. Find the ones that are in the enum but NOT in the database
            var missingProcessTypes = allProcessTypes.Except(existingProcessTypes).ToList();

            // 4. If any are missing, add them
            if (missingProcessTypes.Any())
            {
                var processesToAdd = new List<Processes>();

                foreach (var type in missingProcessTypes)
                {
                    processesToAdd.Add(new Processes { Type = type });
                    // Note: Use 'Models.Management.Processes' here if you need the explicit namespace
                }

                _context.Processes.AddRange(processesToAdd);
                _context.SaveChanges();
            }

            // 1. Check if Admin exists, create if they don't
            var admin = _context.Users.FirstOrDefault(u => u.Name == "Admin");
            if (admin == null)
            {
                admin = new Models.Management.Users
                {
                    Name = "Admin",
                };

                _context.Users.Add(admin);
                _context.SaveChanges(); // Save immediately to generate the new admin.Id
            }

            // 2. Get all process IDs that currently exist in the database
            var allProcessIds = _context.Processes.Select(p => p.Id).ToList();

            // 3. Get the process IDs that the Admin already has access to
            var adminExistingProcessIds = _context.UserProcesses
                .Where(up => up.UserId == admin.Id)
                .Select(up => up.ProcessId)
                .ToList();

            // 4. Find the missing process IDs (all processes EXCEPT the ones admin already has)
            var missingProcessIds = allProcessIds.Except(adminExistingProcessIds).ToList();

            // 5. If the admin is missing any processes, add them
            if (missingProcessIds.Any())
            {
                var adminProcessesToAdd = new List<Models.Management.UserProcesses>();

                foreach (var processId in missingProcessIds)
                {
                    adminProcessesToAdd.Add(new Models.Management.UserProcesses
                    {
                        UserId = admin.Id,
                        ProcessId = processId
                    });
                }

                _context.UserProcesses.AddRange(adminProcessesToAdd);
                _context.SaveChanges();
            }
        }
    }
}
