using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServiceStack.OrmLite;
using reexmonkey.crosscut.essentials.concretes;

namespace reexmonkey.xcal.infrastructure.ormlite.extensions
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

        public static void SaveAllExceptThese<T>(this IDbConnection db, IEnumerable<T> all, IEnumerable<T> these)
            where T : class, IEquatable<T>, new()
        {
            if (!all.NullOrEmpty() && !these.NullOrEmpty())
            {
                var diffs = all.Except(these);
                if (!diffs.NullOrEmpty()) db.SaveAll(diffs);
            }
            else if (!all.NullOrEmpty() && these.NullOrEmpty()) db.SaveAll(all);
            else if (all.NullOrEmpty() && !these.NullOrEmpty()) return;
            else return;
        }


        public static void DeleteTheseExceptAll<T>(this IDbConnection db, IEnumerable<T> all, IEnumerable<T> these)
            where T : class, IEquatable<T>, new()
        {
            if (!all.NullOrEmpty() && !these.NullOrEmpty())
            {
                var diffs = these.Except(all);
                if (!diffs.NullOrEmpty()) db.DeleteAll(diffs);
            }
            else if (!these.NullOrEmpty() && all.NullOrEmpty()) db.DeleteAll(these);
            else if (all.NullOrEmpty() && !these.NullOrEmpty()) return;
            else return;        
        
        
        }


    }
}
