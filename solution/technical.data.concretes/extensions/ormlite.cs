using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using ServiceStack.OrmLite;
using ServiceStack.Common.Utils;
using reexmonkey.foundation.essentials.concretes;

namespace reexmonkey.technical.data.concretes.extensions.ormlite
{
    #region helper enumerations

    public enum Conjunctor
    {
        AND,
        OR
    }

    [Flags]
    public enum JoinMode
    { 
        INNER = 0x0001,
        OUTER = 0x0002,
        LEFT = 0x0004,
        RIGHT = 0x0008,
        FULL = 0x0010
    }

    #endregion

    public static class OrmLiteExtensions
    {

        #region From original service stack source implementation

        internal static IDataReader ExecReader(this IDbCommand cmd, string sql)
        {
            cmd.CommandText = sql;
            return cmd.ExecuteReader();
        }

        internal static List<T> Select<T>(this IDbCommand cmd, string sqlFilter, params object[] filterParams)
        {
            using (var reader = cmd.ExecReader(
                OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), sqlFilter, filterParams)))
            {
                return reader.ConvertToList<T>();
            }
        }

        internal static string GetIdsInSql(this IEnumerable idValues)
        {
            var sql = new StringBuilder();
            foreach (var idValue in idValues)
            {
                if (sql.Length > 0) sql.Append(",");
                sql.AppendFormat("{0}".SqlFormat(idValue));
            }
            return sql.Length == 0 ? null : sql.ToString();
        }

        internal static List<T> GetByIds<T>(this IDbCommand cmd, IEnumerable idValues)
        {
            var sql = idValues.GetIdsInSql();
            return sql == null
                ? new List<T>()
                : Select<T>(cmd, OrmLiteConfig.DialectProvider.GetQuotedColumnName(ModelDefinition<T>.PrimaryKeyName) + " IN (" + sql + ")");
        }

        internal static T FirstOrDefault<T>(this IDbCommand cmd, string filter)
        {
            using (var dbReader = cmd.ExecReader(
                OrmLiteConfig.DialectProvider.ToSelectStatement(typeof(T), filter)))
            {
                return dbReader.ConvertTo<T>();
            }
        }

        internal static T GetByIdOrDefault<T>(this IDbCommand cmd, object idValue)
        {
            return FirstOrDefault<T>(cmd, OrmLiteConfig.DialectProvider.GetQuotedColumnName(ModelDefinition<T>.PrimaryKeyName) + " = {0}".SqlFormat(idValue));
        }

        internal static List<TParam> ReadToList<TParam>(this IDataReader dataReader)
        {
            var to = new List<TParam>();
            using (dataReader)
            {
                while (dataReader.Read())
                {
                    if (dataReader.FieldCount > 1) break;
                    var row = (TParam)dataReader[0];
                    to.Add(row);
                }
            }
            return to;
        }

        internal static int ExecuteSql(this IDbCommand cmd, string sql)
        {
            cmd.CommandText = sql;
            return cmd.ExecuteNonQuery();
        }

        internal static void Update<T>(this IDbCommand cmd, params T[] entities)
        {
            foreach (var entity in entities)
            {
                cmd.ExecuteSql(OrmLiteConfig.DialectProvider.ToUpdateRowStatement(entity));
            }
        }

        internal static void Insert<T>(this IDbCommand cmd, params T[] entities)
        {
            foreach (var entity in entities)
            {
                cmd.ExecuteSql(OrmLiteConfig.DialectProvider.ToInsertRowStatement(cmd, entity));
            }
        }

        internal static void Save<T>(this IDbCommand cmd, T entity, IDbTransaction transaction)
        {
            var id = entity.GetId();
            if (transaction != null) cmd.Transaction = transaction;
            var existing = cmd.GetByIdOrDefault<T>(id);
            if (Equals(existing, default(T))) cmd.Insert(entity);
            else cmd.Update(entity);
        }

        internal static void SaveAll<T>(this IDbCommand cmd, IEnumerable<T> entities, IDbTransaction transaction)
        {
            var rows = entities.ToList();
            var first = rows.FirstOrDefault();
            if (Equals(first, default(T))) return;

            var defkeyvalue = ReflectionUtils.GetDefaultValue(first.GetId().GetType());

            var keymap = (defkeyvalue != null)
                ? rows.Where(x => !defkeyvalue.Equals(x.GetId())).ToDictionary(x => x.GetId())
                : rows.Where(x => x.GetId() != null).ToDictionary(x => x.GetId());

            var existing = cmd.GetByIds<T>(keymap.Keys).ToDictionary(x => x.GetId());
            if (transaction != null) cmd.Transaction = transaction;

            foreach (var row in rows)
            {
                var id = IdUtils.GetId(row);
                if (id != defkeyvalue && existing.ContainsKey(id)) 
                    cmd.Update(row);
                else 
                    cmd.Insert(row);
            }

        }

        #endregion

        #region common dml read operations


        public static string GetQuotedTableName<TTable>()
        {
            var modeldef = ModelDefinition<TTable>.Definition;
            return OrmLiteConfig.DialectProvider.GetQuotedTableName(modeldef);
        }

        public static string GetTableName<TTable>()
        {
            var modeldef = ModelDefinition<TTable>.Definition;
            return OrmLiteConfig.DialectProvider.NamingStrategy.GetTableName(modeldef.ModelName);
        }

        public static string GetPrimaryKeyName<TTable>()
        {
            var modeldef = ModelDefinition<TTable>.Definition;
            return modeldef.PrimaryKey.FieldName;
        }

        public static string GetQuotedPrimaryKeyName<TTable>()
        {
            var modeldef = ModelDefinition<TTable>.Definition;
            return OrmLiteConfig.DialectProvider.GetQuotedColumnName(modeldef.PrimaryKey.FieldName);

        }

        public static string ToQuotedFieldname<TTable>(this string fieldname)
        {
            return OrmLiteConfig.DialectProvider.GetQuotedColumnName(fieldname);
        }

        public static string ToQuotedFieldValue<TValue>(this TValue value)
        {
            return OrmLiteConfig.DialectProvider.GetQuotedValue(value, typeof(TValue));
        }

        public static IEnumerable<string> GetFieldNames<TTable>()
        {
            var modeldef = ModelDefinition<TTable>.Definition;
            return modeldef.FieldDefinitions.Select(x => x.FieldName);
        }

        public static IEnumerable<string> GetQuotedFieldNames<TTable>()
        {
            var modeldef = ModelDefinition<TTable>.Definition;
            return modeldef.FieldDefinitions.Select(x => OrmLiteConfig.DialectProvider.GetQuotedColumnName(x.FieldName));
        }

        public static string PrependTableNameToColumns<TTable>(this string expr, bool quoted = true)
        {
            var modeldef = ModelDefinition<TTable>.Definition;
            var table = (quoted) ? OrmLiteConfig.DialectProvider.GetQuotedTableName(modeldef) : OrmLiteConfig.DialectProvider.NamingStrategy.GetTableName(modeldef.ModelName);
            var fields = (quoted) ? modeldef.FieldDefinitions.Select(x => OrmLiteConfig.DialectProvider.GetQuotedColumnName(x.FieldName)) : modeldef.FieldDefinitions.Select(x => x.FieldName);
            var sb = new StringBuilder(expr);
            foreach (var field in fields)
            {
                sb = sb.Replace(field, string.Format("{0}.{1}", table, field));
            }
            return sb.ToString();
        }

        #endregion

        #region selects and table joins

        public static string ToSqlJoin(this JoinMode mode)
        {
            switch (mode)
            {
                case JoinMode.INNER: return "INNER JOIN";
                case JoinMode.OUTER: return "OUTER JOIN";
                case JoinMode.LEFT: return "LEFT JOIN";
                case JoinMode.RIGHT: return "RIGHT JOIN";
                case JoinMode.FULL: return "FULL JOIN";
                case JoinMode.RIGHT | JoinMode.OUTER: return "RIGHT OUTER JOIN";
                case JoinMode.LEFT | JoinMode.OUTER: return "RIGHT OUTER JOIN";
                default: return "JOIN";
            }

        }

        public static List<string> SelectParam<TModel>(this IDbConnection db,
    Expression<Func<TModel, string>> param,
    int? skip = null,
    int? rows = null)
        {
            return db.SelectParam<TModel, string>(param, skip, rows);
        }

        public static List<TParam> SelectParam<TModel, TParam>(this IDbConnection db,
            Expression<Func<TModel, TParam>> param,
            int? skip = null,
            int? rows = null)
        {

            var sb = new StringBuilder();
            var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<TModel>();

            var tname = GetQuotedTableName<TModel>();
            var pname = (param.Body != null) ? (param.Body as MemberExpression).Member.Name : null;
            if (pname == null) return new List<TParam>();

            ev.Skip = skip;
            ev.Rows = rows;
            sb.AppendFormat("SELECT {0}.{1} FROM {0}", tname, ToQuotedFieldname<TModel>(pname), tname)
                .AppendFormat(" {0} ", ev.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ReadToList<TParam>();
            }
        }

        public static List<string> SelectParam<TModel>(this IDbConnection db,
            Expression<Func<TModel, string>> param,
            Expression<Func<TModel, bool>> predicate,
            int? skip = null,
            int? rows = null)
        {
            return db.SelectParam<TModel, string>(param, predicate, skip, rows);
        }

        public static List<TParam> SelectParam<TModel, TParam>(this IDbConnection db,
            Expression<Func<TModel, TParam>> param,
            Expression<Func<TModel, bool>> predicate,
            int? skip = null,
            int? rows = null)
        {

            var sb = new StringBuilder();
            var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<TModel>();

            var tname = GetQuotedTableName<TModel>();
            var pname = (param.Body != null) ? (param.Body as MemberExpression).Member.Name : null;
            if (pname == null) return new List<TParam>();

            ev.Skip = skip;
            ev.Rows = rows;
            sb.AppendFormat("SELECT {0}.{1} FROM {0}", tname, ToQuotedFieldname<TModel>(pname), tname)
                .AppendFormat(" {0} ", ev.Where(predicate).WhereExpression)
                .Append(ev.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ReadToList<TParam>();
            }
        }


        #region Group I: Select from 1 table (extended)

        /// <summary>
        /// Retrieves records of a POCO table.
        /// </summary>
        /// <typeparam name="T1">The table whose records are retrieved</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1>(this IDbConnection db, int? skip, int? rows)
        {
            var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            return db.Exec(cmd =>
            {
                ev.Skip = skip;
                ev.Rows = rows;
                using (var reader = cmd.ExecReader(ev.ToSelectStatement()))
                    return reader.ConvertToList<T1>();
            });
        }

        /// <summary>
        /// Retrieves records of a POCO table using 1 predicate.
        /// </summary>
        /// <typeparam name="T1">The table whose records are retrieved</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1>(this IDbConnection db, Expression<Func<T1, bool>> P1, int? skip, int? rows)
        {
            var ev = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            return db.Exec(cmd =>
            {
                ev.Skip = skip;
                ev.Rows = rows;
                using (var reader = cmd.ExecReader(ev.Where(P1).ToSelectStatement()))
                    return reader.ConvertToList<T1>();
            });
        }

        #endregion

        #region Group II: Joining 2 tables

        #region  Join Scenario: 2 tables (T1, T2), 1 relation (R12) and 1 predicate (P1)

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 1 predicate (from first table) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12>(this IDbConnection db,

            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, string>> R12_FK2,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, R12, string>(R12_FK1, P1, R12_FK2, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 1 predicate (from second table) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1">The  predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K>> R12_FK2,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, R12, K, K>(R12_FK1, P1, R12_FK2, mode, quoted, skip, rows);
        }


        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 1 predicate (from first table) and 2 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12, K1, K2>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K2>> R12_FK2,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;

            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();

            if (r12_fk1 == null || r12_fk2 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk2, t2, t2_pk)
                .Append(ev1.Where(P1).WhereExpression.PrependTableNameToColumns<T1>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }
        #endregion

        #region  Join Scenario: 2 tables (T1, T2), 1 relation (R12) and 1 predicate (P2)

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 1 predicate (from second table) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2"> The predicate from the second POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, R12, string>(R12_FK1, R12_FK2, P2, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 1 predicate (from second table) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, R12, K, K>(R12_FK1, R12_FK2, P2, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational tables, 1 predicate (from second table) and 2 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2"> The predicate from the second POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12, K1, K2>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;

            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();

            if (r12_fk1 == null || r12_fk2 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .Append(ev2.Where(P2).WhereExpression.PrependTableNameToColumns<T2>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }
        #endregion

        #region Join Scenario: 2 Tables (T1, T2), 1 relation (R12) and 2 predicates (P1, P2)

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 2 predicates (from first and second tables) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, R12, string>(R12_FK1, P1, R12_FK2, P2, C1, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 2 predicates (from first and second tables) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, R12, K, K>(R12_FK1, P1, R12_FK2, P2, C1, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records of a POCO model from 2 tables using 1 relational table, 2 predicates (from first and second tables) and 2 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first and the second POCO tables.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, R12, K1, K2>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();

            if (r12_fk1 == null || r12_fk2 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .Append(ev1.Where(P1).WhereExpression.PrependTableNameToColumns<T1>())
                .AppendFormat(" {0} ", C1.ToString())
                .Append(ev2.Where(P2).WhereExpression.Replace("WHERE", string.Empty).PrependTableNameToColumns<T2>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }

        #endregion

        #endregion

        #region Group III: Joining 3 tables (T1, T2, T3)

        #region  Join Scenario: 3 tables (T1, T2, T3) (T1, T2, T3), 2 relations (R12, R13) and 1 predicate (P1)

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from first table) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<R13, string>> R13_FK1,
            Expression<Func<R13, string>> R13_FK3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, string>(R12_FK1, P1, R12_FK2, R13_FK1, R13_FK3, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from first table) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1">The predicate from the second POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<R13, K>> R13_FK1,
            Expression<Func<R13, K>> R13_FK3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, K, K, K>(R12_FK1, P1, R12_FK2, R13_FK1, R13_FK3, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from first table) and 3 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <typeparam name="K3">The type of keys used in the thirdPOCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K1, K2, K3>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<R13, K1>> R13_FK1,
            Expression<Func<R13, K3>> R13_FK3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where K3 : IComparable<K3>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();
            var ev3 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T3>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var t3 = (quoted) ? GetQuotedTableName<T3>() : GetTableName<T3>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();
            var r13 = (quoted) ? GetQuotedTableName<R13>() : GetTableName<R13>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;
            var r13_fk1 = (R13_FK1.Body != null) ? (R13_FK1.Body as MemberExpression).Member.Name : null;
            var r13_fk3 = (R13_FK3.Body != null) ? (R13_FK3.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;
            r13_fk1 = (quoted) ? ToQuotedFieldname<R13>(r13_fk1) : r13_fk1;
            r13_fk3 = (quoted) ? ToQuotedFieldname<R13>(r13_fk3) : r13_fk3;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();
            var t3_pk = (quoted) ? GetPrimaryKeyName<T3>() : GetQuotedPrimaryKeyName<T3>();

            if (r12_fk1 == null || r12_fk2 == null || r13_fk1 == null || r13_fk3 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r13, r13_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t3, t3_pk, r13, r13_fk3)
                .Append(ev1.Where(P1).WhereExpression.PrependTableNameToColumns<T1>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }
        #endregion

        #region  Join Scenario: 3 tables (T1, T2, T3) (T1, T2, T3), 2 relations (R12, R13) and 1 predicate (P2)

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from second table) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P2"> The predicate from the second POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, string>> R13_FK1,
            Expression<Func<R13, string>> R13_FK3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, string>(R12_FK1, R12_FK2, P2, R13_FK1, R13_FK3, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from second table) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, K>> R13_FK1,
            Expression<Func<R13, K>> R13_FK3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, K, K, K>(R12_FK1, R12_FK2, P2, R13_FK1, R13_FK3, mode, quoted, skip, rows);
        }


        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from second table) and 3 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <typeparam name="K3">The type of keys used in the thirdPOCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2"> The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K1, K2, K3>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, K1>> R13_FK1,
            Expression<Func<R13, K3>> R13_FK3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where K3 : IComparable<K3>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();
            var ev3 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T3>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var t3 = (quoted) ? GetQuotedTableName<T3>() : GetTableName<T3>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();
            var r13 = (quoted) ? GetQuotedTableName<R13>() : GetTableName<R13>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;
            var r13_fk1 = (R13_FK1.Body != null) ? (R13_FK1.Body as MemberExpression).Member.Name : null;
            var r13_fk3 = (R13_FK3.Body != null) ? (R13_FK3.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;
            r13_fk1 = (quoted) ? ToQuotedFieldname<R13>(r13_fk1) : r13_fk1;
            r13_fk3 = (quoted) ? ToQuotedFieldname<R13>(r13_fk3) : r13_fk3;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();
            var t3_pk = (quoted) ? GetPrimaryKeyName<T3>() : GetQuotedPrimaryKeyName<T3>();

            if (r12_fk1 == null || r12_fk2 == null || r13_fk1 == null || r13_fk3 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r13, r13_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t3, t3_pk, r13, r13_fk3)
                .Append(ev2.Where(P2).WhereExpression.PrependTableNameToColumns<T2>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }
        #endregion

        #region  Join Scenario: 3 tables (T1, T2, T3) (T1, T2, T3), 2 relations (R12, R13) and 1 predicate (P3)

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from third table) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3"> The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<R13, string>> R13_FK1,
            Expression<Func<R13, string>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, string>(R12_FK1, R12_FK2, R13_FK1, R13_FK3, P3, mode, quoted, skip, rows);
        }


        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from third table) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<R13, K>> R13_FK1,
            Expression<Func<R13, K>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, K, K, K>(R12_FK1, R12_FK2, R13_FK1, R13_FK3, P3, mode, quoted, skip, rows);
        }


        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 1 predicate (from third table) and 3 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <typeparam name="K3">The type of keys used in the thirdPOCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3"> The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K1, K2, K3>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<R13, K1>> R13_FK1,
            Expression<Func<R13, K3>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where K3 : IComparable<K3>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();
            var ev3 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T3>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var t3 = (quoted) ? GetQuotedTableName<T3>() : GetTableName<T3>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();
            var r13 = (quoted) ? GetQuotedTableName<R13>() : GetTableName<R13>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;
            var r13_fk1 = (R13_FK1.Body != null) ? (R13_FK1.Body as MemberExpression).Member.Name : null;
            var r13_fk3 = (R13_FK3.Body != null) ? (R13_FK3.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;
            r13_fk1 = (quoted) ? ToQuotedFieldname<R13>(r13_fk1) : r13_fk1;
            r13_fk3 = (quoted) ? ToQuotedFieldname<R13>(r13_fk3) : r13_fk3;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();
            var t3_pk = (quoted) ? GetPrimaryKeyName<T3>() : GetQuotedPrimaryKeyName<T3>();

            if (r12_fk1 == null || r12_fk2 == null || r13_fk1 == null || r13_fk3 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r13, r13_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t3, t3_pk, r13, r13_fk3)
                .Append(ev3.Where(P3).WhereExpression.PrependTableNameToColumns<T3>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }

        #endregion

        #region  Join Scenario: 3 tables (T1, T2, T3) (T1, T2, T3), 2 relations (R12, R13) and 2 predicates (P1, P2)

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from first and second tables) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, string>> R13_FK1,
            Expression<Func<R13, string>> R13_FK3,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, string>(R12_FK1, P1, R12_FK2, P2, R13_FK1, R13_FK3, C1, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from first and second tables) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, K>> R13_FK1,
            Expression<Func<R13, K>> R13_FK3,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, K, K, K>(R12_FK1, P1, R12_FK2, P2, R13_FK1, R13_FK3, C1, mode, quoted, skip, rows);
        }


        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from first and second tables) and 3 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <typeparam name="K3">The type of keys used in the thirdPOCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K1, K2, K3>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, K1>> R13_FK1,
            Expression<Func<R13, K3>> R13_FK3,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where K3 : IComparable<K3>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();
            var ev3 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T3>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var t3 = (quoted) ? GetQuotedTableName<T3>() : GetTableName<T3>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();
            var r13 = (quoted) ? GetQuotedTableName<R13>() : GetTableName<R13>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;
            var r13_fk1 = (R13_FK1.Body != null) ? (R13_FK1.Body as MemberExpression).Member.Name : null;
            var r13_fk3 = (R13_FK3.Body != null) ? (R13_FK3.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;
            r13_fk1 = (quoted) ? ToQuotedFieldname<R13>(r13_fk1) : r13_fk1;
            r13_fk3 = (quoted) ? ToQuotedFieldname<R13>(r13_fk3) : r13_fk3;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();
            var t3_pk = (quoted) ? GetPrimaryKeyName<T3>() : GetQuotedPrimaryKeyName<T3>();

            if (r12_fk1 == null || r12_fk2 == null || r13_fk1 == null || r13_fk3 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r13, r13_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t3, t3_pk, r13, r13_fk3)
                .Append(ev1.Where(P1).WhereExpression.PrependTableNameToColumns<T1>())
                .AppendFormat(" {0} ", C1.ToString())
                .Append(ev2.Where(P2).WhereExpression.Replace("WHERE", string.Empty).PrependTableNameToColumns<T2>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }
        #endregion

        #region  Join Scenario: 3 tables (T1, T2, T3) (T1, T2, T3), 2 relations (R12, R13) and 2 predicates (P2, P3)

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from second and third tables) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2"> The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, string>> R13_FK1,
            Expression<Func<R13, string>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, string>(R12_FK1, R12_FK2, P2, R13_FK1, R13_FK3, P3, C1, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from second and third tables) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2"> The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the second and the third predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, K>> R13_FK1,
            Expression<Func<R13, K>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, K, K, K>(R12_FK1, R12_FK2, P2, R13_FK1, R13_FK3, P3, C1, mode, quoted, skip, rows);
        }


        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from second and third tables) and 3 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <typeparam name="K3">The type of keys used in the thirdPOCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P2"> The predicate from the second POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the second and third predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K1, K2, K3>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, K1>> R13_FK1,
            Expression<Func<R13, K3>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where K3 : IComparable<K3>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();
            var ev3 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T3>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var t3 = (quoted) ? GetQuotedTableName<T3>() : GetTableName<T3>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();
            var r13 = (quoted) ? GetQuotedTableName<R13>() : GetTableName<R13>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;
            var r13_fk1 = (R13_FK1.Body != null) ? (R13_FK1.Body as MemberExpression).Member.Name : null;
            var r13_fk3 = (R13_FK3.Body != null) ? (R13_FK3.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;
            r13_fk1 = (quoted) ? ToQuotedFieldname<R13>(r13_fk1) : r13_fk1;
            r13_fk3 = (quoted) ? ToQuotedFieldname<R13>(r13_fk3) : r13_fk3;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();
            var t3_pk = (quoted) ? GetPrimaryKeyName<T3>() : GetQuotedPrimaryKeyName<T3>();

            if (r12_fk1 == null || r12_fk2 == null || r13_fk1 == null || r13_fk3 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r13, r13_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t3, t3_pk, r13, r13_fk3)
                .Append(ev2.Where(P2).WhereExpression.PrependTableNameToColumns<T2>())
                .AppendFormat(" {0} ", C1.ToString())
                .Append(ev3.Where(P3).WhereExpression.Replace("WHERE", string.Empty).PrependTableNameToColumns<T3>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }
        #endregion

        #region  Join Scenario: 3 tables (T1, T2, T3) (T1, T2, T3), 2 relations (R12, R13) and 2 predicates (P1, P3)

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from first and third tables) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<R13, string>> R13_FK1,
            Expression<Func<R13, string>> R13_FK3,
            Expression<Func<T1, bool>> P1,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, string>(R12_FK1, P1, R12_FK2, R13_FK1, R13_FK3, P3, C1, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from first and third tables) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<R13, K>> R13_FK1,
            Expression<Func<R13, K>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, K, K, K>(R12_FK1, P1, R12_FK2, R13_FK1, R13_FK3, P3, C1, mode, quoted, skip, rows);
        }


        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 2 predicates (from first and third tables) and 3 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <typeparam name="K3">The type of keys used in the thirdPOCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the third predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K1, K2, K3>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<R13, K1>> R13_FK1,
            Expression<Func<R13, K3>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where K3 : IComparable<K3>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();
            var ev3 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T3>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var t3 = (quoted) ? GetQuotedTableName<T3>() : GetTableName<T3>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();
            var r13 = (quoted) ? GetQuotedTableName<R13>() : GetTableName<R13>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;
            var r13_fk1 = (R13_FK1.Body != null) ? (R13_FK1.Body as MemberExpression).Member.Name : null;
            var r13_fk3 = (R13_FK3.Body != null) ? (R13_FK3.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;
            r13_fk1 = (quoted) ? ToQuotedFieldname<R13>(r13_fk1) : r13_fk1;
            r13_fk3 = (quoted) ? ToQuotedFieldname<R13>(r13_fk3) : r13_fk3;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();
            var t3_pk = (quoted) ? GetPrimaryKeyName<T3>() : GetQuotedPrimaryKeyName<T3>();

            if (r12_fk1 == null || r12_fk2 == null || r13_fk1 == null || r13_fk3 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r13, r13_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t3, t3_pk, r13, r13_fk3)
                .Append(ev1.Where(P1).WhereExpression.PrependTableNameToColumns<T1>())
                .AppendFormat(" {0} ", C1.ToString())
                .Append(ev3.Where(P3).WhereExpression.Replace("WHERE", string.Empty).PrependTableNameToColumns<T3>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }

        #endregion

        #region Join Scenario: 3 Tables (T1, T2, T3) and 2 relations (R12, R13), 3 predicates (P1, P2, P3)

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 3 predicates (from first, second and third tables) and string-based keys.
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="C2">The conjunction between the second and the third predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13>(this IDbConnection db,
            Expression<Func<R12, string>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, string>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, string>> R13_FK1,
            Expression<Func<R13, string>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1 = Conjunctor.AND,
            Conjunctor C2 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, string, string, string>(R12_FK1, P1, R12_FK2, P2, R13_FK1, R13_FK3, P3, C1, C2, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 3 predicates (from first, second and third tables) and a key type for all tables
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K">The type of keys used in all POCO and associated relational tables.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="C2">The conjunction between the second and the third predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return.</param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K>(this IDbConnection db,
            Expression<Func<R12, K>> R12_FK1,
            Expression<Func<R12, K>> R12_FK2,
            Expression<Func<R13, K>> R13_FK1,
            Expression<Func<R13, K>> R13_FK3,
            Expression<Func<T1, bool>> P1,
            Expression<Func<T2, bool>> P2,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1 = Conjunctor.AND,
            Conjunctor C2 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K : IComparable<K>, IComparable
            where T1 : new()
        {
            return db.Select<T1, T2, T3, R12, R13, K, K, K>(R12_FK1, P1, R12_FK2, P2, R13_FK1, R13_FK3, P3, C1, C2, mode, quoted, skip, rows);
        }

        /// <summary>
        /// Combines records from 3 POCO tables (T1, T2, T3) by the relationship (T1, R12, T2) and (T1, R13, T3) using 2 relational tables (R12, R13), 3 predicates (from first, second and third tables) and 3 key types
        /// </summary>
        /// <typeparam name="T1">The first table in the JOIN relationship. It is also the POCO base for the combined records</typeparam>
        /// <typeparam name="T2">The second table in the JOIN relationship.</typeparam>
        /// <typeparam name="T3">The third table in the JOIN relationship.</typeparam>
        /// <typeparam name="R12">The relation that links the first table and the second POCO table.</typeparam>
        /// <typeparam name="R13">The relation that links the first table and the third POCO table.</typeparam>
        /// <typeparam name="K1">The type of keys used in the first POCO table and associated relational table.</typeparam>
        /// <typeparam name="K2">The type of keys used in the second POCO table and associated relational table.</typeparam>
        /// <typeparam name="K3">The type of keys used in the thirdPOCO table and associated relational table.</typeparam>
        /// <param name="db">The database connection used for the JOIN operation</param>
        /// <param name="R12_FK1">The foreign key of the first relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="P1"> The predicate from the first POCO table.</param>
        /// <param name="R12_FK2">The foreign key of the first relational table that refers to the primary key of the second POCO table.</param>
        /// <param name="P2">The predicate from the second POCO table.</param>
        /// <param name="R13_FK1">The foreign key of the third  relational table that refers to the primary key of the first POCO table.</param>
        /// <param name="R13_FK3">The foreign key of the third  relational table that refers to the primary key of the third POCO table.</param>
        /// <param name="P3">The predicate from the third POCO table.</param>
        /// <param name="C1">The conjunction between the first and the second predicate</param>
        /// <param name="C2">The conjunction between the second and the third predicate</param>
        /// <param name="mode">The type of SQL JOIN used</param>
        /// <param name="quoted">Should the table and column names be set in quotes?</param>
        /// <param name="skip">The row offset of records to return. </param>
        /// <param name="rows">The number of row records to return.</param>
        /// <returns>The list of comined records of the first POCO table.</returns>
        public static List<T1> Select<T1, T2, T3, R12, R13, K1, K2, K3>(this IDbConnection db,
            Expression<Func<R12, K1>> R12_FK1,
            Expression<Func<T1, bool>> P1,
            Expression<Func<R12, K2>> R12_FK2,
            Expression<Func<T2, bool>> P2,
            Expression<Func<R13, K1>> R13_FK1,
            Expression<Func<R13, K3>> R13_FK3,
            Expression<Func<T3, bool>> P3,
            Conjunctor C1 = Conjunctor.AND,
            Conjunctor C2 = Conjunctor.AND,
            JoinMode mode = JoinMode.INNER,
            bool quoted = true,
            int? skip = null,
            int? rows = null)
            where K1 : IComparable<K1>, IComparable
            where K2 : IComparable<K2>, IComparable
            where K3 : IComparable<K3>, IComparable
            where T1 : new()
        {
            var sb = new StringBuilder();
            var ev1 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T1>();
            var ev2 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T2>();
            var ev3 = OrmLiteConfig.DialectProvider.ExpressionVisitor<T3>();

            var t1 = (quoted) ? GetQuotedTableName<T1>() : GetTableName<T1>();
            var t2 = (quoted) ? GetQuotedTableName<T2>() : GetTableName<T2>();
            var t3 = (quoted) ? GetQuotedTableName<T3>() : GetTableName<T3>();
            var r12 = (quoted) ? GetQuotedTableName<R12>() : GetTableName<R12>();
            var r13 = (quoted) ? GetQuotedTableName<R13>() : GetTableName<R13>();

            var r12_fk1 = (R12_FK1.Body != null) ? (R12_FK1.Body as MemberExpression).Member.Name : null;
            var r12_fk2 = (R12_FK2.Body != null) ? (R12_FK2.Body as MemberExpression).Member.Name : null;
            var r13_fk1 = (R13_FK1.Body != null) ? (R13_FK1.Body as MemberExpression).Member.Name : null;
            var r13_fk3 = (R13_FK3.Body != null) ? (R13_FK3.Body as MemberExpression).Member.Name : null;

            r12_fk1 = (quoted) ? ToQuotedFieldname<R12>(r12_fk1) : r12_fk1;
            r12_fk2 = (quoted) ? ToQuotedFieldname<R12>(r12_fk2) : r12_fk2;
            r13_fk1 = (quoted) ? ToQuotedFieldname<R13>(r13_fk1) : r13_fk1;
            r13_fk3 = (quoted) ? ToQuotedFieldname<R13>(r13_fk3) : r13_fk3;


            var t1_pk = (quoted) ? GetPrimaryKeyName<T1>() : GetQuotedPrimaryKeyName<T1>();
            var t2_pk = (quoted) ? GetPrimaryKeyName<T2>() : GetQuotedPrimaryKeyName<T2>();
            var t3_pk = (quoted) ? GetPrimaryKeyName<T3>() : GetQuotedPrimaryKeyName<T3>();

            if (r12_fk1 == null || r12_fk2 == null || r13_fk1 == null || r13_fk3 == null) return new List<T1>();

            ev1.Skip = (skip != null) ? (int?)skip.Value : null;
            ev1.Rows = (rows != null) ? (int?)rows.Value : null;
            sb.AppendFormat("SELECT {0}.* FROM {0} ", t1)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r12, r12_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t2, t2_pk, r12, r12_fk2)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), r13, r13_fk1, t1, t1_pk)
                .AppendFormat("{0} {1} ON {1}.{2} = {3}.{4} ", mode.ToSqlJoin(), t3, t3_pk, r13, r13_fk3)
                .Append(ev1.Where(P1).WhereExpression.PrependTableNameToColumns<T1>())
                .AppendFormat(" {0} ", C1.ToString())
                .Append(ev2.Where(P2).WhereExpression.Replace("WHERE", string.Empty).PrependTableNameToColumns<T2>())
                .AppendFormat(" {0} ", C2.ToString())
                .Append(ev3.Where(P3).WhereExpression.Replace("WHERE", string.Empty).PrependTableNameToColumns<T3>())
                .Append(" ").Append(ev1.LimitExpression);

            using (var reader = db.Exec(cmd => cmd.ExecReader(sb.ToString())))
            {
                return reader.ConvertToList<T1>();
            }
        }

        #endregion

        #endregion

        #endregion 

        #region SaveAll operation without implicit transaction commit

        public static void Save<T>(this IDbConnection db, T entity, IDbTransaction transaction)
        {
            db.Exec(cmd => cmd.Save(entity, transaction));
        }

        public static void SaveAll<T>(this IDbConnection db, IEnumerable<T> entities, IDbTransaction transaction)
        {
            db.Exec(cmd => cmd.SaveAll(entities, transaction));
        }

        #endregion

    }
}