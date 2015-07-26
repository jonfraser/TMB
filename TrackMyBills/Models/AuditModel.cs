using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.DynamicData;

namespace TrackMyBills.Models
{
    public class Audit
    {
        public Guid Id { get; set; }
        public DateTime AuditDate { get; set; }
        public int AuditType { get; set; }
        public string AuditText { get; set; }
    }

    public enum AuditType
    {
        BillerAdded = 1,
        BillAdded,
        BillEdited,
        BillPaid,
        BillDeleted
    }
}
