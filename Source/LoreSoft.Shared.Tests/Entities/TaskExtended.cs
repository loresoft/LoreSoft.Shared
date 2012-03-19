using System;
using System.Collections.Generic;
using System.Text;

namespace LoreSoft.Shared.Tests.Entities
{
    public partial class TaskExtended
    {
        public TaskExtended()
        {
        }

        public int TaskId { get; set; }
        public string Browser { get; set; }
        public string Os { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Byte[] RowVersion { get; set; }

        public virtual Task Task { get; set; }
    }
}