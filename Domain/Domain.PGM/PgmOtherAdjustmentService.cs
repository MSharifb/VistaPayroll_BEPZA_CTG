using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.PGM;
using DAL.PGM.CustomEntities;

namespace Domain.PGM
{
    public class PgmOtherAdjustmentService : PGMCommonService
    {
        public PgmOtherAdjustmentService(PGM_UnitOfWork uow)
            : base(uow)
        {
        }
        
        public IList<OtherAdjustDeductSearchModel> OtherAdjustmentSearchList(string month, string year, string empId, string empName, string desigId, int zoneId)
        {

            var query = @"SELECT tr.Id
	                            , tr.EmployeeId
	                            , tr.SalaryMonth
	                            , tr.SalaryYear
	                            , tr.HeadType
	                            , tr.SalaryHeadId
	                            , SalaryHead = Sal.HeadName
	                            , E.EmpID
	                            , EmployeeName = E.FullName
	                            , EmployeeDesignation = D.Name
                                , tr.IsOverrideStructureAmount
	                            , tr.Amount
                             FROM PGM_OtherAdjustDeduct tr
	                            INNER JOIN PRM_EmploymentInfo E on tr.EmployeeId = E.Id
	                            INNER JOIN PRM_Designation D ON E.DesignationId = D.Id
	                            INNER JOIN PRM_SalaryHead Sal ON tr.SalaryHeadId = Sal.Id
                            WHERE E.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(month))
                query += " AND tr.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(year))
                query += " AND tr.SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(empId))
                query += " AND E.EmpID LIKE '%" + empId + "%'";

            if (!string.IsNullOrEmpty(empName))
                query += " AND E.FullName LIKE '%" + empName + "%'";

            if (!string.IsNullOrEmpty(desigId))
                query += " AND D.Id = " + Convert.ToInt32(desigId);

            query += " ORDER BY D.SortingOrder ASC ";

            var data = PGMUnit.OtehrAdjustDeductSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<OtherAdjustDeductSearchModel> OtherAdjustmentListMaster(string month, string year, int zoneId)
        {

            var query = @"SELECT distinct
	                             tr.SalaryMonth
	                            , tr.SalaryYear
                             FROM PGM_OtherAdjustDeduct tr
                                INNER JOIN PRM_EmploymentInfo EH ON tr.EmployeeId = EH.Id
                            WHERE EH.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(month))
                query += " AND tr.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(year))
                query += " AND tr.SalaryYear LIKE '%" + year + "%'";

            var data = PGMUnit.OtehrAdjustDeductSearch.GetWithRawSql(query).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).AsQueryable(); ;

            return data.ToList();
        }

        public IList<OtherAdjustDeductSearchModel> OtherAdjustmentSearchListStyleOne(string month, string year, string empId, string empName, string desigId, int zoneId)
        {

            var query = @"SELECT DISTINCT
	                              tr.EmployeeId
	                            , tr.SalaryMonth
	                            , tr.SalaryYear
	                            , E.EmpID
	                            , EmployeeName = E.FullName
	                            , EmployeeDesignation = D.Name
                                , D.SortingOrder
                             FROM PGM_OtherAdjustDeduct tr
	                            INNER JOIN PRM_EmploymentInfo E on tr.EmployeeId = E.Id
	                            INNER JOIN PRM_Designation D ON E.DesignationId = D.Id
                            WHERE E.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(month))
                query += " AND tr.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(year))
                query += " AND tr.SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(empId))
                query += " AND E.EmpID LIKE '%" + empId + "%'";

            if (!string.IsNullOrEmpty(empName))
                query += " AND E.FullName LIKE '%" + empName + "%'";

            if (!string.IsNullOrEmpty(desigId))
                query += " AND D.Id = " + Convert.ToInt32(desigId);

            query += " ORDER BY D.SortingOrder ASC ";

            var data = PGMUnit.OtehrAdjustDeductSearch.GetWithRawSql(query);

            return data.ToList();
        }

        public IList<OtherAdjustDeductSearchModel> OtherAdjustmentListMasterStyleOne(string month, string year, int zoneId)
        {

            var query = @"SELECT distinct
	                             tr.SalaryMonth
	                            , tr.SalaryYear
                             FROM PGM_OtherAdjustDeduct tr
                                INNER JOIN PRM_EmploymentInfo EH ON tr.EmployeeId = EH.Id
                            WHERE EH.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(month))
                query += " AND tr.SalaryMonth LIKE '%" + month + "%'";

            if (!string.IsNullOrEmpty(year))
                query += " AND tr.SalaryYear LIKE '%" + year + "%'";

            var data = PGMUnit.OtehrAdjustDeductSearch.GetWithRawSql(query).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).AsQueryable(); ;

            return data.ToList();
        }
    }
}
