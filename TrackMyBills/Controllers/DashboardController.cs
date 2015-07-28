using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackMyBills.ActionFilters;
using TrackMyBills.Services;
using TrackMyBills.Models;
using System.Threading.Tasks;

namespace TrackMyBills.Controllers
{
    public class DashboardController : ControllerBase
    {
        IBudgetService _budgetService = null;
        IBillService _billService = null;
        IAuditService _auditService = null;

        public DashboardController(IBudgetService budgetService, IBillService billService, IAuditService auditService)
            : base()
        {
            this._budgetService = budgetService;
            this._billService = billService;
            this._auditService = auditService;
        }

        public DashboardController()
            : base()
        {
            this._billService = new BillService();
            this._budgetService = new BudgetService();
            this._auditService = new AuditService();
        }

        [CheckLoggedIn]
        public ActionResult Dashboard()
        {
            ViewBag.UserKey = UserInfo.UserKey;
            return View("Dashboard");
        }

        [HttpPost]
        public ActionResult Dashboard(string id)
        {
            Session.Add("AvoidLogin", true);
            return RedirectToAction("AutoLogin", "Account", new { id = id });
        }

        [CheckLoggedIn]
        public ActionResult Budget()
        {
            var budgetItems = this._budgetService.GetBudgetByUserKey(UserInfo.UserKey);
            return View("Budget", budgetItems);
        }

        public JsonResult SaveBudget(BudgetModel budget)
        {
            var newId = this._budgetService.Save(budget);
            return Json(newId);
        }

        [CheckLoggedIn]
        public ActionResult Bill()
        {
            var currentBills = this._billService.GetCurrentBillsByUserKeyAsync(UserInfo.UserKey);
            IEnumerable<DateTime> nextFivePayDates = this._budgetService.GetNextFivePayPeriods(UserInfo.UserKey);
            ViewBag.PayPeriods = nextFivePayDates.ToList();
            return View("Bill", currentBills);
        }

        [CheckLoggedIn]
        public async Task<ActionResult> BillMobile()
        {
            var currentBills = await this._billService.GetCurrentBillsByUserKeyAsync(UserInfo.UserKey);
            var nextFivePayDates = this._budgetService.GetNextFivePayPeriods(UserInfo.UserKey).ToList();
            ViewBag.PayPeriods = nextFivePayDates;

            var billGroups = new Dictionary<int, List<BillModel>>{{0, new List<BillModel>()},
                                                                  {1, new List<BillModel>()},
                                                                  {2, new List<BillModel>()},
                                                                  {3, new List<BillModel>()},
                                                                  {4, new List<BillModel>()},
                                                                  {5, new List<BillModel>()}};
            foreach (var bill in currentBills)
            {
                for (int i = 0; i < nextFivePayDates.Count; i++)
                {
                    if (bill.DueOn <= nextFivePayDates[i])
                    {
                        billGroups[i].Add(bill);
                        break;
                    }
                }
            }

            return View("Bill-Mobile", billGroups);
        }

        [CheckLoggedIn]
        public async Task<ActionResult> ViewPaidBills()
        {
            var paidBills = await this._billService.GetPaidBillsByUserKeyAsync(UserInfo.UserKey);
            var previousXPayPeriods = this._budgetService.GetPreviousPayPeriods(UserInfo.UserKey, 10).OrderBy(p => p).ToList();

            var payPeriods = new Dictionary<DateTime, decimal>();
            for (int i = 9; i > 0; i--)
            {
                var billsPaidInPeriod = paidBills.Where(b => b.BilledFrom.Name != "Mortgage" && b.DueOn >= previousXPayPeriods[i - 1] && b.DueOn < previousXPayPeriods[i]).Sum(b => decimal.Parse(b.Amount));
                payPeriods.Add(previousXPayPeriods[i], billsPaidInPeriod);
            }

            ViewBag.PayPeriodSummary = payPeriods;
            return View("ViewPaidBills");
        }

        [CheckLoggedIn]
        public async Task<JsonResult> SaveBill(BillSaveModel bill)
        {
            var biller = await this._billService.GetBillerByIdAsync(bill.BillerId);
            var newBill = new BillModel
            {
                Amount = bill.Amount,
                //BilledFrom = biller,
                BillerID = biller.ID,
                DueOn = bill.DueDate,
                EnteredOn = DateTime.Now,
                ID = Guid.NewGuid()
            };
            var newId = await this._billService.SaveAsync(newBill);

            if (!string.IsNullOrEmpty(bill.Repeats))
            {
                if (await this._billService.GetBillOccurrencesByBillIdAsync(newId) == null)
                {
                    var newBillOccurrence = new BillOccurrence
                    {
                        ID = Guid.NewGuid(),
                        BillerID = biller.ID,
                        Frequency = (int)GetFrequencyFromRepeatCode(bill.Repeats)
                    };
                    await this._billService.SaveBillerOccurrenceAsync(newBillOccurrence);
                }
            }

            this._auditService.Audit(string.Format("{0} bill added for {1}.",biller.Name, bill.Amount), AuditType.BillAdded);

            return Json(newId);
        }

