using Domain.PGM;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class PGMCommonController : BaseController
    {
        #region Fields

        private readonly PGMCommonService _pgmCommonservice;

        #endregion

        #region Constructor

        public PGMCommonController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }

        #endregion

        [NoCache]
        public JsonResult GetEmployeeInfo(int employeeId)
        {
            try
            {
                var emp = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeById(employeeId);

                if (emp != null)
                {
                    Decimal basicSalary = _pgmCommonservice.PGMUnit.FunctionRepository.GetBasicSalary(employeeId);

                    return Json(new
                    {
                        EmployeeId = emp.Id,
                        EmpID = emp.EmpID,
                        EmployeeInitial = emp.EmployeeInitial,
                        EmployeeName = emp.FullName,
                        EmployeeDesignation = emp.DesignationName,
                        EmployeeDepartment = emp.DivisionName,
                        BasicSalary = basicSalary,
                        JoiningDate = emp.DateofJoining,
                        ConfirmationDate = emp.DateofConfirmation,
                        IsInactive = emp.DateofInactive != null ? true : false,
                        InactiveDate = emp.DateofInactive,
                        AccountNumber = emp.BankAccountNo,
                        Success = true,
                        Message = "Success"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "NoEmployeeFound"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);

                return Json(new
                {
                    Success = false,
                    Message = errMsg
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}