using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.ViewModel
{
    public class LayoutHeader_ViewModel
    {
        public System.Guid SubscribeID { get; set; }

        public System.DateTime SubscribeTime { get; set; }
        public int Score { get; set; }
        public string HeadImgUrl { get; set; }
        public string NickName { get; set; }
        public int ChildSubscribeCount { get; set; }


        public Nullable<bool> IsYouZan { get; set; }
    }
}
