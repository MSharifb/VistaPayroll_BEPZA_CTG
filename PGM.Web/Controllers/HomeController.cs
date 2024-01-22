using System;
using PGM.Web.SecurityService;
using PGM.Web.Utility;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;


namespace PGM.Web.Controllers
{
    public class HomeController : Controller
    {
        private UserManagementServiceClient _userAgent;

        public ActionResult Index()
        {
            //ViewBag.Message = "Dashboard";
            //PGM.Web.Models.Menu model = new PGM.Web.Models.Menu();
            //model.ModuleName = GetModuleAuthentication();

            //return View(model);

            return RedirectToAction("Index", "Home", new { Area = "PGM" });
        }

        //The following DashboardHome method will redirect to the HRmanager Dashboard.
        [NoCache]
        public ActionResult DashboardHome()
        {
            string userName = "";
            string userPass = "";

            userName = User.Identity.Name;
            userPass = MyAppSession.Password;
            
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(userPass))
            {
                return RedirectToAction("Index", "Home");
            }

            string hostServerName = ConfigurationManager.AppSettings["HostServerName"].ToString();
            string mainProjectName = ConfigurationManager.AppSettings["MainProjectName"].ToString();

            String url = "http://" + hostServerName + "/" + mainProjectName + "/Account/LogOnDashboard?uid=" + userName + "&pwd=" + Uri.EscapeDataString(userPass);
            //var url = ConfigurationManager.AppSettings["ExtarnalDashboardUrl"];
            return Redirect(url);
        }

        public ActionResult About()
        {
            return View();
        }

        public string GetModuleAuthentication()
        {
            int UID = 0;
            string moduleName = "";
            _userAgent = new UserManagementServiceClient();
            if (MyAppSession.UserID > 0)
            {
                UID = MyAppSession.UserID;
            }
            var role = _userAgent.GetRolesList();
            var sss1 = _userAgent.GetUserList();
            UserMenu userMenu = new UserMenu();
            userMenu.UserId = UID;
            var userMenuList = _userAgent.GetAllUserMenuList(userMenu);

            var userRole = _userAgent.GetUserRole(UID);

            var userRoleModule = (from ur in userRole
                                  join rol in role on ur.RoleId equals rol.RoleId
                                  select rol.ModuleId).Concat(from um in userMenuList select um.ModuleId);

            var selectedModeles = new List<PGM.Web.SecurityService.Module>();

            foreach (var item in userRoleModule)
            {
                var module = _userAgent.GetModuleById(item);

                var mm = new Module
                {
                    ModuleName = module.ModuleName,
                    ModuleTitle = module.ModuleTitle,
                    SortOrder = module.SortOrder
                };
                selectedModeles.Add(mm);
            }

            MyAppSession.ModuleName = selectedModeles.DistinctBy(x => x.ModuleName).OrderBy(s => s.SortOrder).ToList();

            return moduleName;
        }
    }
}
