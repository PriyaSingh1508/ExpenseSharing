namespace Shared.DTOs
{
    public class GetExpensesDTO
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PaidBy { get; set; }
        public DateTime Date { get; set; }
        public string SplitAmong { get; set; }
        public decimal IndividualAmount { get; set; }
        public string ContributedBy { get; set; }
        public bool IsSettled { get; set; } = false;
    }
}
