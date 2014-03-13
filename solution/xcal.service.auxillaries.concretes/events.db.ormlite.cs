using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.auxillaries.contracts;

namespace reexmonkey.xcal.service.auxillaries.concretes
{
    public class EventsOrmLiteDb: IOrmLiteDb
    {
        private IDbConnection db;
        private IOrmLiteDb alarm_ormlite_db;

        public IDbConnection DbConnection
        {
            get { return this.db; }
            set 
            {
                if (value == null) throw new ArgumentNullException("DbConnection");
                this.db = value;
            }
        }

        public IOrmLiteDb AlarmOrmLiteDb
        {
            get { return this.alarm_ormlite_db; }
            set 
            {
                if (value == null) throw new ArgumentNullException("AlarmOrmLiteDb");
                this.alarm_ormlite_db = value;
            }
        }


    }
}
