namespace PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure
{
    public class EmployeeModel
    {
        #region Ctor
        public EmployeeModel()
        {
            EmploymentInfo = new EmployeeInfoModel();
            EmployeeSalary = new EmployeeSalaryStructureModel();
        }
        #endregion

        #region Standerd Property
        public int Id { get; set; }
        public string EmpId { get; set; }
        public EmployeeInfoModel EmploymentInfo{get;set;}
        public EmployeeSalaryStructureModel EmployeeSalary { get; set; }
        #endregion

        #region Others
        public string ViewType{get;set;}
        #endregion
    }
}