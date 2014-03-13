using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;

namespace reexmonkey.technical.data.concretes
{
    public static class MySqlOrmLiteExtensions
    {
        public static void DropDatabase(this IDbConnection db, string database)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("DROP DATABASE {0}", database);
                x.ExecuteNonQuery();
            });
        }

        public static void DropSchema(this IDbConnection db, string schema)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("DROP SCHEMA {0}", schema);
                x.ExecuteNonQuery();
            });
        }

        public static void CreateSchema(this IDbConnection db, string schema)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("CREATE SCHEMA {0}", schema);
                x.ExecuteNonQuery();
            });
        }

        public static void CreateSchemaIfNotExists(this IDbConnection db, string schema)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("CREATE SCHEMA IF NOT EXISTS {0}", schema);
                x.ExecuteNonQuery();
            });
        }
    }
}
