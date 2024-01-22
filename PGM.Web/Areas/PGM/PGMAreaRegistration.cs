using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM
{
    public class PGMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "PGM"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            string nameSpace = AppConstant.ProjectName + ".Areas." + this.AreaName + "." + "Controllers";
            context.MapRoute(
                "PGM_default",
                "PGM/{controller}/{action}/{id}",
                new { Controller="Home", action = "Index", id = UrlParameter.Optional },
                new[] { nameSpace }
            );
        }
    }
}
