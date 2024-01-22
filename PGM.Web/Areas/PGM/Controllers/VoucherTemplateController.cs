using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.VoucherTemplate;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class VoucherTemplateController : BaseController
    {
        // GET: PGM/VoucherTemplate
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private readonly PGMEntities _pgmContext;

        #endregion

        #region Ctor

        public VoucherTemplateController(PGMCommonService pgmCommonservice, PGMEntities pgmContext)
        {
            this._pgmCommonService = pgmCommonservice;
            this._pgmContext = pgmContext;
        }

        #endregion

        public ActionResult Index(string year, string month)
        {
            var model = new VoucherTemplateMasterViewModel();
            model.Year = year;
            model.Month = month;
            PopulateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Index(VoucherTemplateMasterViewModel model)
        {
            try
            {
                PopulateDropdown(model);
                //var result = _pgmContext.PGM_SalaryAutoPVJVVoucher(LoggedUserZoneInfoId, model.Month, model.Year, MyAppSession.UserID, model.ApproverId, MyAppSession.EmpId);
                model.IsError = 0;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            catch (Exception ex)
            {
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }
            return View("Index", model);
        }

        [NoCache]
        private void PopulateDropdown(VoucherTemplateMasterViewModel model)
        {
            int employeeId = _pgmCommonService.PGMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(x => x.Id).FirstOrDefault();
            var approverList = _pgmContext.ACC_getApproverListByZoneId(LoggedUserZoneInfoId, "AccVou", employeeId, null).ToList();
            var list = new List<SelectListItem>();
            foreach (var item in approverList)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id.ToString()
                });
            }
            model.ApproverList = list;
        }
    }
}