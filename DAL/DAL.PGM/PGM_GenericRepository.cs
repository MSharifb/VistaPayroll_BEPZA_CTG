using DAL.Infrastructure;

namespace DAL.PGM
{
    public class PGM_GenericRepository<T> : DataRepository<T> where T : class
    {
        public PGM_GenericRepository(PGMEntities context)
            : base(context)
        {
            //constructor
        }
    }
}
