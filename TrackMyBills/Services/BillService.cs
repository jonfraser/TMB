using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TrackMyBills.Models;
using Dapper;
using System.Threading.Tasks;

namespace TrackMyBills.Services
{
	public class BillService : IBillService
	{
		public async Task<Guid> SaveAsync(Models.BillModel bill)
		{
			using (var billContext = new SqlConnection(""))
			{
				var newBillId = await billContext.ExecuteScalarAsync<Guid>(
					"insert into Bill select @BillerID, @DueOn, @Amount, @Paid, @EnteredOn; select @@IDENTITY",
					new
					{
						BillerID = bill.BillerID,
						DueOn = bill.DueOn,
						Amount = bill.Amount,
						Paid = bill.Paid,
						EnteredOn = bill.EnteredOn
					});

				return newBillId;
			}
		}

		public async Task<dynamic> UpdateBillAmountAsync(Guid billId, string amount)
		{
			using (var ctx = new SqlConnection())
			{
				await ctx.QueryAsync("update Bill set Amount = @Amount where ID = @BillID",
					new
					{
						Amount = amount,
						BillID = billId
					});
			}
			return null;
		}

		public async Task<dynamic> UpdateBillDueDateAsync(Guid billId, DateTime dueDate)
		{
			using (var ctx = new SqlConnection())
			{
				await ctx.QueryAsync("update Bill set DueOn = @DueOn where ID = @BillID",
					new
					{
						DueOn = dueDate,
						BillID = billId
					});

			}
			return null;
		}

		public async Task<IEnumerable<Models.BillModel>> GetCurrentBillsByUserKeyAsync(string userKey)
		{
			using (var ctx = new SqlConnection())
			{
				return await ctx.QueryAsync<BillModel>("select * from Bill where not Paid order by DueOn");
			}
			//return billContext.Bills.Include("BilledFrom").Where(b => !b.Paid).OrderBy(b => b.DueOn).AsEnumerable();
		}

		public async Task<IEnumerable<Biller>> GetBillersAsync()
		{
			return null;
			//var billContext = new BillContext();
			//return billContext.Billers.AsEnumerable();
		}

		public async Task<Biller> GetBillerByIdAsync(Guid billerId)
		{
			return null;
			//var billContext = new BillContext();
			//return billContext.Billers.FirstOrDefault(b => b.ID == billerId);
		}

		public async Task<bool> BillerExistsAsync(string billerName)
		{
			return true;
			//var billContext = new BillContext();
			//return billContext.Billers.Any(b => b.Name == billerName);
		}

		public async Task<Guid> SaveBillerAsync(string billerName)
		{
			return Guid.Empty;
			//var billerId = Guid.NewGuid();
			//var billContext = new BillContext();
			//billContext.Billers.Add(new Biller { ID = billerId, Name = billerName });
			//billContext.SaveChanges();
			//return billerId;
		}

		public async Task<dynamic> PayBillAsync(Guid billId)
		{
			return true;
			//var billContext = new BillContext();
			//var thisBill = billContext.Bills.FirstOrDefault(b => b.ID == billId);

			//thisBill.Paid = true;

			//billContext.SaveChanges();
		}

		public async Task<dynamic> DeleteBillAsync(Guid billId)
		{
			return null;
			//var billContext = new BillContext();
			//var thisBill = billContext.Bills.FirstOrDefault(b => b.ID == billId);

			//billContext.Bills.Remove(thisBill);

			//billContext.SaveChanges();
		}

		public async Task<dynamic> SaveBillerOccurrenceAsync(BillOccurrence occurrence)
		{
			return null;
			//var billContext = new BillContext();
			//billContext.BillOccurrences.Add(occurrence);
			//billContext.SaveChanges();
		}

		public async Task<BillOccurrence> GetBillOccurrencesByBillIdAsync(Guid billId)
		{
			return null;
			//var bill = GetBillByIdAsync(billId);
			//var billContext = new BillContext();
			//return billContext.BillOccurrences.FirstOrDefault(b => b.BillerID == bill.BillerID);
		}

		public async Task<BillModel> GetBillByIdAsync(Guid billId)
		{
			return null;
			//var billContext = new BillContext();
			//return billContext.Bills.FirstOrDefault(b => b.ID == billId);
		}

		public async Task<IEnumerable<BillModel>> GetPaidBillsByUserKeyAsync(string userKey)
		{
			return null;
			//var billContext = new BillContext();
			//return billContext.Bills.Include("BilledFrom").Where(b => b.Paid).OrderBy(b => b.DueOn).AsEnumerable();
		}
	}
}