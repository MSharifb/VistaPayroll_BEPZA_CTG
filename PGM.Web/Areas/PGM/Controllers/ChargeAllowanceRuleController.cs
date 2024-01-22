using Domain.PGM;
using PGM.Web.Areas.PGM.Models.ChargeAllowanceRule;
using PGM.Web.Utility;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class ChargeAllowanceRuleController : Controller
    {
        #region Properties

        public string Message { get; set; }

        #endregion

        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor

        public ChargeAllowanceRuleController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }

        #endregion

        #region Action

        public ActionResult Index(string message, bool? IsSuccessful)
        {
            var model = new ChargeAllowanceRuleModel();
            var entity = _pgmCommonservice.PGMUnit.ChargeAllowanceRuleRepositoty.GetAll().FirstOrDefault();

            if (entity != null)
            {
                model = entity.ToModel();
                model.HasData = true;
                model.Mode = CrudeAction.Edit;
            }
            else
            {
                model.Mode = CrudeAction.Create;
            }

            PrepareModel(model);

            if (!String.IsNullOrEmpty(message))
            {
                model.IsSuccessful = Convert.ToBoolean(IsSuccessful);
                model.Message = message;
            }
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Create(ChargeAllowanceRuleModel model, string btnSubmit)
        {
            Message = CheckBusinessRule(model);

            try
            {
                if (ModelState.IsValid && string.IsNullOrEmpty(Message))
                {
                    if (btnSubmit == "Save")
                    {
                        var entity = model.ToEntity();
                        entity.IUser = User.Identity.Name;
                        entity.IDate = DateTime.Now;

                        _pgmCommonservice.PGMUnit.ChargeAllowanceRuleRepositoty.Add(entity);
                        _pgmCommonservice.PGMUnit.ChargeAllowanceRuleRepositoty.SaveChanges();

                        model.IsSuccessful = true;
                        model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.HasData = true;
                        return RedirectToAction("Index", new { IsSuccessful = model.IsSuccessful, message = model.Message });
                    }
                }
                else
                {
                    model.IsSuccessful = false;
                    model.Message = string.IsNullOrEmpty(Message) ? Common.GetCommomMessage(CommonMessage.InsertFailed) : Message;
                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            PrepareModel(model);
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public ActionResult Edit(ChargeAllowanceRuleModel model, string btnSubmit)
        {
            Message = CheckBusinessRule(model);
            try
            {
                if (ModelState.IsValid && string.IsNullOrEmpty(Message))
                {
                    if (btnSubmit == "Update")
                    {
                        var entity = model.ToEntity();
                        entity.EDate = DateTime.Now;
                        entity.EUser = User.Identity.Name;

                        _pgmCommonservice.PGMUnit.ChargeAllowanceRuleRepositoty.Update(entity);
                        _pgmCommonservice.PGMUnit.ChargeAllowanceRuleRepositoty.SaveChanges();

                        model.IsSuccessful = true;
                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        return RedirectToAction("Index", new { IsSuccessful = model.IsSuccessful, message = model.Message });
                    }
                }
                else
                {
                    model.IsSuccessful = false;
                    model.Message = string.IsNullOrEmpty(Message) ? Common.GetCommomMessage(CommonMessage.UpdateFailed) : Message;
                }
            }
            catch (Exception ex)
            {
                model.IsSuccessful = false;
                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            PrepareModel(model);

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            string message = "";
            bool isSuccessfull = false;

            try
            {
                _pgmCommonservice.PGMUnit.ChargeAllowanceRuleRepositoty.Delete(id);
                _pgmCommonservice.PGMUnit.ChargeAllowanceRuleRepositoty.SaveChanges();
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

        private void PrepareModel(ChargeAllowanceRuleModel model)
        {
            var list = _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(x => x.HeadType == "Addition").OrderBy(x => x.HeadName).ToList();
            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(list);

            model.BaseOnList = Common.PopulateBaseOn();
        }

        private string CheckBusinessRule(ChargeAllowanceRuleModel model)
        {
            string Message = default(string);

            if (model.ChargeAllowancePercent > 100)
            {
                Message = "Charge Allowance Percent could not greater then 100.";
                return Message;
            }

            return Message;
        }

        #endregion
    }
}