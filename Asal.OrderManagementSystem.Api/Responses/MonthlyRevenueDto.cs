namespace Asal.OrderManagementSystem.Api.Responses
{
    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        private MonthlyRevenueDto()
        {
            
        }
        public static MonthlyRevenueDto FromDateAndRevenue(DateTime date,decimal revenue)
        {
            return new MonthlyRevenueDto
            {
                Month = date.Month,
                Year = date.Year,
                Revenue = revenue
            };
        }

    }
}