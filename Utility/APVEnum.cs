using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class APVEnum
    {
        public enum ApprovalStatusType
        {
            Drafted = 1,
            Cancelled = 2,
            Submitted = 3,
            Rejected = 4,
            Recommended = 5,
            Approved = 6
        }
    }
}
