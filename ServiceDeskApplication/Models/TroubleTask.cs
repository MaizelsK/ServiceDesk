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
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual ApplicationUser User { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string Text { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; } = Enum.GetName(typeof(TroubleTaskStatus), 0);
        public virtual ApplicationUser Assigned { get; set; }

        public bool IsFileAttached { get; set; }
        public string AttachedFileName { get; set; }
        public AttachedFile AttachedFile { get; set; }
    }
}