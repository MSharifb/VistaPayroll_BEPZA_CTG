using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using DAL.PGM;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.SalaryHead;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Resources;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class SalaryHeadController : BaseController
    {

        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor

        public SalaryHeadController(PGMCommonService pgmCommonService)
        {
            this._pgmCommonservice = pgmCommonService;
        }

        #endregion

        [NoCache]
        public ViewResult Index()
        {
            var model = new SalaryHeadViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, SalaryHeadSearchViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            string isTaxable = form["IsTaxable"];
            string isGrossPay = form["IsGrossPayHead"];
            string isOtherAddition = form["IsOtherAddition"];
            string isOtherDeduction = form["IsOtherDeduction"];
            string isActiveHead = form["IsActiveHead"];

            if (request.Searching && viewModel != null)
            {
                filterExpression = viewModel.GetFilterExpression(isTaxable, isGrossPay, isOtherAddition, isOtherDeduction, isActiveHead);
            }

            totalRecords = _pgmCommonservice.PGMUnit.SalaryHeadRepository.GetCount(filterExpression);

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var list = _pgmCommonservice.PGMUnit
                .SalaryHeadRepository
                .GetPaged(filterExpression.ToString()
                        , request.SortingName
                        , request.SortingOrder.ToString()
                        , request.PageIndex
                        , request.RecordsCount
                        , request.PagesCount.HasValue ? request.PagesCount.Value : 1);

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.HeadName,
                    d.HeadType,
                    _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.GetByID(d.GroupId).Name,
                    d.AmountType,
                    d.SortOrder,
                    d.IsTaxable,
                    d.IsGrossPayHead,
                    d.IsOtherAddition,
                    d.IsOtherDeduction,
                    d.IsActiveHead,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            ViewBag.HeadGroup = GetHeadGroup("");
            var model = new SalaryHeadViewModel();
            model.DefaultAmount = 0M;
            model.IsActiveHead = true;
            PopulateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(SalaryHeadViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.errClass = "failed";

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                SetSalaryMappings(model);
                var entity = model.ToEntity();
                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;

                try
                {
                    _pgmCommonservice.PGMUnit.SalaryHeadRepository.Add(entity);
                    _pgmCommonservice.PGMUnit.SalaryHeadRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return View("Index", model);
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }

            ViewBag.HeadGroup = GetHeadGroup(model.HeadType);
            return View("Index", model);
        }

        [NoCache]
        private string GetBusinessLogicValidation(SalaryHeadViewModel model)
        {
            List<String> errorMessage = new List<string>();

            var salaryHeads = _pgmCommonservice
                .PGMUnit
                .SalaryHeadRepository
                .GetAll().ToList();

            if (salaryHeads != null && salaryHeads.Any())
            {
                // Check duplicate basic head
                if (model.IsBasicHead)
                {
                    var duplicateBasicHead = model.Id == 0
                        ? salaryHeads.Any(x => x.IsBasicHead)
                        : salaryHeads.Any(x => x.Id != model.Id && x.IsBasicHead);
                    if (duplicateBasicHead)
                    {
                        errorMessage.Add("Basic salary head already exists!");
                    }
                }

                // Check duplicate salary head name
                bool duplicateHeadNameExist = false;
                bool duplicateHeadMapping = false;

                duplicateHeadNameExist = model.Id == 0 ? salaryHeads.Any(x => x.HeadName == model.HeadName) : salaryHeads.Any(x => x.Id != model.Id && x.HeadName == model.HeadName);
                if (duplicateHeadNameExist == true)
                {
                    errorMessage.Add("Duplicate salary head name found!");
                }

                // Check duplicate salary mapping 
                if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.House_Rent))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsHouseRentHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsHouseRentHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Medical))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsMedicalHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsMedicalHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Conveyance))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsConveyanceHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsConveyanceHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.PF_Own_Contribution))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsPFOwnContributionHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsPFOwnContributionHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.PF_Company_Contribution))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsPFCompanyContributionHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsPFCompanyContributionHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Leave_Without_Pay))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsLeaveWithoutPayHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsLeaveWithoutPayHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Incometax_Addition))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsIncomeTaxAdditionHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsIncomeTaxAdditionHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Incometax_Deduction))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsIncomeTaxDeductionHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsIncomeTaxDeductionHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Pension_Provision))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsPensionHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsPensionHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.GPF))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsGPFHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsGPFHead == true);
                else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Arrear))
                    duplicateHeadMapping = model.Id == 0 ? salaryHeads.Any(x => x.IsArrearHead == true) : salaryHeads.Any(x => x.Id != model.Id && x.IsArrearHead == true);

                if (duplicateHeadMapping == true)
                {
                    errorMessage.Add("Duplicate salary mapping found!");
                }
            }

            // -- Checking sort order: Addition: 1-99; Deduction: 101-199; Provision: 201-299 --
            if (model.HeadType == PGMEnum.SalaryHeadType.Addition.ToString())
            {
                if (model.SortOrder <= 0 || model.SortOrder > 99)
                {
                    errorMessage.Add("Sort Order must be 1-99.");
                }
            }
            else if (model.HeadType == PGMEnum.SalaryHeadType.Deduction.ToString())
            {
                if (model.SortOrder <= 100 || model.SortOrder > 199)
                {
                    errorMessage.Add("Sort Order must be 101-199.");
                }
            }
            else if (model.HeadType == PGMEnum.SalaryHeadType.Provision.ToString())
            {
                if (model.SortOrder <= 200 || model.SortOrder > 299)
                {
                    errorMessage.Add("Sort Order must be 201-299.");
                }
            }
            else
            {
                errorMessage.Add("Please select Head Type!");
            }
            // ---------------


            return Common.ErrorListToString(errorMessage);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {

            var entity = _pgmCommonservice.PGMUnit.SalaryHeadRepository.GetByID(id);
            ViewBag.SelectedId = entity.GroupId;
            ViewBag.HeadGroup = GetHeadGroup(entity.HeadType);

            SalaryHeadViewModel model = entity.ToModel();

            if (entity.IsConveyanceHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Conveyance);
            if (entity.IsHouseRentHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.House_Rent);
            if (entity.IsIncomeTaxAdditionHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Incometax_Addition);
            if (entity.IsIncomeTaxDeductionHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Incometax_Deduction);
            if (entity.IsLeaveWithoutPayHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Leave_Without_Pay);
            if (entity.IsMedicalHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Medical);
            if (entity.IsPFOwnContributionHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.PF_Own_Contribution);
            if (entity.IsGPFHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.GPF);
            if (entity.IsPFCompanyContributionHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.PF_Company_Contribution);
            if (entity.IsPensionHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Pension_Provision);
            if (entity.IsArrearHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Arrear);
            if (entity.IsGratuityHead == true)
                model.HeadMappingId = Convert.ToInt32(PGMEnum.SalaryHeadMapper.Gratuity);


            PopulateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(SalaryHeadViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.errClass = "failed";

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                SetSalaryMappings(model);

                var obj = model.ToEntity();
                obj.EUser = User.Identity.Name;
                obj.EDate = Common.CurrentDateTime;

                if (CheckDuplicateEntry(model, model.Id))
                {
                    IList<PRM_SalaryHeadGroup> headGroupList = GetHeadGroup(model.HeadType);
                    ViewBag.HeadGroup = headGroupList;

                    model.ErrMsg = ErrorMessages.UniqueIndex;
                    return View("Index", model);
                }

                Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                _pgmCommonservice.PGMUnit.SalaryHeadRepository.Update(obj, NavigationList);
                _pgmCommonservice.PGMUnit.SalaryHeadRepository.SaveChanges();

                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                return RedirectToAction("Index");
            }
            else
            {
                model.ErrMsg = errorList;
            }

            PopulateDropdown(model);
            ViewBag.HeadGroup = GetHeadGroup(model.HeadType);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                _pgmCommonservice.PGMUnit.SalaryHeadRepository.Delete(id);
                _pgmCommonservice.PGMUnit.SalaryHeadRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                ModelState.AddModelError("Error", errMsg);
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

        [NoCache]
        public ActionResult GetAmountTypeView()
        {
            Dictionary<string, string> amountType = new Dictionary<string, string>();
            var amountTypes = Common.GetEnumAsDictionary<PGMEnum.AmountType>();

            foreach (KeyValuePair<int, string> item in amountTypes)
            {
                amountType.Add(item.Value, item.Value);
            }

            return PartialView("_Select", amountType);
        }

        [NoCache]
        public ActionResult GetTaxableView()
        {
            return PartialView("Select", Common.PopulateYesNoDDLList());
        }

        [NoCache]
        public ActionResult GetIsGrossPayView()
        {
            return PartialView("Select", Common.PopulateYesNoDDLList());
        }

        [NoCache]
        public ActionResult GetIsOtherAdditionView()
        {
            return PartialView("Select", Common.PopulateYesNoDDLList());
        }

        [NoCache]
        public ActionResult GetIsOtherDeductionView()
        {
            return PartialView("Select", Common.PopulateYesNoDDLList());
        }

        [NoCache]
        public ActionResult GetIsActiveView()
        {
            return PartialView("Select", Common.PopulateYesNoDDLList());
        }

        #region  private method

        private void SetSalaryMappings(SalaryHeadViewModel model)
        {

            if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Conveyance))
            {
                model.IsConveyanceHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.House_Rent))
            {
                model.IsHouseRentHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Incometax_Addition))
            {
                model.IsIncomeTaxAdditionHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Incometax_Deduction))
            {
                model.IsIncomeTaxDeductionHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Leave_Without_Pay))
            {
                model.IsLeaveWithoutPayHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Medical))
            {
                model.IsMedicalHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.GPF))
            {
                model.IsGPFHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.PF_Company_Contribution))
            {
                model.IsPfCompanyContributionHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.PF_Own_Contribution))
            {
                model.IsPfOwnContributionHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Pension_Provision))
            {
                model.IsPensionHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Arrear))
            {
                model.IsArrearHead = true;
            }
            else if (model.HeadMappingId == Convert.ToInt32(PGMEnum.SalaryHeadMapper.Gratuity))
            {
                model.IsGratuityHead = true;
            }
        }

        private IList<PRM_SalaryHeadGroup> GetHeadGroup(string headType)
        {
            IList<PRM_SalaryHeadGroup> headGroupList = null;
            if (headType.Length < 1)
            {
                headGroupList = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.GetAll().ToList();
            }
            else
            {
                headGroupList = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.GetAll().Where(s => s.HeadType == headType).ToList();
            }
            return headGroupList;
        }

        private IList<PRM_SalaryHeadGroup> GetHeadGroupList(string HeadType)
        {
            IList<PRM_SalaryHeadGroup> itemList;
            if (HeadType != null)
            {
                itemList = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Get(q => q.HeadType == HeadType).ToList();
            }
            else
            {
                itemList = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Get().ToList();
            }
            return itemList;
        }

        [NoCache]
        public ActionResult LoadHeadGroup(string Id)
        {
            IList<PRM_SalaryHeadGroup> lst = _pgmCommonservice.PGMUnit.SalaryHeadGroupRepository.Get(q => q.HeadType == Id).ToList();
            return Json(
                lst.Select(x => new { Id = x.Id, Name = x.Name }),
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public ActionResult LoadSortOrder(string headType)
        {
            var initialVal = 0;
            var salaryHeadAd = (from s in _pgmCommonservice.PGMUnit.SalaryHeadRepository.GetAll().Where(q => q.HeadType == headType) select s.SortOrder).Max();

            if (salaryHeadAd == 0)
            {
                if (headType == PGMEnum.SalaryHeadType.Addition.ToString())
                    initialVal = 1;
                else if (headType == PGMEnum.SalaryHeadType.Deduction.ToString())
                    initialVal = 101;
                else
                    initialVal = 201;
            }
            else
            {
                initialVal = salaryHeadAd + 1;
            }

            return Json(initialVal, JsonRequestBehavior.AllowGet);
        }

        private void PopulateDropdown(SalaryHeadViewModel model)
        {
            model.HeadGroupList = Common.PopulateSalaryHeadGroupDDL(GetHeadGroupList(model.HeadType));
            model.HeadTypeList = Common.PopulateSalaryHeadTypeDDL();
            model.HeadMappingList = Common.PopulateSalaryHeadMappersDDL();
            model.AccEntityList = _pgmCommonservice.PGMUnit.AccFundControlInfoRepository.GetAll().ToList()
               .Select(y =>
               new SelectListItem()
               {
                   Text = y.fundControlName,
                   Value = y.id.ToString()
               }).OrderBy(m => m.Text).ToList();
            model.AccountHeadList = _pgmCommonservice.PGMUnit.AccChartOfAccountRepository.GetAll()
                                    .Join(_pgmCommonservice.PGMUnit.AccFundControlInfoRepository.GetAll(), c => c.fundControlId, f => f.id, (c, f) => new { c, f })
                                    .Where(m => m.c.isControlhead == 0).ToList()
               .Select(y =>
               new SelectListItem()
               {
                   Text = y.c.accountName + " (" + y.f.fundControlName + ")",
                   Value = y.c.id.ToString()
               }).OrderBy(m => m.Text).ToList();

        }

        private bool CheckDuplicateEntry(SalaryHeadViewModel model, int strMode)
        {
            if (strMode < 1)
            {
                return _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(q => q.HeadName == model.HeadName).Any();
            }
            else
            {
                return _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(q => q.HeadName == model.HeadName && strMode != q.Id).Any();
            }
        }

        #endregion

    }
}