using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Domain.Enums
{
    public static class PipStatus
    {
        public const string Active = "Active";
        public const string Completed = "Completed";
        public const string Terminated = "Terminated";
        public const string Extended = "Extended";

        public static readonly IReadOnlyList<string> All =
            [Active, Completed, Terminated, Extended];
    }
}
