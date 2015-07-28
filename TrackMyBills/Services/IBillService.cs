using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TrackMyBills.Models;

namespace TrackMyBills.Services
{
    public interface IBillService
    {
        Task<Guid> SaveAsync(BillModel bill);

        Task<IEnumerable<BillModel>> GetCurrentBillsByUserKeyAsync(string userKey);

        Task<IEnumerable<Biller>> GetBillersAsync();

        Task<Biller> GetBillerByIdAsync(Guid billerId);

        Task<bool> BillerExistsAsync(string billerName);

        Task<Guid> SaveBillerAsync(string billerName);

        Task<dynamic> PayBillAsync(Guid billId);

        Task<dynamic> DeleteBillAsync(Guid billId);

		Task<dynamic> SaveBillerOccurrenceAsync(BillOccurrence newBillOccurrence);

        Task<BillOccurrence> GetBillOccurrencesByBillIdAsync(Guid billId);

        Task<BillModel> GetBillByIdAsync(Guid billId);

		Task<dynamic> UpdateBillAmountAsync(Guid billId, string amount);

		Task<dynamic> UpdateBillDueDateAsync(Guid billId, DateTime dueDate);

        Task<IEnumerable<BillModel>> GetPaidBillsByUserKeyAsync(string userKey);
    }
}