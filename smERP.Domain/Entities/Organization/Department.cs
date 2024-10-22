using smERP.Domain.Entities.User;

namespace smERP.Domain.Entities.Organization;

public class Department : Entity
{
    public string Name { get; set; }
    public string DepartmentHeadId { get; set; }
    public Employee DepartmentHead { get; set; }
    public ICollection<Employee> DepartmentEmployees { get; set; }
}
