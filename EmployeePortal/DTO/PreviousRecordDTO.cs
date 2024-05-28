namespace EmployeePortal.DTO
{
    //public class DateRange
    //{
    //    public DateOnly? StartDate { get; set; }
    //    public DateOnly? EndDate { get; set; }
    //}
    public class PreviousRecordDTO
    {
        public string? RecordNumber { get; set; }
        public DateOnly? SubmissionDate { get; set; }
        public short? WorkingHours { get; set; }
        public string? ApprovalStatus { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

    }
}
