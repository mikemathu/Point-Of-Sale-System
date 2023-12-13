using PointOfSaleSystem.Service.Dtos.Dashboard;

namespace PointOfSaleSystem.Service.Services.Dashboard
{
    public class ReportsPaths
    {
        public string GetAccountsReportPath(DashboardReportDto dashboardReportDto)
        {
            string reportLocation = "";

            switch (dashboardReportDto.ReportID)
            {
                case 1:
                    reportLocation = @"TrialBalance/TrialBalanceDetailed.frx";
                    break;
                case 2:
                    reportLocation = @"TrialBalance/TrialBalanceSummary.frx";
                    break;
                case 3:
                    reportLocation = @"IncomeStatement/IncomeStatementDetailed.frx";
                    break;
                case 4:
                    reportLocation = @"IncomeStatement/IncomeStatementSummary.frx";
                    break;
                case 5:
                    reportLocation = @"IncomeStatement/ComprehensiveIncomeStatement.frx";
                    break;
                case 6:
                    reportLocation = @"BalanceSheet/BalanceSheetDetailed.frx";
                    break;
                case 7:
                    reportLocation = @"BalanceSheet/BalanceSheetSummary.frx";
                    break;
                case 8:
                    reportLocation = @"BalanceSheet/ComprehensiveBalanceSheetDetailed.frx";
                    break;
                case 9:
                    reportLocation = @"CashFlowStatements/CashFlowStatement.frx";
                    break;
                case 10:
                    reportLocation = @"CashFlowStatements/ComprehensiveCashFlowStatement.frx";
                    break;
                case 11:
                    reportLocation = @"GeneralLedger/GeneralLedgerDetailed.frx";
                    break;
                case 12:
                    reportLocation = @"GeneralLedger/LedgerSubAccountHistory.frx";
                    break;
                case 13:
                    reportLocation = @"GeneralLedger/LedgerAccountHistory.frx";
                    break;
                case 14:
                    reportLocation = @"GeneralLedger/JournalVouchers.frx";
                    break;
                case 15:
                    reportLocation = @"Expenses/AllExpensesDetailed.frx";
                    break;
                case 16:
                    reportLocation = @"Expenses/AllExpensesSummary.frx";
                    break;
                default:
                    //
                    break;
            }

            return reportLocation;
        }
        public string GetInventoryReportPath(DashboardReportDto dashboardReportDto)
        {
            string reportLocation = "";

            switch (dashboardReportDto.ReportID)
            {
                case 1:
                    reportLocation = @"BelowReorderLevel.frx";
                    break;
                case 2:
                    reportLocation = @"OutOfStock.frx";
                    break;
                case 3:
                    reportLocation = @"NearExpiry.frx";
                    break;
                case 4:
                    reportLocation = @"StockExpiry.frx";
                    break;
                case 5:
                    reportLocation = @"FastMovingItems.frx";
                    break;
                default:
                    //
                    break;
            }

            return reportLocation;
        }

    }
}
