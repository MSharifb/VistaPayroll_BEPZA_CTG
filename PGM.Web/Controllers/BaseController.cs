using PGM.Web.Utility;
using System.Web.Mvc;
using System.Web.Routing;

namespace PGM.Web.Controllers
{
    public class BaseController : Controller
    {
        //private readonly PRMCommonSevice _prmCommonService;

        public BaseController()
        {
            LoggedUserZoneInfoId = -1;
        }

        //public BaseController(PRMCommonSevice prmCommonService)
        //{
        //    LoggedUserZoneInfoId = -1;
        //    _prmCommonService = prmCommonService;
        //}

        public int LoggedUserZoneInfoId { get; set; }
        
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            //  ViewBag.UserMenu = this.UserIntranet.Login;
            if (Request.IsAuthenticated)
            {
                if (MyAppSession.ZoneInfoId > 0)
                {
                    LoggedUserZoneInfoId = MyAppSession.ZoneInfoId;
                }
                else
                {
                    Response.Redirect("~/Account/LogOn");
                    Session.Clear();
                }
            }
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //[NoCache]
        //public ActionResult GetMyNotificationList(JqGridRequest request, NotificationSearchViewModel viewModel)
        //{
        //    string filterExpression = String.Empty, LoginEmpId = "";
        //    int totalRecords = 0;

        //    int loggedinEmployeeId = _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll().FirstOrDefault(e => e.EmpID == MyAppSession.EmpId).Id;

        //    var list = (from n in _prmCommonService.PRMUnit.NotificationRepository.GetAll()
        //        join nf in _prmCommonService.PRMUnit.NotificationFlowRepository.GetAll() on n.Id equals nf.NotificationId
        //        join e in _prmCommonService.PRMUnit.EmploymentInfoRepository.GetAll() on n.IUser equals e.EmpID
        //        where nf.EmployeeId == loggedinEmployeeId
        //        select new NotificationSearchViewModel
        //        {
        //            Id = n.Id,
        //            Module = ((MyNotificationLibEnum.NotificationModule)nf.Module).ToString().Replace("_", " "),
        //            NotificationType = ((MyNotificationLibEnum.NotificationType)n.NotificationType).ToString().Replace("_", " "),
        //            NotificationDate = n.NotificationDate,
        //            NotifyBy = e.FullName + " (" + e.EmpID + ")",
        //            Message = n.Message
        //        }).ToList();


        //    if (request.Searching)
        //    {
        //        if (!string.IsNullOrEmpty(viewModel.ModuleName))
        //        {
        //            if (!viewModel.ModuleName.Equals("0"))
        //            {
        //                list = list.Where(q => q.Module.ToString() == viewModel.ModuleName).ToList();
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(viewModel.NotificationTypeName))
        //        {
        //            if (!viewModel.NotificationTypeName.Equals("0"))
        //            {
        //                list = list.Where(q => q.NotificationType.ToString() == viewModel.NotificationTypeName).ToList();
        //            }
        //        }
        //    }


        //    totalRecords = list == null ? 0 : list.Count;

        //    #region Sorting

        //    if (request.SortingName == "NotificationType")
        //    {
        //        if (request.SortingOrder.ToString().ToLower() == "asc")
        //        {
        //            list = list.OrderBy(x => x.NotificationType).ToList();
        //        }
        //        else
        //        {
        //            list = list.OrderByDescending(x => x.NotificationType).ToList();
        //        }
        //    }

        //    #endregion


        //    JqGridResponse response = new JqGridResponse()
        //    {
        //        TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
        //        PageIndex = request.PageIndex,
        //        TotalRecordsCount = totalRecords
        //    };

        //    foreach (var item in list)
        //    {
        //        response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
        //        {

        //            item.Id,
        //            item.Module,
        //            item.NotificationType,
        //            item.NotificationDate,
        //            item.Message,
        //            "Read",
        //            "Link"
        //        }));
        //    }
        //    return new JqGridJsonResult() { Data = response };
        //}





    }
}