using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;

namespace reexmonkey.technical.data.concretes.extensions.ormlite.mysql
{
    public static class MySqlOrmLiteExtensions
    {
        public static void DropMySqlDatabase(this IDbConnection db, string db_name)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("DROP DATABASE {0}", db_name);
                x.ExecuteNonQuery();
            });
        }

        public static void CreateSchemaIfNotExists(this IDbConnection db, string db_name, bool overwrite = false)
        {
            if (overwrite) db.DropMySqlDatabase(db_name);
            db.Exec(x =>
                {
                    x.CommandText = (overwrite) ? string.Format("CREATE SCHEMA {0}", db_name) :
                string.Format("CREATE SCHEMA IF NOT EXISTS {0}", db_name); ;
                    x.ExecuteNonQuery();
                });

        }
    }
}
