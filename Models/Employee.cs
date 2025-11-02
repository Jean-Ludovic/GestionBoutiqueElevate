namespace GestionBoutiqueElevate.Models
{
    public enum EmployeeRole { Admin, Employee }

    public class Employee
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";  // format XX-00
        public string FullName { get; set; } = "";
        public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
    }
}
