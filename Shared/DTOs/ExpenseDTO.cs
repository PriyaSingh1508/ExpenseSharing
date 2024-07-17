using System.Text.Json.Serialization;

namespace Shared.DTOs
{
    public class ExpenseDTO
    {
        public int ExpenseId { get; set; }
        public int GroupId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; } = 0;
        public string PaidBy { get; set; }
        public DateTime Date { get; set; }
        public string SplitAmong { get; set; }
        [JsonIgnore]
        public decimal IndividualAmount { get; set; } = 0;
      
    }
}
