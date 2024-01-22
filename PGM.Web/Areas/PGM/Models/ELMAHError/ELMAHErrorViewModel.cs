using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.ELMAHError
{
    public class ELMAHErrorViewModel
    {

        public String ErrorId { get; set; }
        public String Application { get; set; }
        public String Host { get; set; }
        public String Type { get; set; }
        public String Source { get; set; }
        public String Message { get; set; }
        public String User { get; set; }
        public String StatusCode { get; set; }
        public String TimeUtc { get; set; }
        public String Sequence { get; set; }
        public String REMOTE_ADDR { get; set; }
        public String HTTP_USER_AGENT { get; set; }
        public String HTTP_REFERER { get; set; }
        public String PATH_TRANSLATED { get; set; }

    }
}