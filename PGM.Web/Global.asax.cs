using Autofac;
using Autofac.Integration.Mvc;
using DAL.PGM;
using Domain.PGM;
using PGM.Web.Filters;
using PGM.Web.Utility;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace PGM.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new AuthorizationFilterAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional }, // Parameter defaults
                new[] { AppConstant.ProjectName + ".Controllers" });

            routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            ModelBinders.Binders.DefaultBinder = new CustomModelBinder();

            DB.Utility.strDBConnectionString = AppConstant.ConnectionString;    // ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            DB.Utility.strDBProvider = AppConstant.Provider;

            #region IoC registration

            var builder = new ContainerBuilder();
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());


            #region DataLayer DI for PGM
            builder.Register(c => new PGMEntities()).InstancePerRequest();
            builder.RegisterType<PGM_UnitOfWork>().InstancePerRequest();
            builder.RegisterGeneric(typeof(PGM_GenericRepository<>)).InstancePerRequest();
            builder.RegisterType<PGM_ExecuteFunctions>().InstancePerRequest();
            #endregion

            #region Service DI for PGM
            builder.RegisterType<PGMCommonService>().InstancePerRequest();
            builder.RegisterType<PgmArrearAdjustmentService>().InstancePerRequest();
            builder.RegisterType<PgmBonusService>().InstancePerRequest();
            builder.RegisterType<PGMMonthlySalaryService>().InstancePerRequest();
            builder.RegisterType<PgmEmployeeSalaryStructureService>().InstancePerRequest();
            builder.RegisterType<PgmSalaryStructureService>().InstancePerRequest();
            builder.RegisterType<PgmOtherAdjustmentService>().InstancePerRequest();
            #endregion

            #region Report

            builder.RegisterType<PGMReportBase>().InstancePerRequest();

            #endregion

            ///**********Example: Dot not Delete **********************************
            //builder.RegisterType<PRM_UnitOfWork>().PropertiesAutowired().InstancePerHttpRequest();
            //builder.Register(x => new PRM_UnitOfWork(new PRM_MfsIwmEntities())).InstancePerHttpRequest();          
            //builder.RegisterType<PRM_GenericRepository<PRM_JobGrade>>().InstancePerHttpRequest();  
            //builder.RegisterGeneric(typeof(PRM_GenericRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
            //builder.RegisterGeneric(typeof(DataRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
            //// Automapper
            //builder.RegisterType<AutomapperStartupTask>().As<IStartupTask>().InstancePerHttpRequest();

            /////////////////////////////////////////////////////////////////////////


            //build container
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            #endregion

            #region AutoMapper

            var mapper = new AutomapperStartupTask();
            mapper.Execute();

            #endregion

            // Custom ModelMetadata for data annotation DisplayName
            ModelMetadataProviders.Current = new ConventionalModelMetadataProvider();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        #region All Enums

        //public enum EnumDocumentApprovalInformation
        //{
        //    Draft,
        //    Prepared,
        //    Submitted,
        //    Rejected,
        //    Approved,
        //    Reviewed,
        //    Recommended
        //}

        //public enum CPFMembershipStatus
        //{
        //    Active,
        //    Inactive
        //}

        //public enum CPFApprovalStatus
        //{
        //    Draft,
        //    Submitted,
        //    Reviewed,
        //    Approved,
        //    Rejected
        //}

        //public enum CPFTransactionType
        //{
        //    Debit,
        //    Credit
        //}

        //public enum ADCBeneficiaryType
        //{
        //    Employee,
        //    Department,
        //    Other
        //}

        //public enum PIMApprovalStatus
        //{
        //    Draft = 1,
        //    Submitted = 2,
        //    Reviewed = 3,
        //    Recommended = 4,
        //    Approved = 5,
        //    Rejected = 6
        //}

        //public enum PIMEDLoginUser
        //{
        //    EdLoginID = 1
        //}

        //public enum PIMActivityStatus
        //{
        //    NotStartedYet = 1,
        //    InProgress = 2,
        //    Complete = 3
        //}

        //public enum PIMBudgetHeadType
        //{
        //    Income,
        //    Expense
        //}

        //public enum ProjectCategory
        //{
        //    Internal = 1,
        //    External = 2
        //};

        #endregion
    }
}