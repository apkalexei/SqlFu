﻿using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    
    public static class CrudHelpers
    {

        public static InsertedId Insert<T>(this DbConnection db, T data, Action<IInsertableOptions<T>> cfg = null)
        {
            var info = db.GetPocoInfo<T>();
            var options = info.CreateInsertOptions<T>();
            cfg?.Invoke(options);

            var provider = db.GetProvider();
            var builder=new InsertSqlBuilder(info,data,provider,options);

            return db.GetValue<InsertedId>(builder.GetCommandConfiguration());
        }

        public static Task<InsertedId> InsertAsync<T>(this DbConnection db, T data,CancellationToken cancel ,Action<IInsertableOptions<T>> cfg = null)
        {
            var info = db.GetPocoInfo<T>();
            var options = info.CreateInsertOptions<T>();
            cfg?.Invoke(options);

            var provider = db.GetProvider();
            var builder=new InsertSqlBuilder(info,data,provider,options);

            return db.GetValueAsync<InsertedId>(builder.GetCommandConfiguration(), cancel);
        }
        static Insertable<T> CreateInsertOptions<T>(this TableInfo info)
        {
            return new Insertable<T>()
            {
                DbSchema = info.Table.Schema,
                TableName = info.Table.Name,
                IdentityColumn = info.IdentityColumn
            };
        }

        public static IBuildUpdateTable<T> Update<T>(this DbConnection db,Action<IHelperOptions> cfg=null)
        {
            var opt = new HelperOptions();
            cfg?.Invoke(opt);
            return new UpdateBuilder<T>(db,opt);
        }

        /// <summary>
        /// Perform update table with data from an anonymous object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="cfg">Configure name and other</param>
        /// <param name="columns">Select which columns to update from an anonymous object</param>
        /// <returns></returns>
        public static IBuildUpdateTableFrom<T> UpdateFrom<T>(this DbConnection db, Action<IHelperOptions> cfg, Func<IUpdateColumns, IColumnsToUpdate<T>> columns) where  T:class 
        {
            var options=new HelperOptions(); 
            var u = new UpdateColumns();
            cfg(options);
            var builder = columns(u) as UpdateColumns.CreateBuilder<T>;
            var updater=new UpdateBuilder<T>(db,options);
            builder.PopulateBuilder(updater);
            return updater;
        }


        public static int DeleteFrom<T>(this DbConnection db,Expression<Func<T, bool>> criteria=null)
        {
            var builder=new DeleteBuilder<T>(db);
            if (criteria!=null) builder.WriteCriteria(criteria);
            return db.Execute(builder.GetCommandConfiguration());
        }

        public static Task<int> DeleteFromAsync<T>(this DbConnection db,CancellationToken token,Expression<Func<T, bool>> criteria=null)
        {
            var builder=new DeleteBuilder<T>(db);
            if (criteria!=null) builder.WriteCriteria(criteria);
            return db.ExecuteAsync(builder.GetCommandConfiguration(),token);
        }
      
    }
}