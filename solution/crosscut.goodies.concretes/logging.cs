using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace reexmonkey.crosscut.goodies.concretes
{
    public class NlogTable : ILogTable
    {
        public string Guid { get; set; }
        public string Message { get; set; }
        public string CreationDate { get; set; }
        public string Origin { get; set; }
        public string LogLevel { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
    }
}
