namespace Web.UI.Models
{
    public class DailyStockPrice
    {
        public int day { get; set; }
        public decimal price { get; set; }

        public DailyStockPrice()
        {
            day = 0;
            price = 0;
        }
    }
}
