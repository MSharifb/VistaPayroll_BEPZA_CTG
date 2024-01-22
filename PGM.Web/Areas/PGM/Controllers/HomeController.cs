
using PGM.Web.SecurityService;
using PGM.Web.Utility;
using System;
using System.Configuration;
using System.Web.Mvc;
using Domain.PGM;


namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class HomeController : Controller
    {
        private UserManagementServiceClient _userAgent;
        private PGMCommonService _pgmCommonService;

        #region Ctor
        public HomeController(PGMCommonService pgmCommonService)
        {
            this._userAgent = new UserManagementServiceClient();
            this._pgmCommonService = pgmCommonService;
        }
        #endregion
        
        [NoCache]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SwitchZone(int id)
        {
            MyAppSession.ZoneInfoId = id;
            MyAppSession.ZoneName = _pgmCommonService.PGMUnit.ZoneInfoRepository.GetByID(id).ZoneName;
            return RedirectToAction("Index");
        }

        public ActionResult Unauthorized()
        {
            return View();
        }


    } // End of class Home
}
