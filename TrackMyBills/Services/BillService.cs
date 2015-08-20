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
                var newBillId = Guid.NewGuid();
				await billContext.ExecuteAsync(
					"insert into BillModels select @BillID, @BillerID, @DueOn, @Amount, @Paid, @EnteredOn",
					new
					{
                        BillID = newBillId,
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
				await ctx.ExecuteAsync("update BillModels set Amount = @Amount where ID = @BillID",
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
				await ctx.ExecuteAsync("update BillModels set DueOn = @DueOn where ID = @BillID",
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
				var bills = (await ctx.QueryAsync<BillModel>("select * from BillModels where Paid = 0")).OrderBy(b=>b.DueOn);
                var mappings = await ctx.QueryAsync<Biller>("select * from Billers");
                foreach (var bill in bills)
                {
                    bill.BilledFrom = mappings.FirstOrDefault(m => m.ID == bill.BillerID);
                }
                return bills;
			}
		}

		public async Task<IEnumerable<Biller>> GetBillersAsync()
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return await ctx.QueryAsync<Biller>("select * from Billers");
			}
		}

		public async Task<Biller> GetBillerByIdAsync(Guid billerId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<Biller>("select * from Billers where ID = @ID",
					new { ID = billerId })).FirstOrDefault();
			}
		}

		public async Task<bool> BillerExistsAsync(string billerName)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<Biller>("select * from Billers where Name = @Name",
					new { Name = billerName })).Any();
			}
		}

		public async Task<Guid> SaveBillerAsync(string billerName)
		{
			var billerId = Guid.NewGuid();
			using (var billContext = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				var newBillerId = await billContext.ExecuteScalarAsync<Guid>(
					"insert into Billers select @BillerID, @BillerName",
					new
					{
						BillerID = billerId,
						BillerName = billerName
					});

				return billerId;
			}
		}

		public async Task<dynamic> PayBillAsync(Guid billId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				await ctx.ExecuteAsync("update BillModels set Paid = 1 where ID = @BillID",
					new
					{
						BillID = billId
					});

			}
			return true;
		}

		public async Task<dynamic> DeleteBillAsync(Guid billId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				await ctx.ExecuteAsync("delete from BillModels where ID = @BillID",
					new
					{
						BillID = billId
					});

			}
			return null;
		}

		public async Task<dynamic> SaveBillerOccurrenceAsync(BillOccurrence occurrence)
		{
			using (var billContext = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				await billContext.ExecuteAsync(
                    "insert into BillOccurrences select @ID, @DayOfWeekDue, @DayOfMonthDue, @BillerID, @Frequency",
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
		}

		public async Task<BillOccurrence> GetBillOccurrencesByBillIdAsync(Guid billId)
		{
			var bill = await GetBillByIdAsync(billId);
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<BillOccurrence>("select * from BillOccurrences where BillerID = @BillerID",
					new { BillerID = bill.BillerID })).FirstOrDefault();
			}
		}

		public async Task<BillModel> GetBillByIdAsync(Guid billId)
		{
			using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
			{
				return (await ctx.QueryAsync<BillModel>("select * from BillModels where ID = @ID",
					new { ID = billId })).FirstOrDefault();
			}
		}

		public async Task<IEnumerable<BillModel>> GetPaidBillsByUserKeyAsync(string userKey)
		{
            using (var ctx = new SqlConnection(ConfigurationManager.ConnectionStrings["TMB"].ConnectionString))
            {
                var bills = (await ctx.QueryAsync<BillModel>("select * from BillModels where Paid = 1")).OrderBy(b => b.DueOn);
                var mappings = await ctx.QueryAsync<Biller>("select * from Billers");
                foreach (var bill in bills)
                {
                    bill.BilledFrom = mappings.FirstOrDefault(m => m.ID == bill.BillerID);
                }
                return bills;
            }
        }
	}
}