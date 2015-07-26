using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackMyBills.Models;

namespace TrackMyBills.Services
{
    public class AuditService : IAuditService
    {
        public void Audit(string auditText, AuditType type)
        {
            var auditContext = new BillContext();
            auditContext.Audits.Add(new Audit
            {
                Id = Guid.NewGuid(),
                AuditDate = DateTime.UtcNow,
                AuditText = auditText,
                AuditType = (int)type
            });
            auditContext.SaveChanges();
        }

        public IEnumerable<Audit> GetAuditMessages(DateTime from, DateTime to, AuditType? type)
        {
            var auditContext = new BillContext();
            var audits = auditContext.Audits.Where(a=>a.AuditDate >= from && a.AuditDate <= to);
            if (type.HasValue)
            {
                audits = audits.Where(a=>a.AuditType == (int)type.Value);
            }
            return audits.OrderBy(a=>a.AuditDate).ToList();
        }

        public IEnumerable<Audit> GetAuditMessages(int count, AuditType? type)
        {
            var auditContext = new BillContext();
            var audits = auditContext.Audits.OrderByDescending(a=>a.AuditDate).AsEnumerable();
            if (type.HasValue)
            {
                audits = audits.Where(a => a.AuditType == (int)type.Value);
            }
            return audits.Take(count).OrderBy(a => a.AuditDate).ToList();
        }
    }
}