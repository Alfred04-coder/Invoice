namespace InvoiceManagement.ViewModel
{
    public class DashboardViewModel
    {
        //Summary
        public decimal TotalSales { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int CompanyId { get; set; }

        // Filters
        public string SelectedFilter { get; set; } = "month";

        public decimal TotalDebt { get; set; }

        // Tables
        public List<RecentInvoiceViewModel> RecentInvoices { get; set; } = new();
        public List<TopCustomerVM> TopCustomers { get; set; } = new();

        //🔥 Chart Data
        public List<string> TopCustomerNames { get; set; } = new();
        public List<decimal> TopCustomerTotals { get; set; } = new();
        public List<TopDebtorViewModel> TopDebtors { get; internal set; }
    }

    public class TopDebtorViewModel
    {
        public string CustomerName { get; set; }
        public decimal Debt { get; set; }
    }

    public class RecentInvoiceViewModel
    {
        public int InvoiceId { get; set; }
        public int CompanyId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class TopCustomerVM
    {
        public string CustomerName { get; set; }
        public decimal TotalSpent { get; set; }
        public int InvoiceCount { get; set; }
    }
}
//namespace MyInvoice.ViewModel
//{
//    public class DashboardViewModel
//    {
//        // Summary
//        public decimal TotalSales { get; set; }
//        public int TotalInvoices { get; set; }
//        public int TotalProducts { get; set; }
//        public int LowStockCount { get; set; }

//        // Filters
//        public string SelectedFilter { get; set; } = "month";

//        // Tables
//        public List<RecentInvoiceVM> RecentInvoices { get; set; } = new();
//        public List<TopCustomerVM> TopCustomers { get; set; } = new();

//        // 🔥 Chart Data
//        public List<string> TopCustomerNames { get; set; } = new();
//        public List<decimal> TopCustomerTotals { get; set; } = new();
//    }

//    //public class DashboardViewModel
//    //{
//    //    public string SelectedFilter { get; set; }

//    //    public decimal TotalSales { get; set; }
//    //    public int TotalInvoices { get; set; }
//    //    public int TotalProducts { get; set; }
//    //    public int LowStockCount { get; set; }

//    //    public List<RecentInvoiceViewModel> RecentInvoices { get; set; }

//    //           public List<TopCustomerVM> TopCustomers { get; set; } = new();
//    //    //🔥 Chart Data
//    //    public List<string> TopCustomerNames { get; set; } = new();
//    //        public List<decimal> TopCustomerTotals { get; set; } = new();

//    //}

//    //public class RecentInvoiceViewModel
//    //{
//    //    public int InvoiceId { get; set; }
//    //    public string InvoiceNumber { get; set; }
//    //    public string CustomerName { get; set; }
//    //    public DateTime InvoiceDate { get; set; }
//    //    public decimal GrandTotal { get; set; }
//    //}


//    //public class TopCustomerVM
//    //{
//    //    public string CustomerName { get; set; }
//    //    public decimal TotalSpent { get; set; }
//    //    public int InvoiceCount { get; set; }
//    //}
//}


//namespace MyInvoice.ViewModel
//{
//    public class DashboardViewModel
//    {
//        public decimal TotalSales { get; set; }
//        public int TotalInvoices { get; set; }

//        public List<string> Months { get; set; } = new();
//        public List<decimal> MonthlySales { get; set; } = new();

//        public List<string> CustomerNames { get; set; } = new();
//        public List<decimal> CustomerTotals { get; set; } = new();

//        public List<string> Days { get; set; } = new();
//        public List<decimal> DailySales { get; set; } = new();
//    }
//}
