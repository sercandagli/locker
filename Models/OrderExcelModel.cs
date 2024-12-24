namespace ExportExcel.Models
{
    public class OrderExcelModel
    {
        public int Id { get; set; }
        public string SenderName { get; set; }

        public string ReceiverName { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; }

    }
}