namespace EmployeePortal.DTO
{
    public class EmployeeListDTO
    {
        public List<(Guid, string)> EmployeeId { get; set; }
        public List<(Guid, string)> AdminEmployee { get; set; }
    }
}
