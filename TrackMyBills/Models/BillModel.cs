using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TrackMyBills.Models
{
    public class BillModel
    {
        public Guid ID { get; set; }
        public Guid BillerID { get; set; }
        public virtual Biller BilledFrom { get; set; }
        public DateTime DueOn { get; set; }
        public string Amount { get; set; }
        public bool Paid { get; set; }
        public DateTime EnteredOn { get; set; }
    }
    public class BillSaveModel
    {
        public Guid BillerId { get; set; }
        public DateTime DueDate { get; set; }
        public string Amount { get; set; }
        public string Repeats { get; set; }
    }
    
    public class Biller
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
    }
    
    public class BillOccurrence
    {
        public Guid ID { get; set; }
        public Guid BillerID { get; set; }
        public virtual Biller BilledFrom { get; set; }
        public int? DayOfWeekDue { get; set; }
        public int? DayOfMonthDue { get; set; }
        public int? Frequency { get; set; }
    }
    
    public enum BillFrequency
    {
        OneOff = 1,
        Weekly = 2,
        Fortnightly = 3,
        Monthly = 4,
        Quarterly = 5,
        Yearly = 6
    }

	//public class BillContext : DbContext
	//{
	//	public DbSet<BillModel> Bills { get; set; }
	//	public DbSet<Biller> Billers { get; set; }
	//	public DbSet<BillOccurrence> BillOccurrences { get; set; }
	//	public DbSet<Audit> Audits { get; set; }
	//}

}