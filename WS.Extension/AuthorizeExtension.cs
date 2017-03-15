using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WS.BLL;
using WS.Model;

namespace WS.Extension
{
    public class AuthorizeExtension : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated) //判断用户是否通过验证
            {
                return false;
            }
            if (string.IsNullOrEmpty(Roles))
            {
                return true;
            }

            bool isAccess = JudgeAuthorize(httpContext.User.Identity.Name, Roles);
            if (isAccess)
            {
                return true;

            }
            else
            {
                return base.AuthorizeCore(httpContext);


            }

        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string path = filterContext.HttpContext.Request.Path;
                string strUrl = "/Users/Login?ReturnUrl={0}";
                filterContext.HttpContext.Response.Redirect(string.Format(strUrl, HttpUtility.UrlEncode(path)), true);
            }
            else
            {
                filterContext.Result = new RedirectResult("/Users/NoPermission");
            }
        }

        public bool JudgeAuthorize(string userName, string permissionRoles)
        {
            string userRoleName = GetUserRole(userName).RoleName;

            if (permissionRoles.Contains(userRoleName))
            {
                return true;

            }
            else
            {
                return false;
            }

        }

        public Users_Roles GetUserRole(string userName)
        {
            Users_BLL ubll = new Users_BLL();
            Users_Roles user_role = ubll.Get(a => a.UserName == userName).Users_Roles;

            return user_role;
        }
    }
}
