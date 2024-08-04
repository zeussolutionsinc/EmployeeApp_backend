namespace EmployeePortal.DTO
{
    public class ApproverXEmployee
    {
        public Guid Approver { get; set; }
        public Guid EmployeeId { get; set; }
    }


    public class ApproverXEmployeeDTO
    {
        public List<ApproverXEmployee> ApproverXEmployeeList { get; set; }

    }
}
