using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBoutiqueElevate.Models;

namespace GestionBoutiqueElevate.Services
{
    public class InMemoryEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _emps = new()
        {
            new Employee{ Id=1, Code="AD-01", FullName="Admin Principal", Role=EmployeeRole.Admin },
            new Employee{ Id=2, Code="JB-11", FullName="Jean Bellemare", Role=EmployeeRole.Employee },
            new Employee{ Id=3, Code="SD-22", FullName="Sarah Dubois", Role=EmployeeRole.Employee }
        };

        public Task<Employee?> GetByCodeAsync(string code)
        {
            var c = (code ?? "").Trim().ToUpperInvariant();
            return Task.FromResult(_emps.FirstOrDefault(e => e.Code == c));
        }

        public Task<IEnumerable<Employee>> GetAllAsync() =>
            Task.FromResult(_emps.AsEnumerable());

        public Task<Employee> AddAsync(Employee e)
        {
            e.Id = _emps.Count == 0 ? 1 : _emps.Max(x => x.Id) + 1;
            _emps.Add(e);
            return Task.FromResult(e);
        }

        public Task<string> GenerateCodeAsync()
        {
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (; ; )
            {
                var r = new System.Random();
                var a = letters[r.Next(0, letters.Length)];
                var b = letters[r.Next(0, letters.Length)];
                var n = r.Next(0, 100).ToString("00");
                var code = $"{a}{b}-{n}";
                if (!_emps.Any(x => x.Code == code)) return Task.FromResult(code);
            }
        }
    }
}
