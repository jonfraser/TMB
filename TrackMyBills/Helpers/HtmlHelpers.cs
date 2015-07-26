using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrackMyBills.Helpers
{
    public static class HtmlHelpers
    {
        public static string GetClassForDaysRemaining(this HtmlHelper html, int days)
        {
            if (days <= 3)
            {
                return "BillDueVerySoon";
            }
            else if (days <= 7)
            {
                return "BillDueSoon";
            }
            else
            {
                return "";
            }
        }

        public static string GetClassForDaysRemaining(this HtmlHelper html, DateTime dueDate, List<DateTime> payPeriods)
        {
            if (dueDate <= payPeriods[0])
            {
                return "BillDueVerySoon";
            }
            else if (dueDate <= payPeriods[1])
            {
                return "BillDueSoon";
            }
            else
            {
                return "";
            }
        }

        public static MvcHtmlString GetPayPeriod(this HtmlHelper html, DateTime dueDate, List<DateTime> payPeriods)
        {
            string message = "";
            if (dueDate <= payPeriods[0])
            {
                message = "this pay period - by " + payPeriods[0].ToString("dd MMM yy");
            }
            else if (dueDate <= payPeriods[1])
            {
                message = "next pay period - by " + payPeriods[1].ToString("dd MMM yy");
            }
            else
            {
                for (int i = 2; i < payPeriods.Count; i++)
                {
                    if (dueDate <= payPeriods[i])
                    {
                        message = "in " + i.ToString() + " pay periods - by " + payPeriods[i].ToString("dd MMM yy");
                        break;
                    }
                }
            }

            return new MvcHtmlString(message);
        }
    }
}