using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.GratuityRule;
using PGM.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class GratuityRuleController : Controller
    {
        #region Properties

        public string Message { get; set; }

        #endregion

        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor

        public GratuityRuleController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }

        #endregion

        #region Action

        [NoCache]
        public ActionResult Index(string message, bool? IsSuccessful)
        {
            GratuityRuleModel model = new GratuityRuleModel();
            List<int> grossGratuityHeads = new List<int>();
            var entity = _pgmCommonservice.PGMUnit.GratuityPolicyRepository.GetAll().FirstOrDefault();
            if (entity != null)
            {
                model = entity.ToModel();
                model.Mode = CrudeAction.Edit;
                grossGratuityHeads = _pgmCommonservice.PGMUnit.GratuityGrossSalaryHeadRepository.GetAll()
                    .Select(h => h.HeadId).ToList<int>();

               // model.SalaryHeadIds = grossGratuityHeads.ToArray<int>(); // If I assign heads in model then multicombo is not showing selected heads. I do not know why!

                if (model.IsEligibleFromJoiningDate)
                    model.GratuityEligibleFromId = Convert.ToInt32(PGMEnum.GratuityEligibleFromType.Joining_Date);
                else
                    model.GratuityEligibleFromId = Convert.ToInt32(PGMEnum.GratuityEligibleFromType.Confirmation_Date);
            }
            else
            {
                model.Mode = CrudeAction.Create;
                model.EffectiveDate = DateTime.Now;
            }

            PrepareModel(model, grossGratuityHeads.ToArray<int>());

            if (!String.IsNullOrEmpty(message))
            {
                model.IsSuccessful = Convert.ToBoolean(IsSuccessful);
                model.Message = message;
            }
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(GratuityRuleModel model, string btnSubmit)
        {
            string Message = CheckBusinessRule(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    var entity = model.ToEntity();

                    entity.IsEligibleFromJoiningDate = false;
                    if (model.GratuityEligibleFromId == Convert.ToInt32(PGMEnum.GratuityEligibleFromType.Joining_Date))
                        entity.IsEligibleFromJoiningDate = true;

                    // Saving rule
                    _pgmCommonservice.PGMUnit.GratuityPolicyRepository.Add(entity);
                    _pgmCommonservice.PGMUnit.GratuityPolicyRepository.SaveChanges();

                    // Saving gross salary heads
                    foreach (var head in model.SalaryHeadIds)
                    {
                        var salaryHeadEntity = new PGM_GratuityGrossSalaryHead();
                        salaryHeadEntity.HeadId = head;

                        _pgmCommonservice.PGMUnit.GratuityGrossSalaryHeadRepository.Add(salaryHeadEntity);
                    }
                    _pgmCommonservice.PGMUnit.GratuityGrossSalaryHeadRepository.SaveChanges();

                    model.IsSuccessful = true;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    return RedirectToAction("Index", new { IsSuccessful = model.IsSuccessful, message = model.Message });
                }
                catch (Exception ex)
                {
                    model.IsSuccessful = false;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsSuccessful = false;
                model.Message = Message;
            }
            PrepareModel(model, new[] { 0 });

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(GratuityRuleModel model, string btnSubmit)
        {
            string Message = CheckBusinessRule(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    PGM_GratuityRule entity = model.ToEntity();
                    entity.EDate = DateTime.Now;
                    entity.EUser = User.Identity.Name;

                    entity.IsEligibleFromJoiningDate = false;
                    if (model.GratuityEligibleFromId == Convert.ToInt32(PGMEnum.GratuityEligibleFromType.Joining_Date))
                        entity.IsEligibleFromJoiningDate = true;


                    _pgmCommonservice.PGMUnit.GratuityPolicyRepository.Update(entity);
                    _pgmCommonservice.PGMUnit.GratuityPolicyRepository.SaveChanges();


                    #region Upfate Gross Salary Heads
                    // Check whether gross salary heads changed or not.
                    var grossHeadList = _pgmCommonservice.PGMUnit.GratuityGrossSalaryHeadRepository.GetAll();
                    bool notMatch = false;
                    var headList = grossHeadList as PGM_GratuityGrossSalaryHead[] ?? grossHeadList.ToArray();
                    if (model.SalaryHeadIds.Count() == headList.Count())
                    {
                        foreach (var headid in model.SalaryHeadIds)
                        {
                            if (!headList.Select(g => g.HeadId).Contains(headid))
                            {
                                notMatch = true;
                                break;
                            }
                        }
                    }
                    if (notMatch) // if change found from previous condition then update.
                    {
                        // Delete first
                        foreach (var deleteEntity in headList)
                        {
                            _pgmCommonservice.PGMUnit.GratuityGrossSalaryHeadRepository.Delete(deleteEntity);
                        }
                        // Then Update
                        foreach (var head in model.SalaryHeadIds)
                        {
                            var salaryHeadEntity = new PGM_GratuityGrossSalaryHead();
                            salaryHeadEntity.HeadId = head;
                            _pgmCommonservice.PGMUnit.GratuityGrossSalaryHeadRepository.Add(salaryHeadEntity);
                        }
                        _pgmCommonservice.PGMUnit.GratuityGrossSalaryHeadRepository.SaveChanges();
                    } 
                    #endregion

                    model.IsSuccessful = true;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                    return RedirectToAction("Index", new { IsSuccessful = model.IsSuccessful, message = model.Message });
                }
                catch (Exception ex)
                {
                    model.IsSuccessful = false;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.IsSuccessful = false;
                model.Message = Message;
            }

            PrepareModel(model, new[] {0});
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public JsonResult Delete(int id)
        {
            string message = "";
            bool isSuccessfull = false;

            try
            {
                _pgmCommonservice.PGMUnit.GratuityPolicyRepository.Delete(id);
                _pgmCommonservice.PGMUnit.GratuityPolicyRepository.SaveChanges();

                message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                isSuccessfull = true;
            }
            catch (Exception ex)
            {
                isSuccessfull = false;
                message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }
            return Json(new
            {
                Success = isSuccessfull,
                Message = message
            });
        }

        #endregion

        #region Others

        [NoCache]
        private void PrepareModel(GratuityRuleModel model, int[] selectedSalaryHeads)
        {
            model.SalaryHeadList = _pgmCommonservice.PGMUnit.SalaryHeadRepository.GetAll()
                .Where(s => s.HeadType == "Addition")
                .OrderBy(x => x.HeadName)
                .ToList()
                .Select(y => new SelectListItem()
                {
                    Text = y.HeadName,
                    Value = y.Id.ToString(),
                    Selected = selectedSalaryHeads.Contains(y.Id)
                }).ToList();

            model.GratuityEligibleFromList = Common.PopulateGratuityEligibleFromDDL();
        }

        [NoCache]
        private string CheckBusinessRule(GratuityRuleModel model)
        {
            string Message = string.Empty;

            return Message;
        }

        #endregion

    }
}
