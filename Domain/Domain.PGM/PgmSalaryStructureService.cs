using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.PGM;

namespace Domain.PGM
{
    public class PgmSalaryStructureService : PGMCommonService
    {
        public PgmSalaryStructureService(PGM_UnitOfWork uow)
            : base(uow)
        {
        }


        public IList<PRM_SalaryStructureDetail> GetSalaryStrutureDetails(int gradeId, int stepId, out int salaryStructureId)
        {
            var query = from ssd in base.PGMUnit.SalaryStructureDetailRepository.Fetch()
                join sh in base.PGMUnit.SalaryHeadRepository.GetAll() on ssd.HeadId equals sh.Id into ssh
                from sh in ssh.DefaultIfEmpty()
                where ssd.PRM_SalaryStructure.GradeId == gradeId
                      && ssd.PRM_SalaryStructure.StepId == stepId
                      && sh.IsActiveHead
                orderby sh.SortOrder
                select ssd;

            var queryId = from q in query
                select q.PRM_SalaryStructure.Id;

            var result = query.ToList();
            salaryStructureId = queryId.FirstOrDefault();

            return result;
        }


    }
}
