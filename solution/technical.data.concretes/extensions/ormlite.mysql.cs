using ServiceStack.OrmLite;
using System.Data;

namespace reexjungle.technical.data.concretes.extensions.ormlite.mysql
{
    public static class MySqlOrmLiteExtensions
    {
        public static void DropSchema(this IDbConnection db, string db_name)
        {
            db.Exec(x =>
            {
                x.CommandText = string.Format("DROP SCHEMA {0}", db_name);
                x.ExecuteNonQuery();
            });
        }

        public static void CreateSchemaIfNotExists(this IDbConnection db, string db_name, bool overwrite = false)
        {
            if (overwrite) db.DropSchema(db_name);
            db.Exec(x =>
                {
                    x.CommandText = (overwrite)
                        ? string.Format("CREATE SCHEMA {0}", db_name)
                        : string.Format("CREATE SCHEMA IF NOT EXISTS {0}", db_name);
                    x.ExecuteNonQuery();
                });
        }
    }
}