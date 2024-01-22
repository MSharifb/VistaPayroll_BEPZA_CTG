using Domain.PGM;
using PGM.Web.SecurityService;
using PGM.Web.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using DAL.PGM;
using PGM.Web.Models;

namespace PGM.Web.Controllers
{
    public class AccountController : Controller
    {
        #region Fields

        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Ctor
        public AccountController(PGMCommonService pgmCommonservice)
            : this(null, null)
        {
            this._pgmCommonservice = pgmCommonservice;
        }

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.
        //
        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }
        #endregion

        #region Properties
        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }
        #endregion

        #region Action Results

        #region Notification Action Results
        //public JsonResult GetNotifications()
        //{
        //    int loggedinEmployeeId = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll().FirstOrDefault(e => e.EmpID == MyAppSession.EmpId).Id;

        //    var notificationAlreadyRead = _prmCommonservice.PRMUnit.NotificationReadByRepository
        //        .GetAll()
        //        .Where(n => n.EmployeeId == loggedinEmployeeId).ToList();

        //    var myNotifications = (from n in _prmCommonservice.PRMUnit.NotificationRepository.GetAll()
        //                           join nf in _prmCommonservice.PRMUnit.NotificationFlowRepository.GetAll() on n.Id equals nf.NotificationId
        //                           join e in _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll() on n.IUser equals e.EmpID
        //                           where nf.EmployeeId == loggedinEmployeeId
        //                           && !(from nr in notificationAlreadyRead select nr.NotificationId).Contains(n.Id)
        //                           select new NotificationSearchViewModel
        //                           {
        //                               Id = n.Id,
        //                               Module = ((MyNotificationLibEnum.NotificationModule)nf.Module).ToString().Replace("_", " "),
        //                               NotificationType = ((MyNotificationLibEnum.NotificationType)n.NotificationType).ToString().Replace("_", " "),
        //                               NotificationDate = Common.GetDate(n.NotificationDate),
        //                               NotifyBy = e.FullName + " (" + e.EmpID + ")",
        //                               Message = n.Message,
        //                               RedirectLink = n.RedirectToLink
        //                           }).ToList();


        //    return Json(
        //        new
        //        {
        //            notifications = myNotifications
        //        }, JsonRequestBehavior.AllowGet
        //    );

        //}

        //[HttpPost]
        //[NoCache]
        //public ActionResult MarkMyNotificationAsRead(int id)
        //{
        //    var model = new NotificationReadByViewModel();

        //    try
        //    {
        //        int loggedinEmployeeId = _prmCommonservice.PRMUnit.EmploymentInfoRepository.GetAll().FirstOrDefault(e => e.EmpID == MyAppSession.EmpId).Id;

        //        model.NotificationId = id;
        //        model.EmployeeId = loggedinEmployeeId;
        //        var entity = model.ToEntity();

        //        _prmCommonservice.PRMUnit.NotificationReadByRepository.Add(entity);
        //        _prmCommonservice.PRMUnit.NotificationReadByRepository.SaveChanges();

        //        return Json(new
        //        {
        //            Success = 1,
        //            Message = ErrorMessages.DeleteSuccessful
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new
        //        {
        //            Success = 0,
        //            Message = ErrorMessages.DeleteFailed
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[NoCache]
        //public ActionResult RedirectToUrlFromNotification(String redirectUrl)
        //{
        //    string hostServerName = ConfigurationManager.AppSettings["url"].ToString();

        //    var url = hostServerName + redirectUrl;

        //    return Json(
        //        new
        //        {
        //            url = url
        //        }, JsonRequestBehavior.AllowGet);
        //} 
        #endregion

        #region Two Factor Authentication

        public void SendCodeViaSMS(string mobile, string code)
        {
            String sid = "BEPZA"; String user = "BEPZA"; String pass = "$20>PyQ33";
            String URI = "http://sms.sslwireless.com/pushapi/dynamic/server.php";
            String SMSSenderAndText = string.Empty;

            if (!string.IsNullOrEmpty(mobile) && !mobile.StartsWith("88"))
            {
                int i = 0;
                mobile = "88" + mobile;
                // insert
                SMSSenderAndText += string.Format("&sms[{0}][0]={1}&sms[{0}][1]={2}&sms[{0}][2]=BEPZA{3}", i, mobile, System.Web.HttpUtility.UrlEncode("Authentication Code :" + code), DateTime.Now.Ticks.ToString());
            }

            String myParameters = "user=" + user + "&pass=" + pass + SMSSenderAndText + "&sid=" + sid;
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(URI, myParameters);
            }
        }

        public string GenerateUniqueNumber()
        {
            string numbers = "1234567890";

            string characters = numbers;
            int length = 6;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }

            return otp;
        }

        #endregion

        //[NoCache]
        //[HttpGet]
        //public ActionResult LogOn()
        //{
        //    LogOnModel model = new LogOnModel();
        //    model.ISMultiZone = false;
        //    return View(model);
        //}

        [NoCache]
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            return FreshLogin(model, returnUrl);
        }

        [NoCache]
        public ActionResult LogOn(String uid, String pwd, Int32? ZoneId, String isInjectingLogon = "0")
        {
            LogOnModel model = new LogOnModel();
            model.ISMultiZone = false;

            if (String.IsNullOrEmpty(uid) || String.IsNullOrEmpty(pwd))
            {
                return View(model);
            }

            model.UserName = uid;
            model.Password = pwd;
            model.ZoneInfoId = Common.GetInteger(ZoneId);
            model.RememberMe = true;

            if (!String.IsNullOrEmpty(isInjectingLogon) && isInjectingLogon != "1")
            {
                return FreshLogin(model, String.Empty);
            }

            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

            var user = new User();
            MembershipService.GetUserByUserName(model.UserName, model.Password, out user);// created new method for encripted password.
            MyAppSession.User = user;
            MyAppSession.UserID = user.UserId;
            MyAppSession.Password = model.Password;


            // Get Zone Name
            var zone = new Zone();
            MembershipService.GetZoneNameList(model.ZoneInfoId, out zone);
            MyAppSession.ZoneInfoId = model.ZoneInfoId;
            MyAppSession.ZoneName = zone.ZoneName;

            //MyAppSession.ZoneInfoId = model.ZoneInfoId;
            //var zone = _pgmCommonservice.PGMUnit.FunctionRepository.GetZoneInfoById(model.ZoneInfoId);
            //if (zone != null)
            //{
            //    MyAppSession.ZoneName = zone.ZoneName;
            //}

            var emp = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeByEmpId(user.EmpId);
            if (emp != null)
            {
                MyAppSession.EmpId = emp.EmpID;
                MyAppSession.EmpName = emp.FullName;
                MyAppSession.EmpDesignation = emp.DesignationName;
            }

            #region Validate Is Multi Zone

            var IsMultiZone = MembershipService.ValidateZone(user.EmpId, user.ZoneId);
            MyAppSession.IsMultiZone = IsMultiZone;

            #endregion

            #region getUser ZoneList

            var List = MembershipService.GetZoneList(user.EmpId, user.ZoneId);
            HashSet<int> zoneIDs = new HashSet<int>(List.Select(s => s.ZoneId));
            var ddlFilterList = _pgmCommonservice.PGMUnit.ZoneInfoRepository.GetAll()
                .Where(x => zoneIDs.Contains(x.Id))
                .OrderBy(s => s.SortOrder)
                .ToList();

            List<PRM_ZoneInfo> ZoneList = new List<PRM_ZoneInfo>();
            foreach (var item in ddlFilterList)
            {
                var obj = new PRM_ZoneInfo
                {
                    Id = item.Id,
                    ZoneName = item.ZoneName,
                };
                ZoneList.Add(obj);
            }
            model.ZoneList = ZoneList;
            MyAppSession.SelectedZoneList = ZoneList;

            #endregion

            return RedirectToAction("Index", "Home");
        }


        private ActionResult FreshLogin(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                var Type = Request.Browser.Type;
                var Name = Request.Browser.Browser;
                var Version = Request.Browser.Version;

                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    var user = new User();
                    var zone = new Zone();

                    MembershipService.ValidateUser(model.UserName, model.Password, out user);

                    if (user.Status)
                    {

                        if (MembershipService.IsVerificationEnable())
                        {
                            if (Request.Cookies["Username"] != null)
                            {
                                var userName = Request.Cookies["Username"].Value;
                                var password = Request.Cookies["Password"].Value;
                                if (userName == model.UserName && password == model.Password)
                                {
                                    model.IsVerified = true;
                                }
                            }

                            if (!model.IsVerified)
                            {
                                MyAppSession.AuthenticationCode = GenerateUniqueNumber();
                                SendCodeViaSMS(user.Phone, MyAppSession.AuthenticationCode);
                                return View("VerifyAccount", model);
                            }
                            else if (model.IsVerified && !string.IsNullOrEmpty(model.TwoFactorCode))
                            {
                                if (MyAppSession.AuthenticationCode == model.TwoFactorCode)
                                {

                                }
                                else
                                {
                                    ModelState.AddModelError("", "Incorrect Authentication Code.");
                                    return View("VerifyAccount", model);
                                }
                            }

                            if (model.RememberMeforVerification)
                            {
                                Response.Cookies["Username"].Value = model.UserName;
                                Response.Cookies["Password"].Value = model.Password;
                            }

                        }

                        #region Validate Is Multi Zone

                        var IsMultiZone = MembershipService.ValidateZone(user.EmpId, user.ZoneId);

                        #endregion

                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        MyAppSession.User = user;

                        MyAppSession.UserID = user.UserId;
                        MyAppSession.Password = model.Password;
                        MyAppSession.EmpId = user.EmpId;
                        MyAppSession.EmpName = user.LastName;
                        MyAppSession.EmpDesignation = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeByEmpId(user.EmpId).DesignationName;
                        MyAppSession.IsMultiZone = IsMultiZone;

                        #region getUser ZoneList

                        var List = MembershipService.GetZoneList(user.EmpId, user.ZoneId);
                        HashSet<int> zoneIDs = new HashSet<int>(List.Select(s => s.ZoneId));
                        var ddlFilterList = _pgmCommonservice.PGMUnit.ZoneInfoRepository.GetAll()
                            .Where(x => zoneIDs.Contains(x.Id))
                            .OrderBy(s => s.SortOrder)
                            .ToList();

                        List<PRM_ZoneInfo> ZoneList = new List<PRM_ZoneInfo>();
                        foreach (var item in ddlFilterList)
                        {
                            var obj = new PRM_ZoneInfo
                            {
                                Id = item.Id,
                                ZoneName = item.ZoneName,
                            };
                            ZoneList.Add(obj);
                        }
                        model.ZoneList = ZoneList;
                        MyAppSession.SelectedZoneList = ZoneList;

                        #endregion

                        var EmploueeId = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeByEmpId(user.EmpId).Id;

                        if (!IsMultiZone)
                        {
                            #region Get Zone Name
                            MembershipService.GetZoneNameList(user.ZoneId, out zone);
                            #endregion

                            MyAppSession.ZoneInfoId = user.ZoneId;
                            MyAppSession.ZoneName = zone.ZoneName;

                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            model.ISMultiZone = true;
                            return View("ZoneLogOn", model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user is inactive.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View("LogOn", model);
        }

        //
        // POST: /Account/ZoneLogOn
        public bool ZoneLogOn(int ZoneId)
        {
            #region Get Zone Name
            var zone = new Zone();
            MembershipService.GetZoneNameList(ZoneId, out zone);
            #endregion

            MyAppSession.ZoneInfoId = ZoneId;
            MyAppSession.ZoneName = zone.ZoneName;

            return true;
        }

        //
        // GET: /Account/SwitchEPZ
        public ActionResult SwitchEPZ(LogOnModel model)
        {
            try
            {
                model.ISMultiZone = true;
                model.ZoneInfoId = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeByEmpId(MyAppSession.EmpId).ZoneInfoId;
                var List = MembershipService.GetZoneList(MyAppSession.EmpId, model.ZoneInfoId);
                HashSet<int> zoneIDs = new HashSet<int>(List.Select(s => s.ZoneId));
                var ddlFilterList = _pgmCommonservice.PGMUnit.ZoneInfoRepository.GetAll()
                    .Where(x => zoneIDs.Contains(x.Id))
                    .OrderBy(s => s.SortOrder)
                    .ToList();

                List<PRM_ZoneInfo> ZoneList = new List<PRM_ZoneInfo>();
                foreach (var item in ddlFilterList)
                {
                    var obj = new PRM_ZoneInfo
                    {
                        Id = item.Id,
                        ZoneName = item.ZoneName,
                    };
                    ZoneList.Add(obj);
                }
                model.ZoneList = ZoneList;
            }
            catch
            {
                return LogOff();
            }

            return View("ZoneLogOn", model);
        }

        //
        // GET: /Account/LogOff
        [NoCache]
        public ActionResult LogOnDashboard(string uid, string pwd)
        {
            LogOnModel model = new LogOnModel();
            model.UserName = uid;
            model.Password = pwd;

            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                }

                var user = new User();
                MembershipService.ValidateUser(model.UserName, model.Password, out user);
                MyAppSession.User = user;
                MyAppSession.UserID = user.UserId;
                MyAppSession.Password = model.Password;

                #region Get Zone Name
                var zone = new Zone();
                MembershipService.GetZoneNameList(user.ZoneId, out zone);
                #endregion

                MyAppSession.ZoneInfoId = user.ZoneId;
                MyAppSession.ZoneName = zone.ZoneName;

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            string url = "";
            string hostServerName = ConfigurationManager.AppSettings["HostServerName"].ToString();
            string mainProjectName = ConfigurationManager.AppSettings["MainProjectName"].ToString();

            Session.Abandon();
            Session.Clear();
            HttpContext.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
            FormsAuth.SignOut();

            url = "http://" + hostServerName + "/" + mainProjectName + "/Account/LogOff";
            return Redirect(url);
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            MyAppSession.AuthenticationCode = GenerateUniqueNumber();
            SendCodeViaSMS(MyAppSession.User.Phone, MyAppSession.AuthenticationCode);

            return View();
        }

        //
        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                if (MyAppSession.AuthenticationCode == model.AuthenticationCode)
                {
                    try
                    {
                        //
                        changePasswordSucceeded = MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect Authentication Code.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserManual()
        {
            return View();
        }

        public ActionResult GetUserManual(String fileName)
        {
            String filePath = Server.MapPath("~/Content/UserManual/" + fileName);
            if (System.IO.File.Exists(filePath))
            {
                Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);
                FileStream fsSource = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fsSource, "application/pdf");
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        // The FormsAuthentication type is sealed and contains static members, so it is difficult to
        // unit test code that calls its members. The interface and helper class below demonstrate
        // how to create an abstract wrapper around such a type in order to make the AccountController
        // code unit testable.
        public interface IFormsAuthentication
        {
            void SignIn(string userName, bool createPersistentCookie);
            void SignOut();
        }

        public class FormsAuthenticationService : IFormsAuthentication
        {
            public void SignIn(string userName, bool createPersistentCookie)
            {
                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            }
            public void SignOut()
            {
                FormsAuthentication.SignOut();
            }
        }

        public interface IMembershipService
        {
            int MinPasswordLength { get; }

            bool ValidateUser(string userName, string password);
            bool ValidateUser(string userName, string password, out User user);
            bool ValidateZone(string empID, int zoneId);
            List<Zone> GetZoneList(string empID, int zoneId);
            MembershipCreateStatus CreateUser(string userName, string password, string email);
            bool ChangePassword(string userName, string oldPassword, string newPassword);
            bool GetZoneNameList(int zoneId, out Zone zone);
            bool IsVerificationEnable();
            bool GetUserByUserName(string userName, string password, out User user);
        }

        public class AccountMembershipService : IMembershipService
        {
            private CustomMembershipProvider _provider;

            public AccountMembershipService()
                : this(null)
            {
            }

            public AccountMembershipService(CustomMembershipProvider provider)
            {
                _provider = provider ?? new CustomMembershipProvider();
            }

            public int MinPasswordLength
            {
                get
                {
                    return _provider.MinRequiredPasswordLength;
                }
            }

            public bool ValidateUser(string userName, string password)
            {
                return _provider.ValidateUser(userName, password);
            }

            public MembershipCreateStatus CreateUser(string userName, string password, string email)
            {
                MembershipCreateStatus status;
                _provider.CreateUser(userName, password, email, null, null, true, null, out status);
                return status;
            }

            public bool ChangePassword(string userName, string oldPassword, string newPassword)
            {
                return new CustomMembershipProvider().ChangePassword(userName, oldPassword, newPassword);
            }

            public bool ValidateUser(string userName, string password, out User user)
            {
                return _provider.ValidateUser(userName, password, out user);
            }

            public bool ValidateZone(string empID, int zoneId)
            {
                return _provider.ValidateZone(empID, zoneId);
            }

            public List<Zone> GetZoneList(string empID, int zoneId)
            {
                return _provider.GetZoneList(empID, zoneId);
            }

            public bool GetZoneNameList(int zoneId, out Zone zone)
            {
                return _provider.GetZoneNameList(zoneId, out zone);
            }

            public bool IsVerificationEnable()
            {
                return _provider.IsVerificationEnable();
            }
            public bool GetUserByUserName(string userName, string password, out User user)
            {
                return _provider.GetUserByUserName(userName, password, out user);
            }
        }
    }
}
