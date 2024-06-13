namespace EmployeePortal.DTO
{
    public class TimeSheetStatsDashboardDTO
    {
        public int ApprovedRecords { get; set; }
        public int RejectedRecords { get; set; }  
        public int PendingRecords { get; set; }
        public int SubmittedRecords { get; set; }
        public int TotalRecords { get; set; }
        public List<string> CurrentProjects { get; set; }

    }
}
