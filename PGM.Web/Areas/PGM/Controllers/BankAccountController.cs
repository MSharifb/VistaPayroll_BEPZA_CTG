using Domain.PGM;
using PGM.Web.Areas.PGM.Models.BankAccount;
using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class BankAccountController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Ctor
        public BankAccountController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonservice = pgmCommonservice;
        }
        #endregion

        #region Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        [NoCache]
        public ActionResult Index()
        {
            var model = new BankAccountModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, BankAccountModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<BankAccountModel> list = (from BA in _pgmCommonservice.PGMUnit.BankAccount.GetAll()
                                           join BAC in _pgmCommonservice.PGMUnit.AccBankAccountRepository.GetAll() on BA.BankAccountId equals BAC.id
                                           join B in _pgmCommonservice.PGMUnit.AccBankRepository.GetAll() on BAC.bankId equals B.id
                                           join BB in _pgmCommonservice.PGMUnit.AccBankBranchRepository.GetAll() on BAC.bankBranchId equals BB.id

                                           where (string.IsNullOrEmpty(model.AccountFor) || model.AccountFor == BA.AccountFor) && ((model.BankId == BAC.bankId) || model.BankId == 0)
                                           && (BA.ZoneInfoId == LoggedUserZoneInfoId)

                                           select new BankAccountModel()
                                           {
                                               Id = BA.Id,
                                               AccountFor = BA.AccountFor,
                                               BankAccountNo = BAC.accountNumber,
                                               BankName = B.bankName,
                                               BranchName = BB.branchName,
                                               BankAccountId = BAC.id,
                                               BankAddress = BB.address,
                                               BankId = B.id
                                           }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();
            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

            if (request.SortingName == "AccountFor")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AccountFor).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AccountFor).ToList();
                }
            }

            if (request.SortingName == "BankAccountNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankAccountNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankAccountNo).ToList();
                }
            }

            if (request.SortingName == "BankId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankName).ToList();
                }
            }

            if (request.SortingName == "BranchName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BranchName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BranchName).ToList();
                }
            }

            if (request.SortingName == "BankAddress")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankAddress).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankAddress).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.AccountFor,
                    d.BankAccountNo,
                    d.BankName,
                    d.BranchName,
                    d.BankAddress,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new BankAccountModel();
            PrepareModel(model);
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(BankAccountModel model)
        {
            Message = CheckBusinessRule(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.ZoneInfoId = LoggedUserZoneInfoId;

                    _pgmCommonservice.PGMUnit.BankAccount.Add(entity);
                    _pgmCommonservice.PGMUnit.BankAccount.SaveChanges();
                    Message = ErrorMessages.InsertSuccessful;
                }
                catch (Exception ex)
                {
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                Message = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
            }

            return new JsonResult()
            {
                Data = Message,
            };
        }


        public ActionResult BackToList()
        {
            var model = new BankAccountModel();

            return View("_Search", model.BankAccountList);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.BankAccount.GetByID(id, "Id");

            var model = entity.ToModel();

            PrepareModel(model);
            model.Mode = CrudeAction.Edit;

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(BankAccountModel model)
        {
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.ZoneInfoId = LoggedUserZoneInfoId;
                    _pgmCommonservice.PGMUnit.BankAccount.Update(entity, "Id");
                    _pgmCommonservice.PGMUnit.BankAccount.SaveChanges();
                    Message = ErrorMessages.UpdateSuccessful;
                }
                catch (Exception ex)
                {
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                Message = string.IsNullOrEmpty(Message) ? ErrorMessages.UpdateFailed : Message;
            }

            return new JsonResult()
            {
                Data = Message
            };
        }

        [HttpPost]
        [NoCache]
        public ActionResult Delete(int id)
        {
            try
            {
                _pgmCommonservice.PGMUnit.BankAccount.Delete(id);
                _pgmCommonservice.PGMUnit.BankAccount.SaveChanges();

                Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch
            {
                Message = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            }

            return new JsonResult()
            {
                Data = Message
            };
        }

        #endregion

        #region Utils
        [NoCache]
        private void PrepareModel(BankAccountModel model)
        {
            //Account For
            model.AccountForList = AccountForList();

            //List of Bank
            model.BankNameList = _pgmCommonservice.PGMUnit.AccBankRepository.GetAll().OrderBy(x => x.bankName)
                .Select(y => new SelectListItem()
                {
                    Text = y.bankName,
                    Value = y.id.ToString()
                }).ToList();

            model.BankId = Convert.ToInt32((from BBA in _pgmCommonservice.PGMUnit.AccBankAccountRepository.GetAll()
                                            where BBA.id == model.BankAccountId
                                            select BBA.bankId).FirstOrDefault());

            var bankAccNo = (from BBA in _pgmCommonservice.PGMUnit.AccBankAccountRepository.GetAll()
                             where BBA.bankId == model.BankId && LoggedUserZoneInfoId == BBA.ZoneInfoId
                             select BBA).ToList();

            model.BankAccountNoList = bankAccNo.Select(x => new SelectListItem()
                            {
                                Text = x.accountNumber,
                                Value = x.id.ToString()
                            }).ToList();

            model.BranchName = Convert.ToString((from BA in _pgmCommonservice.PGMUnit.AccBankAccountRepository.GetAll()
                                                 join BB in _pgmCommonservice.PGMUnit.AccBankBranchRepository.GetAll() on BA.bankBranchId equals BB.id
                                                 where (BA.id == model.BankAccountId)
                                                 select BB.branchName).FirstOrDefault());
        }

        [NoCache]
        private IList<SelectListItem> AccountForList()
        {
            IList<SelectListItem> AccountFor = new List<SelectListItem>();
            AccountFor = Common.PopulateAccountForList().OrderBy(x => x.Text).ToList();
            return AccountFor;
        }

        public ActionResult GetAccountForList()
        {
            return View("_Select", AccountForList());
        }

        [NoCache]
        public ActionResult GetBankNameList()
        {
            IList<SelectListItem> bankList = new List<SelectListItem>();

            bankList = _pgmCommonservice.PGMUnit.AccBankRepository.GetAll().OrderBy(x => x.bankName)
                .Select(y => new SelectListItem()
                {
                    Text = y.bankName,
                    Value = y.id.ToString()
                }).ToList();


            return View("_Select", bankList);
        }

        [NoCache]
        public JsonResult GetBranchName(int id)
        {
            var branchName = from BBA in _pgmCommonservice.PGMUnit.AccBankAccountRepository.GetAll()
                             join BB in _pgmCommonservice.PGMUnit.AccBankBranchRepository.GetAll() on BBA.bankBranchId equals BB.id
                             where BBA.id == id
                             select BB.branchName;
            return new JsonResult() { Data = branchName, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [NoCache]
        public JsonResult GetBankBranchAddress(int id)
        {
            var branchAddress = from BBA in _pgmCommonservice.PGMUnit.AccBankAccountRepository.GetAll()
                                join BB in _pgmCommonservice.PGMUnit.AccBankBranchRepository.GetAll() on BBA.bankBranchId equals BB.id
                                where BBA.id == id
                                select BB.address;
            return new JsonResult() { Data = branchAddress, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [NoCache]
        public ActionResult GetBankAccNo(int id)
        {
            var bankAccNo = (from BBA in _pgmCommonservice.PGMUnit.AccBankAccountRepository.GetAll()
                             where BBA.bankId == id && LoggedUserZoneInfoId == BBA.ZoneInfoId
                             select BBA).OrderBy(x => x.accountNumber).ToList().
                            Select(x => new SelectListItem()
            {
                Text = x.accountNumber,
                Value = x.id.ToString()
            }).ToList();

            return View("_SelectAccNo", bankAccNo);
        }

        [NoCache]
        private string CheckBusinessRule(BankAccountModel model)
        {
            if (model.BankAddress == string.Empty)
            {
                Message = "Bank Address is required.";
            }
            return Message;
        }
        #endregion
    }
}
