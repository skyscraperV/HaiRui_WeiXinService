using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.ViewModel
{
    public class Subscriber_ViewModel
    {
        public System.Guid SubscribeID { get; set; }
        public string NickName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Nullable<System.DateTime> SubscribeTime { get; set; }
        public string OpenID { get; set; }
        public string FromOpenID { get; set; }
        public Nullable<System.Guid> AccountID { get; set; }
        public Nullable<int> Sex { get; set; }
        public string HeadImgUrl { get; set; }
        public Nullable<int> Score { get; set; }
        public Nullable<System.DateTime> UnSubscribeTime { get; set; }
        public Nullable<bool> IsOK { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Precision { get; set; }
        public Nullable<System.DateTime> LocationTime { get; set; }
        public string FromNickName { get; set; }
        public string FromHeadImgUrl { get; set; }

        public int FansCount { get; set; }
        public int FansStayCount { get; set; }
        public int FansLeaveCount { get; set; }

        public string FansStayPercent { get; set; }
        public string FansLeavePercent { get; set; }


    }
}
