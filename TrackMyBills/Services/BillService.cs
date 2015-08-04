using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TrackMyBills.Models;
using Dapper;
using System.Threading.Tasks;
using System.Configuration;

namespace TrackMyBills.Services
{
	public class BillService : IBillService
	{
		public async Task<Guid> SaveAsync(Models.BillModel bill)
		{
			using (var billContext = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
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
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
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
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
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
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<BillModel>("select * from BillModels where Paid = 0")).OrderBy(b=>b.DueOn);
			}
			//return billContext.Bills.Include("BilledFrom").Where(b => !b.Paid).OrderBy(b => b.DueOn).AsEnumerable();
		}

		public async Task<IEnumerable<Biller>> GetBillersAsync()
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return await ctx.QueryAsync<Biller>("select * from Biller");
			}
			//var billContext = new BillContext();
			//return billContext.Billers.AsEnumerable();
		}

		public async Task<Biller> GetBillerByIdAsync(Guid billerId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<Biller>("select * from Biller where ID = @ID",
					new { ID = billerId })).FirstOrDefault();
			}
			//var billContext = new BillContext();
			//return billContext.Billers.FirstOrDefault(b => b.ID == billerId);
		}

		public async Task<bool> BillerExistsAsync(string billerName)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<Biller>("select * from Biller where Name = @Name",
					new { Name = billerName })).Any();
			}
			//var billContext = new BillContext();
			//return billContext.Billers.Any(b => b.Name == billerName);
		}

		public async Task<Guid> SaveBillerAsync(string billerName)
		{
			var billerId = Guid.NewGuid();
			using (var billContext = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				var newBillerId = await billContext.ExecuteScalarAsync<Guid>(
					"insert into Biller select @BillerID, @BillerName; select @@IDENTITY",
					new
					{
						BillerID = billerId,
						BillerName = billerName
					});

				return billerId;
			}
			//var billerId = Guid.NewGuid();
			//var billContext = new BillContext();
			//billContext.Billers.Add(new Biller { ID = billerId, Name = billerName });
			//billContext.SaveChanges();
			//return billerId;
		}

		public async Task<dynamic> PayBillAsync(Guid billId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				await ctx.QueryAsync("update Bill set Paid = 1 where ID = @BillID",
					new
					{
						ID = billId
					});

			}
			return true;
			//var billContext = new BillContext();
			//var thisBill = billContext.Bills.FirstOrDefault(b => b.ID == billId);

			//thisBill.Paid = true;

			//billContext.SaveChanges();
		}

		public async Task<dynamic> DeleteBillAsync(Guid billId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				await ctx.QueryAsync("delete from Bill where ID = @BillID",
					new
					{
						ID = billId
					});

			}
			return null;
			//var billContext = new BillContext();
			//var thisBill = billContext.Bills.FirstOrDefault(b => b.ID == billId);

			//billContext.Bills.Remove(thisBill);

			//billContext.SaveChanges();
		}

		public async Task<dynamic> SaveBillerOccurrenceAsync(BillOccurrence occurrence)
		{
			using (var billContext = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				await billContext.ExecuteAsync(
					"insert into BillOccurrence select @ID, @BillerID, @DayOfMonthDue, @DayOfWeekDue, @Frequency",
					new
					{
						ID = occurrence.ID,
						BillerID = occurrence.BillerID,
						DayOfMonthDue = occurrence.DayOfMonthDue,
						DayOfWeekDue = occurrence.DayOfWeekDue,
						Frequency = occurrence.Frequency
					});

			}
			return null;
			//var billContext = new BillContext();
			//billContext.BillOccurrences.Add(occurrence);
			//billContext.SaveChanges();
		}

		public async Task<BillOccurrence> GetBillOccurrencesByBillIdAsync(Guid billId)
		{
			var bill = await GetBillByIdAsync(billId);
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<BillOccurrence>("select * from BillOccurrence where BillerID = @BillerID",
					new { BillerID = bill.BillerID })).FirstOrDefault();
			}
			//var billContext = new BillContext();
			//return billContext.BillOccurrences.FirstOrDefault(b => b.BillerID == bill.BillerID);
		}

		public async Task<BillModel> GetBillByIdAsync(Guid billId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<BillModel>("select * from BillModel where ID = @ID",
					new { ID = billId })).FirstOrDefault();
			}
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