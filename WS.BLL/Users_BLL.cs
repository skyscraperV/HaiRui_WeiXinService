using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WS.Model;
using WS.Utility;

namespace WS.BLL
{
    public partial class Users_BLL
    {
        public bool CheckExist(string username)
        {

            return dal.GetCount(a => a.UserName == username) > 0 ? true : false;
        }
        public bool CheckExist(string username, string password)
        {
            password = SecurityHelper.MD5(password);
            return dal.GetCount(a => a.UserName == username && a.PassWord == password) > 0 ? true : false;
        }

        public Users GetCurrentUser()
        {

            Users user = dal.Get(a => a.UserName == HttpContext.Current.User.Identity.Name);
            return user;
        }
    }
}