        [HttpPost]
        [CheckLoggedIn]
        public async Task<JsonResult> UpdateBillAmount(Guid billId, string amount)
        {
            try
            {
                var bill = await this._billService.GetBillByIdAsync(billId);
                var biller = await this._billService.GetBillerByIdAsync(bill.BillerID);
                await this._billService.UpdateBillAmountAsync(billId, amount);
                this._auditService.Audit(string.Format("{0} bill amount updated from {1} to {2}.", biller.Name, bill.Amount, amount), AuditType.BillEdited);

                return Json(null);
            }
            catch (Exception exc)
            {
                return Json(exc.ToString());
            }
        }

        [HttpPost]
        [CheckLoggedIn]
        public async Task<JsonResult> UpdateBillDueDate(Guid billId, DateTime dueDate)
        {
            try
            {
                var bill = await this._billService.GetBillByIdAsync(billId);
                var biller = await this._billService.GetBillerByIdAsync(bill.BillerID);
                
                await this._billService.UpdateBillDueDateAsync(billId, dueDate);

                this._auditService.Audit(string.Format("{0} bill due date updated from {1} to {2}.", biller.Name, bill.DueOn.ToString("dd/MM/yyyy"), dueDate.ToString("dd/MM/yyyy")), AuditType.BillEdited);

                return Json(null);
            }
            catch (Exception exc)
            {
                return Json(exc.ToString());
            }
        }

        private BillFrequency GetFrequencyFromRepeatCode(string repeatCode)
        {
            var frequencyDictionary = new Dictionary<string, BillFrequency>
            {
                {"W", BillFrequency.Weekly},
                {"F", BillFrequency.Fortnightly},
                {"M", BillFrequency.Monthly},
                {"Q", BillFrequency.Quarterly},
                {"Y", BillFrequency.Yearly}
            };
            return frequencyDictionary[repeatCode];
        }

        [HttpGet]
        [CheckLoggedIn]
        public JsonResult GetBillers()
        {
            return Json(this._billService.GetBillersAsync(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckLoggedIn]
        public async Task<JsonResult> AddNewBiller(string billerName)
        {
            if (string.IsNullOrEmpty(billerName))
            {
                return Json(null);
            }

            if (await this._billService.BillerExistsAsync(billerName))
            {
                return Json(null);
            }

            var billerId = await this._billService.SaveBillerAsync(billerName);

            this._auditService.Audit(string.Format("Biller added - {0}.", billerName), AuditType.BillerAdded);

            return Json(billerId);
        }

        [HttpPost]
        [CheckLoggedIn]
        public async Task<JsonResult> PayBill(Guid billId)
        {
            if (billId == Guid.Empty)
            {
                throw new ArgumentNullException("billId");
            }

            await this._billService.PayBillAsync(billId);

            var oldBill = await this._billService.GetBillByIdAsync(billId);
            var biller = await this._billService.GetBillerByIdAsync(oldBill.BillerID);
            this._auditService.Audit(string.Format("{0} bill owing {1} due on {2} was paid.", biller.Name, oldBill.Amount, oldBill.DueOn.ToString("dd/MM/yyyy")), AuditType.BillPaid);

            BillOccurrence repeats = await this._billService.GetBillOccurrencesByBillIdAsync(billId);

            if (repeats != null && repeats.Frequency.HasValue)
            {
                
                await this._billService.SaveAsync(new BillModel
                {
                    ID = Guid.NewGuid(),
                    EnteredOn = DateTime.UtcNow,
                    BillerID = oldBill.BillerID,
                    Amount = oldBill.Amount,
                    DueOn = CalculateDueDateBasedOnRepeatFrequency(oldBill.DueOn, (BillFrequency)repeats.Frequency)
                });
                this._auditService.Audit(string.Format("{0} bill added for {1}.", biller.Name, oldBill.Amount), AuditType.BillAdded);

            }

            return Json(true);
        }

        private DateTime CalculateDueDateBasedOnRepeatFrequency(DateTime billDate, BillFrequency billFrequency)
        {
            var newDueDate = new Dictionary<BillFrequency, DateTime>
            {
                {BillFrequency.Weekly, billDate.AddDays(7)},
                {BillFrequency.Fortnightly, billDate.AddDays(14)},
                {BillFrequency.Monthly, billDate.AddMonths(1)},
                {BillFrequency.Quarterly, billDate.AddMonths(3)},
                {BillFrequency.Yearly, billDate.AddYears(1)}
            };
            return newDueDate[billFrequency];
        }

        [HttpPost]
        [CheckLoggedIn]
        public async Task<JsonResult> DeleteBill(Guid billId)
        {
            if (billId == Guid.Empty)
            {
                throw new ArgumentNullException("billId");
            }

            await this._billService.DeleteBillAsync(billId);

            var oldBill = await this._billService.GetBillByIdAsync(billId);
            var biller = await this._billService.GetBillerByIdAsync(oldBill.BillerID);
            this._auditService.Audit(string.Format("{0} bill owing {1} due on {2} was deleted.", biller.Name, oldBill.Amount, oldBill.DueOn.ToString("dd/MM/yyyy")), AuditType.BillPaid);

            return Json(true);
        }

        [CheckLoggedIn]
        public ActionResult AuditLog()
        {
            var audits = this._auditService.GetAuditMessages(100, null);
            return View("AuditLog", audits);
        }

    }
}
