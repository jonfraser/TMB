using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackMyBills.Models;

namespace TrackMyBills.Services
{
    public interface IBudgetService
    {
        IEnumerable<BudgetModel> GetBudgetByUserKey(string userKey);

        Guid Save(BudgetModel budgetModel);

        IEnumerable<DateTime> GetNextFivePayPeriods(string userKey);

        IEnumerable<DateTime> GetPreviousPayPeriods(string userKey, int howManyPeriods);
    }
}