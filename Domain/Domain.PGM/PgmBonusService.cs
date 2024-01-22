using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.PGM.CustomEntities;
using DAL.Infrastructure;
using DAL.PGM;

namespace Domain.PGM
{
    public class PgmBonusService : PGMCommonService
    {
        public PgmBonusService(PGM_UnitOfWork uow) : base(uow) { }
        
        public IList<BonusDetailsSearchModel> GetBonusDetailsList(
            string filterExpression
            , string sortExpression
            , string sortDirection
            , int pageIndex
            , int pageSize
            , int pagesCount
            , string initial = ""
            , string empId = ""
            , string name = ""
            , int? DivisionId = 0
            , int bonusId = 0
            , Boolean isCount = false)
        {
            var query = @"SELECT BD.Id
                                ,BD.BonusId
                                ,BM.BonusYear
                                ,BM.BonusMonth
                                ,BD.EmployeeId
                                ,E.EmpID
                                ,E.EmployeeInitial
                                ,E.FullName
                                ,BD.DivisionId
                                ,D.Name as Division
                                ,E.BankAccountNo AS AccountNo
                                ,BD.EmpBasicSalary
                                ,BD.EmpBonusAmount
                                ,BD.EmpRevenueStamp
                                ,BD.EmpNetPayable
                        FROM PGM_BonusDetail BD 
                            INNER JOIN PRM_EmploymentInfo E ON BD.EmployeeId = E.Id
                            INNER JOIN PGM_Bonus BM ON BM.Id = BD.BonusId
                            LEFT JOIN PRM_Division D ON BD.DivisionId = D.Id
                        where 0=0";

            if (!string.IsNullOrEmpty(initial))
                query += " AND E.EmployeeInitial LIKE '%" + initial.Trim() + "%'";

            if (!string.IsNullOrEmpty(empId))
                query += " AND E.EmpID LIKE '%" + empId.Trim() + "%'";

            if (!string.IsNullOrEmpty(name))
                query += " AND E.FullName LIKE '%" + name.Trim() + "%'";

            if (DivisionId != null && DivisionId != 0)
                query += " AND BD.DivisionId= " + DivisionId;

            if (bonusId != 0)
                query += " AND BD.BonusId= " + bonusId;

            var data = PGMUnit.BonusDetailsSearch.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(sortExpression))
                    return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
                else
                    return data.Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<BonusProcessSearchModel> GetBonusMasterData(
            string filterExpression, string sortExpression, string sortDirection, int pageIndex, int pageSize, int pagesCount, bool isCount, string year = "", string month = "", int zoneId = 0)
        {
            var query = @"SELECT DISTINCT B.Id
                        , B.BonusYear
                        , B.BonusMonth
                        , B.EffectiveDate                        
                        , BT.BonusType
                        , B.BonusTypeId
                        , B.BonusAmount
                        , B.AmountType
                        , B.RevenueStamp
                        , B.IsAll
                        FROM PGM_Bonus B  
                            LEFT JOIN [PGM_BonusDetail] BND ON B.Id = BND.BonusId  
                            LEFT JOIN PGM_BonusType BT ON BT.Id = B.BonusTypeId
                        WHERE BND.BonusWithdrawFromZoneId = " + zoneId;

            if (!string.IsNullOrEmpty(year))
                query += " AND B.BonusYear= '" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += " AND B.BonusMonth= '" + month + "'";

            var data = PGMUnit.BonusProcessSearch.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }

