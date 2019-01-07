using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceDeskApplication.Models
{
    public class AttachedFile
    {
        public Guid Id { get; set; }
        public string TroubleTaskId { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}