using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProductify.Core.Common
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime UploadDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
