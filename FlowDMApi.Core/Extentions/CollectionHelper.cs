using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace FlowDMApi.Core.Extentions
{
    public static class CollectionHelper
    {
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    var value = prop.GetValue(item);
                    if (value == null)
                    {
                        row[prop.Name] = DBNull.Value;
                    }
                    else
                    {
                        row[prop.Name] = value;
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static IList<T> ToList<T>(this IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static IList<T> ToList<T>(this DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            var rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ToList<T>(rows);
        }




        public static DataRow ConvertToDataRow<T>(T entity) where T : class
        {
            DataTable table = CreateTable<T>();

            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
            {
                var value = prop.GetValue(entity);
                if (value == null)
                {
                    row[prop.Name] = DBNull.Value;
                }
                else
                {
                    row[prop.Name] = value;
                }
            }

            return row;
        }

        private static T CreateItem<T>(this DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, (value == DBNull.Value ? null : value), null);
                    }
                    catch
                    {
                        // You can log something here
                        throw;
                    }
                }
            }

            return obj;
        }

        private static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            
            foreach (PropertyDescriptor prop in properties)
            {
                // The the type of the property
                Type columnType = prop.PropertyType;

                // We need to check whether the property is NULLABLE
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                    columnType = prop.PropertyType.GetGenericArguments()[0];
                }

                // Add the column definition to the datatable.
                table.Columns.Add(prop.Name, columnType);
            }

            return table;
        }
    }

}
