using DAL.PGM;
using DAL.PGM.CustomEntities;
using DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.PGM
{
    public class PGMMonthlySalaryService : PGMCommonService
    {
        public PGMMonthlySalaryService(PGM_UnitOfWork uow) : base(uow) { }

        public IList<SalaryMonthInfo> GetSalaryInfoList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount, string year, string month, int zoneId)
        {
            var query = @"SELECT DISTINCT
                            PS.SalaryYear,
                            PS.SalaryMonth,
                            ISNULL(confirmInfo.IsConfirmed, 0) AS IsConfirmed
                        FROM
                            PGM_Salary PS LEFT JOIN
                            (
	                            SELECT DISTINCT SalaryYear, SalaryMonth, IsConfirmed
	                            FROM PGM_Salary
	                            WHERE IsConfirmed = 1
                                    AND SalaryWithdrawFromZoneId = " + zoneId + @"
                            ) confirmInfo ON PS.SalaryYear = confirmInfo.SalaryYear AND PS.SalaryMonth = confirmInfo.SalaryMonth
                        WHERE PS.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(year))
                query += " AND PS.SalaryYear LIKE '%" + year + "%'";

            if (!string.IsNullOrEmpty(month))
                query += " AND PS.SalaryMonth LIKE '%" + month + "%'";

            var data = PGMUnit.SalaryMonthList.GetWithRawSql(query)
                .OrderByDescending(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01"))
                .AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }


        //
        public IList<MonthlySalaryDetail> GetSalaryDetailInfoList(string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount
            , String empId = ""
            , String year = ""
            , String month = ""
            , int DivisionId = 0
            , int EmploymentTypeId = 0
            , String EmployeeInitial = ""
            , bool? IsWithheld = null
            , int zoneId = 0)
        {
            //
            var query = @"SELECT * FROM (SELECT Id
                                        , SalaryYear
                                        , SalaryMonth
                                        , EmployeeId
                                        , EmployeeInitial
                                        , DivisionId
                                        , EmploymentTypeId
                                        , EmpID
                                        , FullName
                                        , AccountNo
                                        , IsWithheld
                                        , IsPaid
                                        , SUM(GrossSal) AS GrossSal
                                        , SUM(TotalDeduction) AS TotalDeduction
                                        , SUM(NetPay) AS NetPay
                                        , SortingOrder
                                    FROM (
                                        SELECT Id,SalaryYear ,SalaryMonth , EmployeeId, EmployeeInitial, DivisionId, EmploymentTypeId, EmpID, FullName, AccountNo, IsWithheld, IsPaid, [Gross Salary] as GrossSal, [Total Deduction] as TotalDeduction, [Net Payable] as NetPay, SortingOrder
                                        FROM
                                        (SELECT S.Id, S.SalaryYear, S.SalaryMonth, S.EmployeeId, E.EmployeeInitial, S.DivisionId, S.EmploymentId as 'EmploymentTypeId', E.EmpID, E.FullName, S.AccountNo, S.IsWithheld, S.IsPaid, SD.HeadName, SD.HeadAmount, S.IsConfirmed, D.SortingOrder
                                            FROM PGM_Salary S
                                            INNER JOIN PRM_EmploymentInfo E ON S.EmployeeId = E.Id
                                            INNER JOIN PRM_Designation D ON E.DesignationId = D.Id
                                            INNER JOIN PGM_SalaryDetail SD ON S.Id=SD.SalaryId
                                        WHERE S.SalaryWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(empId))
                query += " AND EmpID = '" + empId + "'";

            if (!string.IsNullOrEmpty(year))
                query += " AND S.SalaryYear = '" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += " AND S.SalaryMonth= '" + month + "'";

            query += @"AND SD.HeadName IN ('Gross Salary','Total Deduction','Net Payable')) p
                        PIVOT( SUM(HeadAmount) FOR HeadName IN ([Gross Salary],[Total Deduction],[Net Payable])) as Pvt
                    ) tbl
                    GROUP BY Id,SalaryYear ,SalaryMonth , EmployeeId, EmployeeInitial, DivisionId, EmploymentTypeId, EmpID, FullName, AccountNo, IsWithheld, IsPaid, SortingOrder
                    ) tbl WHERE 0=0 ";

            if (DivisionId != 0)
                query += " AND DivisionId=" + DivisionId;

            if (EmploymentTypeId != 0)
                query += " AND EmploymentTypeId=" + EmploymentTypeId;

            if (!string.IsNullOrEmpty(EmployeeInitial))
                query += " AND EmployeeInitial LIKE '%" + EmployeeInitial + "%'";

            if (IsWithheld == true)
                query += " AND IsWithheld=" + 1;

            if (IsWithheld == false)
                query += " AND IsWithheld=" + 0;

            query += " ORDER BY SortingOrder ASC ";

            var data = PGMUnit.MonthlySalaryDetailInfo.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(sortExpression))
                {
                    return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
                }
                else
                {
                    return data.Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
                }
            }
            //
        }
        //




    }
}
