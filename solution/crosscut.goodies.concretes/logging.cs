using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.MySql.DataAnnotations;

namespace reexmonkey.crosscut.goodies.concretes
{
    [Alias("nlogtable")]
    public class NlogTable : ILogTable
    {
        [Index(true)]
        public string Guid { get; set; }
        public string Application { get; set; }
        public string Timestamp { get; set; }
        public string Origin { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        [Text]
        public string Message { get; set; }
    }
}
