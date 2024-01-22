using Domain.PGM;
using PGM.Web.Areas.PGM.Models.SalaryRevisionwithArrearAdjustment;
using PGM.Web.Utility;

using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class SalaryRevisionwithArrearAdjustmentController : Controller
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor
        public SalaryRevisionwithArrearAdjustmentController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }
        #endregion

        // GET: PGM/SalaryRevisionwithArrearAdjustment
        public ActionResult Index()
        {
            var model = new SalaryRevisionwithArrearAdjustmentViewModel();
            return View(model);
        }

        public ActionResult Create()
        {
            SalaryRevisionwithArrearAdjustmentViewModel model = new SalaryRevisionwithArrearAdjustmentViewModel();
            return View(model);
        }

        #region Employee Information
        [NoCache]
        public JsonResult GetEmployeeInfo(int empId)
        {
            var emp = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeById(empId);

            return Json(new
            {
                EmpId = emp.EmpID,
                SalaryStructureId = emp.SalaryStructureId,
                SalaryScaleId = emp.SalaryScaleId,
                GradeId = emp.JobGradeId,
                StepId = emp.StepId,
                EmployeeName = emp.FullName,
                Designation = emp.DesignationName,
                SalaryScaleName = emp.SalaryScaleName,
                GradeName = emp.JobGradeName,
                StepName = emp.StepName,
                GrossSalary = emp.GrossSalary,
                isConsolidated = emp.isConsolidated,
            });

        }
        #endregion

    }
}