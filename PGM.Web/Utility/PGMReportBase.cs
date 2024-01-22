using DAL.PGM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using Domain.PGM;
using Microsoft.Reporting.WebForms;


namespace PGM.Web.Utility
{
    public class PGMReportBase : Page
    {
        #region Fields-------------

        internal readonly PGMEntities _pgmContext;
        internal readonly Common _common;
        internal readonly PGM_ExecuteFunctions _pgmExecuteFunctions;

        CustomMembershipProvider _provider = new CustomMembershipProvider();

        #endregion

        #region Consturctor--------------

        public PGMReportBase()
        {
            _pgmContext = new PGMEntities();
            _common = new Common();
            _pgmExecuteFunctions = new PGM_ExecuteFunctions(_pgmContext);
        }

        public dynamic GetZoneInfoForReportHeader()
        {
            return from c in _pgmExecuteFunctions.GetZoneInfoList()
                   where c.Id == LoggedUserZoneInfoId
                   select new
                   {
                       c.CompanyName,
                       c.CompanyLogo,
                       ZoneId = c.Id,
                       c.ZoneName,
                       c.ZoneAddress,
                       c.ZoneCode,
                       c.IsHeadOffice
                   };
        }

        public int LoggedUserZoneInfoId
        {
            get { return MyAppSession.ZoneInfoId; }
        }

        public IList<SelectListItem> GetZoneDDL()
        {
            var userZoneList = _provider.GetZoneList(HttpContext.Current.User.Identity.Name, LoggedUserZoneInfoId);

            HashSet<int> zoneIDs = new HashSet<int>(userZoneList.Select(s => s.ZoneId));
            var sortZoneList = _pgmContext.vwPGMZoneInfoes.Where(x => zoneIDs.Contains(x.Id)).OrderBy(s => s.SortOrder).ToList();

            var resultList = new List<SelectListItem>();
            foreach (var item in sortZoneList)
            {
                if (userZoneList.Count == 1)
                {
                    resultList.Add(new SelectListItem()
                    {
                        Text = item.ZoneName,
                        Value = item.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    resultList.Add(new SelectListItem()
                    {
                        Text = item.ZoneName,
                        Value = item.Id.ToString()
                    });
                }

            }

            return resultList.DistinctBy(x => x.Value).ToList();

        }

        public string ConvertListToString<T>(T[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var value in array)
            {
                builder.Append(value);
                builder.Append(',');
            }
            if (builder.Length > 0) builder.Length--;
            return builder.ToString();
        }

        #endregion

        public dynamic GetEmployeeList()
        {
            dynamic dataSource = null;
            HashSet<int> zoneIDs = new HashSet<int>(MyAppSession.SelectedZoneList.Select(s => s.Id));
            if (zoneIDs != null && zoneIDs.Count > 0)
            {
                dataSource = _pgmExecuteFunctions.GetEmployeeListForReport(zoneIDs)
                    .Select(q => new
                    {
                        Id = q.Id,
                        DisplayText = q.FullName
                    }).ToList();
            }

            return dataSource;
        }

        public IList<SelectListItem> ZoneListCached { get { return GetZoneDDL(); } }

        public class FilteredZoneList
        {
            public int Id { get; set; }
            public String ZoneName { get; set; }

            public FilteredZoneList(int id, string zoneName)
            {
                this.Id = id;
                this.ZoneName = zoneName;
            }
        }

        public static bool ExportToPDF(ReportViewer viewer, string fileName)
        {
            try
            {
                // Check if TempFiles directory not exists
                String physicalDirectoryPath = HttpContext.Current.Server.MapPath(Common.TemporaryFileRepository);
                if (!Directory.Exists(physicalDirectoryPath))
                {
                    Directory.CreateDirectory(physicalDirectoryPath);
                }

                String newFilePath = Common.TemporaryFileRepository + fileName;
                FileStream file = new FileStream(HttpContext.Current.Server.MapPath(newFilePath), FileMode.Create);

                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = viewer.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings);

                using (FileStream fs = file)
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                HttpContext.Current.Response.Redirect(newFilePath, "_blank", String.Empty);
            }
            catch (Exception ex)
            {
                CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }

            return true;
        }
    }

    public class RdlcReportHelper
    {
        public static Stream FixHeadWidth(String reportPath)
        {
            XDocument reportXml = XDocument.Load(reportPath);

            foreach (var element in reportXml.Descendants(XName.Get("Value",
                @"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition")))
            {
                XAttribute attribute = element.Attribute(XName.Get("LocID",
                    @"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"));

                if (attribute != null)
                {

                }
            }

            Stream ms = new MemoryStream();
            reportXml.Save(ms, SaveOptions.OmitDuplicateNamespaces);
            ms.Position = 0;

            return ms;
        }
    }


}