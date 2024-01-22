using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.SalaryHeadGroup;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using DAL.PGM;
using PGM.Web.Resources;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class SalaryHeadGroupController : Controller
    {

        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor
        public SalaryHeadGroupController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }
        #endregion

        public ViewResult Index()
        {
            var model = new SalaryHeadGroupViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SalaryHeadGroupSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.GetCount(filterExpression);

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.GetPaged(filterExpression.ToString(), request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1);

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.Name,
                    d.HeadType,
                    d.SortOrder,
                    d.Remarks,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            SalaryHeadGroupViewModel model = new SalaryHeadGroupViewModel();
            model.HeadTypeList = Common.PopulateSalaryHeadTypeDDL();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SalaryHeadGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = model.ToEntity();
                obj.IUser = User.Identity.Name;
                obj.IDate = Common.CurrentDateTime;

                if (CheckDuplicateEntry(model,0))
                {                  
                    model.ErrMsg = ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                    return View(model);
                }

                _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Add(obj);
                _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.SaveChanges();
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            return View(model);
        }



        public ActionResult Edit(int id)
        {
            var obj = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.GetByID(id);
            SalaryHeadGroupViewModel model = obj.ToModel();
            model.HeadTypeList = Common.PopulateSalaryHeadTypeDDL();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SalaryHeadGroupViewModel model)
        {

            if (ModelState.IsValid)
            {
                var obj = model.ToEntity();
                obj.EUser = User.Identity.Name;
                obj.EDate = Common.CurrentDateTime;

                if (CheckDuplicateEntry(model,model.Id))
                {
                    model.ErrMsg = ErrorMessages.UniqueIndex;
                    model.errClass = "failed";
                    return View(model);
                }

                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Update(obj, NavigationList);
                _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.SaveChanges();

                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                
            }

            model.HeadTypeList = Common.PopulateSalaryHeadTypeDDL();
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                List<Type> allTypes = new List<Type> { typeof(PRM_SalaryHeadGroup) };

                _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Delete(id);
                _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.SaveChanges();
                result = true;
            }
            catch (UpdateException ex)
            {
                try
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                    ModelState.AddModelError("Error", errMsg);
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            catch 
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }

        [NoCache]
        public ActionResult GetHeadTypeView()
        {
            Dictionary<string, string> headType = new Dictionary<string, string>();

            var headTypes = Common.GetEnumAsDictionary<PGMEnum.SalaryHeadType>();
            foreach (KeyValuePair<int, string> item in headTypes)
            {
                headType.Add(item.Value, item.Value);
            }

            return PartialView("_Select", headType);
        }

        private bool CheckDuplicateEntry(SalaryHeadGroupViewModel model,int strMode)
        {
            if (strMode <1)
            {
                return _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Get(q => q.Name == model.Name).Any();
            }
            else
            {
                return _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Get(q => q.Name == model.Name && strMode != q.Id).Any();
            }
        }

    }
}