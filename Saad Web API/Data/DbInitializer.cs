using Models.Attributes;
using Models.Finances;

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
                    Name = "Admin",
                };

                var user = new Models.Production.Users
                {
                    Name = "Simple User",
                };

                _context.Users.Add(admin);
                _context.Users.Add(user);
                _context.SaveChanges();

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

                _context.UserProcesses.AddRange(adminProcesses);
                _context.UserProcesses.AddRange(userProcesses);
                _context.SaveChanges();
            }
            if (!_context.Customers.Any())
            {
                var customers = new List<Customers>();
                customers.Add(new Customers
                {
                    FirstName = "1Ioannis",
                    LastName = "Velentzas",
                    TaxNumber = "418256581723",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale,
                    IsDraft = true
                });

                customers.Add(new Customers
                {
                    FirstName = "2Cevin",
                    LastName = "Schmidt",
                    TaxNumber = "576882734",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail,
                    IsDraft = true
                });
                customers.Add(new Customers
                {
                    FirstName = "3Elena",
                    LastName = "Rodriguez",
                    TaxNumber = "ES992837465",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "4Hiroshi",
                    LastName = "Tanaka",
                    TaxNumber = "JP112233445",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "5Amina",
                    LastName = "Osei",
                    TaxNumber = "GH556677889",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "6Lucas",
                    LastName = "Dubois",
                    TaxNumber = "FR443322110",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "7Svetlana",
                    LastName = "Ivanova",
                    TaxNumber = "RU887766554",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "8Arjun",
                    LastName = "Patel",
                    TaxNumber = "IN776655443",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "9Sienna",
                    LastName = "Brooks",
                    TaxNumber = "AU334455667",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "10Matteo",
                    LastName = "Ricci",
                    TaxNumber = "IT221144335",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "11Freja",
                    LastName = "Nielsen",
                    TaxNumber = "DK990011223",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "12Liam",
                    LastName = "O'Connor",
                    TaxNumber = "IE665577884",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });
                customers.Add(new Customers
                {
                    FirstName = "13Elena",
                    LastName = "Rodriguez",
                    TaxNumber = "ES992837465",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "14Hiroshi",
                    LastName = "Tanaka",
                    TaxNumber = "JP112233445",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "15Amina",
                    LastName = "Osei",
                    TaxNumber = "GH556677889",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "16Lucas",
                    LastName = "Dubois",
                    TaxNumber = "FR443322110",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "17Svetlana",
                    LastName = "Ivanova",
                    TaxNumber = "RU887766554",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "18Arjun",
                    LastName = "Patel",
                    TaxNumber = "IN776655443",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "19Sienna",
                    LastName = "Brooks",
                    TaxNumber = "AU334455667",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "20Matteo",
                    LastName = "Ricci",
                    TaxNumber = "IT221144335",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "21Freja",
                    LastName = "Nielsen",
                    TaxNumber = "DK990011223",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "22Liam",
                    LastName = "O'Connor",
                    TaxNumber = "IE665577884",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });
                customers.Add(new Customers
                {
                    FirstName = "23Elena",
                    LastName = "Rodriguez",
                    TaxNumber = "ES992837465",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "24Hiroshi",
                    LastName = "Tanaka",
                    TaxNumber = "JP112233445",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "25Amina",
                    LastName = "Osei",
                    TaxNumber = "GH556677889",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "26Lucas",
                    LastName = "Dubois",
                    TaxNumber = "FR443322110",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "27Svetlana",
                    LastName = "Ivanova",
                    TaxNumber = "RU887766554",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "28Arjun",
                    LastName = "Patel",
                    TaxNumber = "IN776655443",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "29Sienna",
                    LastName = "Brooks",
                    TaxNumber = "AU334455667",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "30Matteo",
                    LastName = "Ricci",
                    TaxNumber = "IT221144335",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "31Freja",
                    LastName = "Nielsen",
                    TaxNumber = "DK990011223",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "32Liam",
                    LastName = "O'Connor",
                    TaxNumber = "IE665577884",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });
                customers.Add(new Customers
                {
                    FirstName = "33Elena",
                    LastName = "Rodriguez",
                    TaxNumber = "ES992837465",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "34Hiroshi",
                    LastName = "Tanaka",
                    TaxNumber = "JP112233445",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "35Amina",
                    LastName = "Osei",
                    TaxNumber = "GH556677889",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "36Lucas",
                    LastName = "Dubois",
                    TaxNumber = "FR443322110",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "37Svetlana",
                    LastName = "Ivanova",
                    TaxNumber = "RU887766554",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "38Arjun",
                    LastName = "Patel",
                    TaxNumber = "IN776655443",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "39Sienna",
                    LastName = "Brooks",
                    TaxNumber = "AU334455667",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "40Matteo",
                    LastName = "Ricci",
                    TaxNumber = "IT221144335",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                customers.Add(new Customers
                {
                    FirstName = "41Freja",
                    LastName = "Nielsen",
                    TaxNumber = "DK990011223",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Wholesale
                });

                customers.Add(new Customers
                {
                    FirstName = "42Liam",
                    LastName = "O'Connor",
                    TaxNumber = "IE665577884",
                    CreatedDate = DateTime.Now,
                    Type = CustomerType.Retail
                });

                _context.AddRange(customers);
                _context.SaveChanges();
            }
            if (!_context.ProductCategories.Any())
            {
                var categories = new List<ProductCategories>();
                categories.Add(new ProductCategories
                {
                    CategoryName = "Moto Covers"
                });
                categories.Add(new ProductCategories
                {
                    CategoryName = "Yacht Covers"
                });

                _context.AddRange(categories);
                _context.SaveChanges();
            }

        }
    }
}
