using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.SalaryStructure;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using PGM.Web.Controllers;

/*
Revision History (RH):
		SL		: 01
		Author	: AMN
		Date	: 2016-01-25
        SCR     : ERP_BEPZA_PGM_SCR.doc (SCR#55)
		Change	: User can input lower salary step but same grade initially. – Less Increment
		---------
*/

namespace PGM.Web.Areas.PGM.Controllers
{
    public class SalaryStructureController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor
        public SalaryStructureController(PGMCommonService pgmCommonService)
        {
            _pgmCommonService = pgmCommonService;
        }
        #endregion

        #region Action

        public ViewResult Index()
        {
            var model = new SalaryStructureModel();
            model.Id = 0;
            PopulateSalaryStructureModelLists(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SalaryStructureSearchViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            if (request.Searching)
            {
                if (viewModel != null)
                    filterExpression = viewModel.GetFilterExpression();
            }

            totalRecords = _pgmCommonService.PGMUnit.SalaryStructureRepository.GetCount(filterExpression);

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),

                //Page number
                PageIndex = request.PageIndex,

                //Total records count
                TotalRecordsCount = totalRecords
            };

            var list = _pgmCommonService.GetSalaryStructureList(filterExpression.ToString(),
                request.SortingName,
                request.SortingOrder.ToString(),
                request.PageIndex,
                request.RecordsCount,
                request.PagesCount.HasValue ? request.PagesCount.Value : 1,
                viewModel.SalaryScaleId,
                viewModel.GradeId,
                viewModel.StepId,
                out totalRecords).OrderBy(x => x.GradeId);

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.ID), new List<object>()
                {
                    d.ID,
                    d.SalaryScaleId,
                    d.SalaryScaleName,
                    d.GradeId,
                    d.GradeName,
                    d.StepId,
                    d.StepName,
                    d.Amount,
                    "Delete"
                }));
            }

            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult GetSalaryScaleforView()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            var gradeList = _pgmCommonService.PGMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.DateOfEffective).ToList();

            foreach (var data in gradeList)
            {
                dic.Add(data.Id, data.SalaryScaleName);
            }

            return PartialView("Select", dic);
        }

        [NoCache]
        public ActionResult GetGradeforView()
        {
            Dictionary<int, string> grade = new Dictionary<int, string>();
            return PartialView("Select", grade);
        }

        [NoCache]
        public ActionResult GetGradeStepforView()
        {
            Dictionary<int, int> gradeStep = new Dictionary<int, int>();
            return PartialView("_Select", gradeStep);
        }

        [NoCache]
        public ViewResult Details(int id)
        {
            var salaryStructure = _pgmCommonService.PGMUnit.SalaryStructureRepository.GetByID(id);
            return View(salaryStructure);
        }

        [NoCache]
        public ActionResult Create()
        {
            var mainModel = new SalaryStructureModel();
            mainModel.SalaryStructureDetail = GetSalaryHeads();

            // Fill ViewBag
            ViewBag.salaryscale = GetSalaryScale();
            ViewBag.grade = GetGrade(0);
            ViewBag.gradestep = GetGradeStep(0);

            return View(mainModel);
        }

        [HttpPost]
        public ActionResult Create(SalaryStructureModel model)
        {
            try
            {
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    entity.IUser = User.Identity.Name;
                    entity.IDate = Common.CurrentDateTime;

                    errorList = GetBusinessLogicValidation(entity);
                    if (errorList.Count == 0)
                    {
                        foreach (var structure in entity.PRM_SalaryStructureDetail)
                        {
                            structure.IUser = User.Identity.Name;
                            structure.IDate = DateTime.Now;
                        }

                        _pgmCommonService.PGMUnit.SalaryStructureRepository.Add(entity);
                        _pgmCommonService.PGMUnit.SalaryStructureRepository.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.Message = errorList.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);

                model.SalaryStructureDetail = GetSalaryHeads();
            }

            if (!model.IsSuccessful)
            {
                model.IsConsolidated = _pgmCommonService.PGMUnit.JobGradeRepository.GetByID(model.GradeId).IsConsolidated == true ? true : false;
                foreach (var item in model.SalaryStructureDetail)
                {
                    PopulateHeadAmountTypeList(item);
                }
            }

            FillViewBag(model);

            return View(model);
        }

        private List<SalaryStructureDetailsModel> GetSalaryHeads()
        {
            //getSalaryHead
            var salaryHead = _pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll().Where(h=> h.IsActiveHead).ToList();
            var salaryDetails = new List<SalaryStructureDetailsModel>();
            foreach (var item in salaryHead)
            {
                var childModel = new SalaryStructureDetailsModel();
                PopulateHeadAmountTypeList(childModel);
                childModel.HeadId = item.Id;
                childModel.IsTaxable = item.IsTaxable;
                childModel.HeadType = item.HeadType;
                childModel.DisplayHeadName = item.HeadName;
                childModel.AmountType = item.AmountType;
                childModel.IsGrossPayHead = item.IsGrossPayHead;
                salaryDetails.Add(childModel);
            }

            return salaryDetails;
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var salaryHead = _pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll().Where(h=> h.IsActiveHead).ToList();
            var salaryDetails = new List<SalaryStructureDetailsModel>();

            var salaryStructure = _pgmCommonService.PGMUnit.SalaryStructureRepository.GetByID(id);

            var model = salaryStructure.ToModel();
            ////
            model.InitialBasic = Convert.ToDecimal(salaryStructure.PRM_JobGrade.InitialBasic);
            model.YearlyIncrement = Convert.ToDecimal(salaryStructure.PRM_JobGrade.YearlyIncrement);
            model.IsConsolidated = salaryStructure.PRM_JobGrade.IsConsolidated == true ? true : false;
            model.SalaryScaleId = salaryStructure.SalaryScaleId;
            model.GradeId = salaryStructure.GradeId;
            model.StepId = salaryStructure.StepId;

            SalaryStructureDetailsModel childModel;
            PRM_SalaryStructureDetail SalaryStructureDetail;
            foreach (var item in salaryHead)
            {
                childModel = new SalaryStructureDetailsModel();
                childModel.HeadId = item.Id;

                SalaryStructureDetail = salaryStructure.PRM_SalaryStructureDetail.Where(d => d.HeadId == item.Id).FirstOrDefault();
                if (SalaryStructureDetail != null)
                {
                    childModel = SalaryStructureDetail.ToModel();
                    childModel.IsGrossPayHead = SalaryStructureDetail.PRM_SalaryHead.IsGrossPayHead;
                    childModel.HeadType = SalaryStructureDetail.HeadType;
                    childModel.DisplayHeadName = item.HeadName;
                    childModel.cssSalaryHeadClass = "";
                }
                else
                {
                    childModel.DisplayHeadName = item.HeadName;
                    childModel.HeadType = item.HeadType;
                    childModel.AmountType = item.AmountType;
                    childModel.cssSalaryHeadClass = "cssSalaryHeadClass";
                }

                PopulateHeadAmountTypeList(childModel);

                salaryDetails.Add(childModel);
            }
            ///////// End of structure

            model.SalaryStructureDetail = null;
            model.SalaryStructureDetail = salaryDetails;

            ModelState.Clear();

            #region View Bag
            FillViewBag(model);

            ViewBag.SelectedId = model.GradeId;
            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SalaryStructureModel model)
        {
            try
            {
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {
                    var salaryStructure = model.ToEntity();
                    salaryStructure.EUser = User.Identity.Name;
                    salaryStructure.EDate = Common.CurrentDateTime;

                    var lstSalaryStructureDetails = new ArrayList();

                    if (model.SalaryStructureDetail != null)
                    {
                        PRM_SalaryStructureDetail _salaryStructureDetails;
                        foreach (var salaryStructureDetails in model.SalaryStructureDetail)
                        {
                            _salaryStructureDetails = salaryStructureDetails.ToEntity();

                            _salaryStructureDetails.SalaryStructureId = salaryStructure.Id;
                            // if old item then reflection will retrive old IUser & IDate
                            _salaryStructureDetails.IUser = User.Identity.Name;
                            _salaryStructureDetails.IDate = DateTime.Now;
                            _salaryStructureDetails.EDate = DateTime.Now;
                            _salaryStructureDetails.EUser = User.Identity.Name;

                            lstSalaryStructureDetails.Add(_salaryStructureDetails);
                        }
                    }

                    Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                    NavigationList.Add(typeof(PRM_SalaryStructureDetail), lstSalaryStructureDetails);

                    errorList = GetBusinessLogicValidation(salaryStructure);
                    if (errorList.Count == 0)
                    {
                        _pgmCommonService.PGMUnit.SalaryStructureRepository.Update(salaryStructure, NavigationList);
                        _pgmCommonService.PGMUnit.SalaryStructureRepository.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        model.IsSuccessful = false;
                        model.Message = errorList.FirstOrDefault();// Common.ErrorListToString(errorList);
                    }
                }

            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            if (!model.IsSuccessful)
            {
                model.IsConsolidated = _pgmCommonService.PGMUnit.JobGradeRepository.GetByID(model.GradeId).IsConsolidated == true ? true : false;
                foreach (var item in model.SalaryStructureDetail)
                {
                    PopulateHeadAmountTypeList(item);
                }
            }

            FillViewBag(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(PRM_SalaryStructureDetail) };
                _pgmCommonService.PGMUnit.SalaryStructureRepository.Delete(id, allTypes);
                _pgmCommonService.PGMUnit.SalaryStructureRepository.SaveChanges();
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
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion

        #region Other Public Actions

        [NoCache]
        public ActionResult GetJobGradeBySalaryScaleId(int salaryScaleId)
        {
            var gradeList = _pgmCommonService.PGMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == salaryScaleId);

            return Json(
                new
                {
                    JobGrades = gradeList.Select(x => new { Id = x.Id, GradeName = x.GradeName })
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public ActionResult GetStep(int gradeId)
        {
            var grade = _pgmCommonService.PGMUnit.JobGradeRepository.GetByID(gradeId);
            bool? isConsolidate = _pgmCommonService.PGMUnit.JobGradeRepository.GetByID(gradeId).IsConsolidated;
            return Json(
                new
                {
                    steps = GetGradeStep(gradeId).Select(x => new { Id = x.Id, StepName = x.StepName, StepAmount = x.StepAmount }),
                    ic = grade.IsConsolidated,
                    initialBasic = grade.InitialBasic,
                    yearlyIncrement = grade.YearlyIncrement
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public JsonResult GetStepAmountByStepId(int stepId)
        {
            var step = _pgmCommonService.PGMUnit.JobGradeStepRepository.GetByID(stepId);

            return Json(
                new
                {
                    stepamount = step.StepAmount
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [NoCache]
        public JsonResult GetGradeId(int salaryScaleId, string gradeName)
        {
            int gradeId = 0;
            var grade = _pgmCommonService.PGMUnit.JobGradeRepository.GetAll().FirstOrDefault(x => x.SalaryScaleId == salaryScaleId && x.GradeName == gradeName);
            if (grade != null)
            {
                gradeId = grade.Id;
            }

            return Json(new
            {
                gradeId = gradeId
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private method

        private IList<PRM_SalaryScale> GetSalaryScale()
        {
            return _pgmCommonService.PGMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.SalaryScaleName).ToList();
        }

        private IList<PRM_JobGrade> GetGrade(int salaryScaleId = 0)
        {
            if (salaryScaleId == 0)
            {
                return _pgmCommonService.PGMUnit.JobGradeRepository.GetAll().ToList();
            }
            else
            {
                return
                    _pgmCommonService.PGMUnit.JobGradeRepository.Get(t => t.SalaryScaleId == salaryScaleId).ToList();
            }
        }

        private IList<PRM_GradeStep> GetGradeStep(int gradeId)
        {
            IList<PRM_GradeStep> itemList;
            if (gradeId > 0)
            {
                itemList = _pgmCommonService.PGMUnit.JobGradeStepRepository.Get(q => q.JobGradeId == gradeId).OrderBy(t => t.StepName).ToList();
            }
            else
            {
                itemList = new List<PRM_GradeStep>();
            }
            return itemList;
        }

        private void FillViewBag(SalaryStructureModel model)
        {
            // Fill ViewBag
            IList<PRM_SalaryScale> salScaleList = GetSalaryScale();
            ViewBag.salaryscale = salScaleList;

            IList<PRM_JobGrade> gradeList = GetGrade(model.SalaryScaleId);
            ViewBag.grade = gradeList;

            IList<PRM_GradeStep> GradeStepList = GetGradeStep(model.GradeId);
            ViewBag.gradestep = GradeStepList;
        }

        private void PopulateHeadAmountTypeList(SalaryStructureDetailsModel model)
        {
            //dynamic ddlList;
            model.HeadAmountTypeList = Common.PopulateAmountType().ToList();
        }

        private void PopulateSalaryStructureModelLists(SalaryStructureModel model)
        {
            model.SalaryScaleList = Common.PopulateSalaryScaleDDL(GetSalaryScale().ToList());
            model.GradeList = Common.PopulateJobGradeDDL(GetGrade(model.SalaryScaleId).ToList());
            model.StepList = Common.PopulateStepList(GetGradeStep(model.GradeId));
        }
        
        public List<string> GetBusinessLogicValidation(PRM_SalaryStructure SalaryStructure)
        {
            List<string> errorMessage = new List<string>();

            if (SalaryStructure.PRM_SalaryStructureDetail
                .Where(q => q.AmountType == "Percent" && q.Amount > 100).Count() > 0)
            {
                errorMessage.Add("Amount can't exceed 100 for amount type 'Percent'");
            }

            if (SalaryStructure.PRM_SalaryStructureDetail
                .Where(q => _pgmCommonService.PGMUnit.SalaryHeadRepository
                .GetByID(q.HeadId)
                .IsGrossPayHead == true && q.AmountType == "Percent").Sum(q => q.Amount) > 100)
            {
                errorMessage.Add("Total Gross Pay Head Amount can't exceed 100%");
            }

            return errorMessage;
        }

        #endregion
    }
}