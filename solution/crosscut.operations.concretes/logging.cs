using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.MySql.DataAnnotations;
using reexjungle.crosscut.operations.contracts;

namespace reexjungle.crosscut.operations.concretes
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
