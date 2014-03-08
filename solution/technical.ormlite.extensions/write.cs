using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;
using reexmonkey.crosscut.essentials.concretes;

namespace reexmonkey.technical.ormlite.extensions
{
    public static class OrmLiteWriteExtensions
    {
        public static void DropDatabase(this IDbConnection db, string name)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("DROP DATABASE {0}", name);
                x.ExecuteNonQuery();
            });
        }

        public static void DropSchema(this IDbConnection db, string name)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("DROP SCHEMA {0}", name);
                x.ExecuteNonQuery();
            });
        }

        public static void CreateSchema(this IDbConnection db, string name)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("CREATE SCHEMA {0}", name);
                x.ExecuteNonQuery();
            });
        }

        public static void CreateSchemaIfNotExists(this IDbConnection db, string name)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("CREATE SCHEMA IF NOT EXISTS {0}", name);
                x.ExecuteNonQuery();
            });
        }
    }
}
