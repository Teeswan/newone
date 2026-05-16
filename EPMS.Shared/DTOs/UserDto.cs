using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int EmployeeId { get; set; }

        // ဝန်ထမ်းရဲ့ နာမည်အစစ်ကိုပါ တစ်ပါတည်း ထည့်ပြပေးရန်
        public string EmployeeName { get; set; }

        // User တစ်ယောက်မှာ Role တွေ အများကြီးရှိနိုင်လို့ List နဲ့ လက်ခံပါမယ်
        public List<string> Roles { get; set; } = new List<string>();
    }
}
