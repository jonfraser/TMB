using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackMyBills.Models;

namespace TrackMyBills.Services
{
    public interface IBillService
    {
        Guid Save(BillModel bill);

        IEnumerable<BillModel> GetCurrentBillsByUserKey(string userKey);

        IEnumerable<Biller> GetBillers();

        Biller GetBillerById(Guid billerId);

        bool BillerExists(string billerName);

        Guid SaveBiller(string billerName);

        void PayBill(Guid billId);

        void DeleteBill(Guid billId);

        void SaveBillerOccurrence(BillOccurrence newBillOccurrence);

        BillOccurrence GetBillOccurrencesByBillId(Guid billId);

        BillModel GetBillById(Guid billId);

        void UpdateBillAmount(Guid billId, string amount);

        void UpdateBillDueDate(Guid billId, DateTime dueDate);

        IEnumerable<BillModel> GetPaidBillsByUserKey(string userKey);
    }
}