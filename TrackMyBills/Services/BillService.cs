using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackMyBills.Models;

namespace TrackMyBills.Services
{
    public class BillService : IBillService
    {
        public Guid Save(Models.BillModel bill)
        {
            var billContext = new BillContext();
            billContext.Bills.Add(bill);
            billContext.SaveChanges();
            return bill.ID;   
        }

        public void UpdateBillAmount(Guid billId, string amount)
        {
            using (var ctx = new BillContext())
            {
                var thisBill = ctx.Bills.FirstOrDefault(b => b.ID == billId);
                if (thisBill != null)
                {
                    thisBill.Amount = amount;
                    ctx.SaveChanges();
                }
            }
        }

        public void UpdateBillDueDate(Guid billId, DateTime dueDate)
        {
            using (var ctx = new BillContext())
            {
                var thisBill = ctx.Bills.FirstOrDefault(b => b.ID == billId);
                if (thisBill != null)
                {
                    thisBill.DueOn = dueDate;
                    ctx.SaveChanges();
                }
            }
        }

        public IEnumerable<Models.BillModel> GetCurrentBillsByUserKey(string userKey)
        {
            var billContext = new BillContext();
            return billContext.Bills.Include("BilledFrom").Where(b=>!b.Paid).OrderBy(b=>b.DueOn).AsEnumerable();
        }

        public IEnumerable<Biller> GetBillers()
        {
            var billContext = new BillContext();
            return billContext.Billers.AsEnumerable();
        }

        public Biller GetBillerById(Guid billerId)
        {
            var billContext = new BillContext();
            return billContext.Billers.FirstOrDefault(b => b.ID == billerId);
        }

        public bool BillerExists(string billerName)
        {
            var billContext = new BillContext();
            return billContext.Billers.Any(b=>b.Name == billerName);
        }

        public Guid SaveBiller(string billerName)
        {
            var billerId = Guid.NewGuid();
            var billContext = new BillContext();
            billContext.Billers.Add(new Biller { ID = billerId, Name = billerName });
            billContext.SaveChanges();
            return billerId;   
        }

        public void PayBill(Guid billId)
        {
            var billContext = new BillContext();
            var thisBill = billContext.Bills.FirstOrDefault(b => b.ID == billId);

            thisBill.Paid = true;

            billContext.SaveChanges();
        }

        public void DeleteBill(Guid billId)
        {
            var billContext = new BillContext();
            var thisBill = billContext.Bills.FirstOrDefault(b => b.ID == billId);

            billContext.Bills.Remove(thisBill);

            billContext.SaveChanges();
        }

        public void SaveBillerOccurrence(BillOccurrence occurrence)
        {
            var billContext = new BillContext();
            billContext.BillOccurrences.Add(occurrence);
            billContext.SaveChanges();
        }
        
        public BillOccurrence GetBillOccurrencesByBillId(Guid billId)
        {
            var bill = GetBillById( billId);
            var billContext = new BillContext();
            return billContext.BillOccurrences.FirstOrDefault(b => b.BillerID == bill.BillerID);
        }

        public BillModel GetBillById(Guid billId)
        {
            var billContext = new BillContext();
            return billContext.Bills.FirstOrDefault(b => b.ID == billId);
        }
        
        public IEnumerable<BillModel> GetPaidBillsByUserKey(string userKey)
        {
            var billContext = new BillContext();
            return billContext.Bills.Include("BilledFrom").Where(b => b.Paid).OrderBy(b => b.DueOn).AsEnumerable();
        }
    }
}