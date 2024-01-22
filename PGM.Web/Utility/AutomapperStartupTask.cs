using AutoMapper;
using Utility;

namespace PGM.Web.Utility
{
    public class AutomapperStartupTask : IStartupTask
    {
        public void Execute()
        { 
            PGMMapper _pgmMapper = new PGMMapper();
        }

        protected virtual void ViceVersa<T1, T2>()
        {
            Mapper.CreateMap<T1, T2>();
            Mapper.CreateMap<T2, T1>();
        }
    }
}