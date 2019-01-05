using ServiceDeskApplication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ServiceDeskApplication.Models
{
    public class TroubleTask
    {
        public virtual Guid Id { get; set; } = Guid.NewGuid();
        public virtual ApplicationUser User { get; set; }
        public virtual DateTime GeneratedDate { get; set; }
        public virtual string Text { get; set; }
        public virtual string Comment { get; set; }
        public virtual string Status { get; set; } = Enum.GetName(typeof(TroubleTaskStatus), 0);
        public virtual ApplicationUser Assigned { get; set; }
    }
}