using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OronganLeandroUlysisEPHRS
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public decimal MonthlySalary { get; set; }
        public double TaxRate { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
    }

    class Program
    {

           const string AppTitle = "EPHRS - Feature Branch v2";

        static List<Employee> employees = new List<Employee>();
        static int nextId = 1001;

        static void Main()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== " + AppTitle + " ===");
                Console.WriteLine("1. Create Employee");
                Console.WriteLine("2. Read All Employees");
                Console.WriteLine("3. Search / Filter");
                Console.WriteLine("4. Update Employee");
                Console.WriteLine("5. Delete Employee");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": CreateEmployee(); break;
                    case "2": ReadAll(); break;
                    case "3": SearchMenu(); break;
                    case "4": UpdateEmployee(); break;
                    case "5": DeleteEmployee(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }
   
        static void CreateEmployee()
        {
            Console.WriteLine("\n--- Create Employee ---");
            var emp = new Employee
            {
                Id = nextId++,
                FirstName = ReadRequired("First name: "),
                LastName = ReadRequired("Last name: "),
                Department = ReadRequired("Department (IT/HR/Sales/Finance): "),
                JobTitle = ReadRequired("Job title: "),
                MonthlySalary = ReadDecimal("Monthly salary: "),
                TaxRate = ReadDouble("Tax rate (e.g. 0.12): "),
                HireDate = ReadDate("Hire date (yyyy-MM-dd): "),
                IsActive = ReadBool("Active? (y/n): "),
                Email = ReadRequired("Email: ")
            };
            employees.Add(emp);
            Console.WriteLine($"Employee created with ID {emp.Id}.");
        }

        
        static void ReadAll()
        {
            Console.WriteLine("\n--- All Employees ---");
            PrintTable(employees.OrderBy(e => e.Id));
        }

        static void SearchMenu()
        {
            Console.WriteLine("\n--- Search / Filter ---");
            Console.WriteLine("1. By name (first or last)");
            Console.WriteLine("2. By department");
            Console.WriteLine("3. By status (active/inactive)");
            Console.WriteLine("4. By salary range");
            Console.WriteLine("5. By ID");
            Console.Write("Choose: ");

            IEnumerable<Employee> result;
            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    string name = ReadRequired("Name keyword: ").ToLower();
                    result = employees.Where(e => e.FirstName.ToLower().Contains(name)
                                               || e.LastName.ToLower().Contains(name));
                    break;
                case "2":
                    string dept = ReadRequired("Department: ").ToLower();
                    result = employees.Where(e => e.Department.ToLower() == dept);
                    break;
                case "3":
                    bool active = ReadBool("Active? (y/n): ");
                    result = employees.Where(e => e.IsActive == active);
                    break;
                case "4":
                    decimal min = ReadDecimal("Min salary: ");
                    decimal max = ReadDecimal("Max salary: ");
                    result = employees.Where(e => e.MonthlySalary >= min && e.MonthlySalary <= max)
                                      .OrderByDescending(e => e.MonthlySalary);
                    break;
                case "5":
                    int id = ReadInt("Employee ID: ");
                    result = employees.Where(e => e.Id == id);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            PrintTable(result);
        }

        static void UpdateEmployee()
        {
            Console.WriteLine("\n--- Update Employee ---");
            int id = ReadInt("Employee ID to update: ");
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            Console.WriteLine("Press Enter to keep the current value.");

            string s;
            Console.Write($"First name [{emp.FirstName}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.FirstName = s.Trim();
            Console.Write($"Last name [{emp.LastName}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.LastName = s.Trim();
            Console.Write($"Department [{emp.Department}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.Department = s.Trim();
            Console.Write($"Job title [{emp.JobTitle}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.JobTitle = s.Trim();

            Console.Write($"Monthly salary [{emp.MonthlySalary}]: ");
            if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sal) && sal >= 0)
                emp.MonthlySalary = sal;

            Console.Write($"Tax rate [{emp.TaxRate}]: ");
            if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) && tax >= 0 && tax <= 1)
                emp.TaxRate = tax;

            Console.Write($"Hire date [{emp.HireDate:yyyy-MM-dd}]: ");
            if (DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime hd))
                emp.HireDate = hd;

            Console.Write($"Active? y/n [{(emp.IsActive ? "y" : "n")}]: ");
            s = Console.ReadLine()?.Trim().ToLower();
            if (s == "y") emp.IsActive = true;
            else if (s == "n") emp.IsActive = false;

            Console.Write($"Email [{emp.Email}]: ");
            if (!string.IsNullOrWhiteSpace(s = Console.ReadLine())) emp.Email = s.Trim();

            Console.WriteLine("Employee updated.");
            PrintTable(employees.Where(e => e.Id == id));
        }

        static void DeleteEmployee()
        {
            Console.WriteLine("\n--- Delete Employee ---");
            int id = ReadInt("Employee ID to delete: ");
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            if (ReadBool($"Delete {emp.FirstName} {emp.LastName}? (y/n): "))
            {
                employees.RemoveAll(e => e.Id == id);
                Console.WriteLine("Employee deleted.");
            }
            else Console.WriteLine("Delete cancelled.");
        }

        static void PrintTable(IEnumerable<Employee> list)
        {
            var rows = list.ToList();
            if (!rows.Any()) { Console.WriteLine("No records found."); return; }

            Console.WriteLine(new string('-', 150));
            Console.WriteLine($"{"ID",-6}{"First",-12}{"Last",-12}{"Dept",-10}{"Job Title",-18}{"Salary",-12}{"Tax",-7}{"Net Pay",-12}{"Hired",-12}{"Active",-8}{"Email"}");
            Console.WriteLine(new string('-', 150));
            foreach (var e in rows)
            {
                decimal net = e.MonthlySalary - e.MonthlySalary * (decimal)e.TaxRate;
                Console.WriteLine($"{e.Id,-6}{e.FirstName,-12}{e.LastName,-12}{e.Department,-10}{e.JobTitle,-18}{e.MonthlySalary,-12:N2}{e.TaxRate,-7:P0}{net,-12:N2}{e.HireDate,-12:yyyy-MM-dd}{(e.IsActive ? "Yes" : "No"),-8}{e.Email}");
            }
            Console.WriteLine(new string('-', 150));
            Console.WriteLine($"Records: {rows.Count} | Total Monthly Payroll: {rows.Sum(e => e.MonthlySalary):N2}");
        }
        static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(s)) return s;
                Console.WriteLine("Required field.");
            }
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v)) return v;
                Console.WriteLine("Enter a valid whole number.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v) && v >= 0) return v;
                Console.WriteLine("Enter a valid non-negative amount.");
            }
        }

        static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double v) && v >= 0 && v <= 1) return v;
                Console.WriteLine("Enter a rate between 0 and 1 (e.g. 0.12).");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime v)) return v;
                Console.WriteLine("Use format yyyy-MM-dd.");
            }
        }

        static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine()?.Trim().ToLower();
                if (s == "y") return true;
                if (s == "n") return false;
                Console.WriteLine("Enter y or n.");
            }
        }
    }
}