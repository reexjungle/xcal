using reexjungle.crosscut.operations.contracts;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace reexjungle.xcal.crosscut.concretes.operations
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

        [StringLength(int.MaxValue)]
        public string Message { get; set; }
    }
}