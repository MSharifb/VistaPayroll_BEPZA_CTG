
using System.Collections.Generic;
using System.Linq;
using DAL.PGM;

namespace Domain.PGM
{
    public class PgmEmployeeSalaryStructureService : PGMCommonService
    {
        public PgmEmployeeSalaryStructureService(PGM_UnitOfWork uow)
            : base(uow)
        {
        }

        public IList<PRM_EmpSalaryDetail> GetEmpSalaryStructureDetails(int empId, out int salaryStructureId)
        {
            var query = from sd in base.PGMUnit.EmpSalaryDetailRepository.Fetch()
                join sh in base.PGMUnit.SalaryHeadRepository.GetAll() on sd.HeadId equals sh.Id into ssh
                from sh in ssh.DefaultIfEmpty()

                where sd.EmployeeId == empId && sh.IsActiveHead

                orderby sh.SortOrder

                select sd;

            var queryId = from s in base.PGMUnit.EmpSalaryRepository.Fetch()
                where s.EmployeeId == empId
                select s.SalaryStructureId;


            var result = query.ToList();
            salaryStructureId = queryId.FirstOrDefault();

            return result;
        }

    }
}