        public IList<IncentiveBonusProcessSearchModel> GetIncentiveBonusProcess(
            string filterExpression, string sortExpression, string sortDirection
            , int pageIndex, int pageSize, int pagesCount, bool isCount
            , string FinancialYear, DateTime IncentiveBonusDate, int DepartmentId
            , int StaffCategoryId, int EmploymentTypeId)
        {
            var query = @"SELECT 
                         IB.[Id] Id 
                        ,IB.FinancialYear                       
                        ,[IncentiveBonusDate]
                        ,ISNULL([DepartmentId], 0) AS DepartmentId
                        ,ISNULL([StaffCategoryId], 0) AS StaffCategoryId
                        ,ISNULL([EmploymentTypeId], 0) AS EmploymentTypeId
	                    ,ISNULL(dep.Name, 'All') AS DepartmentName
	                    ,ISNULL(sCat.Name, 'All') AS StaffCategoryName
	                    ,ISNULL(eType.Name, 'All') AS EmploymentTypeName

                    FROM [dbo].[PGM_IncentiveBonus] IB 
                        LEFT JOIN PRM_Division dep ON dep.Id = IB.DepartmentId
	                    LEFT JOIN PRM_StaffCategory sCat ON sCat.Id = IB.StaffCategoryId
	                    LEFT JOIN PRM_EmploymentType eType ON eType.Id = IB.EmploymentTypeId

                    WHERE 0=0";

            if (!string.IsNullOrEmpty(FinancialYear))
                query += " AND IB.FinancialYear= '" + FinancialYear + "'";

            if (IncentiveBonusDate != DateTime.MinValue)
                query += " AND IB.IncentiveBonusDate= '" + IncentiveBonusDate + "'";

            if (DepartmentId != 0)
                query += " AND IB.DepartmentId = " + DepartmentId;

            if (StaffCategoryId != 0)
                query += " AND IB.StaffCategoryId = " + StaffCategoryId;

            if (EmploymentTypeId != 0)
                query += " AND IB.EmploymentTypeId = " + EmploymentTypeId;

            var data = PGMUnit.IncentiveBonusProcessSearch.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
            }
        }


        public IList<IncentiveBonusDetailProcessSearchModel> GetIncentiveBonusDetailsList(
            string filterExpression
            , string sortExpression
            , string sortDirection
            , int pageIndex
            , int pageSize
            , int pagesCount
            , bool isCount

            , string empId = ""
            , string name = ""
            , int IncentiveBonusId = 0)
        {
            var query = @" SELECT 
	                     BD.Id
	                    ,E.EmpID
	                    ,E.FullName
	                    ,BD.BasicSalary
	                    ,BD.IncentiveBonusAmount
	                    ,BD.RevenueStamp
	                    ,BD.NetPayable
                    FROM PGM_IncentiveBonusDetail BD 
	                    INNER JOIN PRM_EmploymentInfo E ON BD.EmployeeId=E.Id
	                    INNER JOIN PGM_IncentiveBonus BM ON BM.Id=BD.IncentiveBonusId
                    where 0=0 ";

            if (!string.IsNullOrEmpty(empId))
                query += " AND E.EmpID LIKE '%" + empId.Trim() + "%'";

            if (!string.IsNullOrEmpty(name))
                query += " AND E.FullName LIKE '%" + name.Trim() + "%'";

            if (IncentiveBonusId != 0)
                query += " AND BD.IncentiveBonusId= " + IncentiveBonusId;

            var data = PGMUnit.IncentiveBonusDetailProcessSearchRepository.GetWithRawSql(query).AsQueryable();

            if (isCount)
            {
                return data.ToList();
            }
            else
            {
                try
                {
                    return data.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * pageSize).Take(pagesCount * pageSize).ToList();
                }
                catch
                {
                    return data.ToList();
                }
            }
        }

        public IList<BankAdviceLetterSearchModel> GetBonusEmployeeExistList(string year = "", string month = "")
        {
            var query = @" SELECT 'A' as exist FROM PGM_BankLetter B
                         WHERE B.LetterType ='Bonus'";

            if (!string.IsNullOrEmpty(year))
                query += " AND B.SalaryYear = '" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += "  AND B.SalaryMonth= '" + month + "'";

            var data = PGMUnit.BankAdviceLetterSearch.GetWithRawSql(query);
            return data.ToList();
        }

        public IList<BankAdviceLetterSearchModel> GetBonusIndividualEmployeeExistList(
            string year = "", string month = "", int empId = 0)
        {
            var query = @"SELECT 'A' AS EXIST FROM PGM_BankLetter 
                        B INNER JOIN PGM_BankLetterDetail BD ON B.Id=BD.BankLetterId
                        WHERE B.LetterType ='Bonus' ";

            if (!string.IsNullOrEmpty(year))
                query += " AND B.SalaryYear = '" + year + "'";

            if (!string.IsNullOrEmpty(month))
                query += "  AND B.SalaryMonth= '" + month + "'";

            if (empId != 0)
            {
                query += " AND BD.EmployeeId= " + empId;

            }

            var data = PGMUnit.BankAdviceLetterSearch.GetWithRawSql(query);
            return data.ToList();
        }

    }
}
