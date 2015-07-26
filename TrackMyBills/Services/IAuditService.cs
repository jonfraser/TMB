using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackMyBills.Models;

namespace TrackMyBills.Services
{
    public interface IAuditService
    {
        void Audit(string auditText, AuditType type);

        IEnumerable<Audit> GetAuditMessages(DateTime from, DateTime to, AuditType? type);

        IEnumerable<Audit> GetAuditMessages(int count, AuditType? type);
    }
}