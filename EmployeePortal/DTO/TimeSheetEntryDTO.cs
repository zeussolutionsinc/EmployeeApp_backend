namespace EmployeePortal.DTO
{
    public class TimeSheetEntryDTO
    {
        public Guid? EmployeeId { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public List<ProjectDateHoursEntry> ProjectDateHours { get; set; } = new List<ProjectDateHoursEntry>();
        public Guid? Approver { get; set; }
    }
}
