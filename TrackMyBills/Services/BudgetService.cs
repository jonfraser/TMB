using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TrackMyBills.Services
{
    public class BudgetService : IBudgetService
    {
        public IEnumerable<Models.BudgetModel> GetBudgetByUserKey(string userKey)
        {
            throw new NotImplementedException();
        }

        public Guid Save(Models.BudgetModel budgetModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DateTime> GetNextFivePayPeriods(string userKey)
        {
            //this would usually:
            //look up pay date and frequency in database
            //work out the pay cycle
            //return next 5 after today in the cycle

            var payDate = DateTime.ParseExact(ConfigurationManager.AppSettings["PayDate"], "dd/MM/yyyy", null);
            var foundNextPay = false;

            while (!foundNextPay)
            {
                if (payDate.Date >= DateTime.UtcNow.Date.AddHours(10)) //brisbane is utc + 10
                {
                    foundNextPay = true;
                }
                else
                {
                    payDate = payDate.AddDays(14);
                }
            }

            var pays = new List<DateTime>() { payDate };

            for (int i = 0; i < 4; i++)
            {
                payDate = payDate.AddDays(14);
                pays.Add(payDate);
            }

            return pays;
        }


        public IEnumerable<DateTime> GetPreviousPayPeriods(string userKey, int howManyPeriods)
        {
            var payDate = DateTime.ParseExact(ConfigurationManager.AppSettings["PayDate"], "dd/MM/yyyy", null);
            var foundLastPay = false;

            while (!foundLastPay)
            {
                if (payDate.Date >= DateTime.UtcNow.Date)
                {
                    foundLastPay = true;
                }
                else
                {
                    payDate = payDate.AddDays(14);
                }
            }

            payDate = payDate.AddDays(-14);

            var pays = new List<DateTime>() { payDate };

            for (int i = 0; i < howManyPeriods - 1; i++)
            {
                payDate = payDate.AddDays(-14);
                pays.Add(payDate);
            }

            return pays;
        }
    }
}