using System;


namespace DAL.PGM.CustomEntities
{
    public class EmployeeInfoDDLModel
    {
        public int Id { get; set; }
        public String FullName { get; set; }
    }

    public class EmployeeInfoDDLQueryModel
    {
        public int Id { get; set; }
        public String FullName { get; set; }
        public String EmpID { get; set; }
        public DateTime? DateOfInactive { get; set; }
    }
}
