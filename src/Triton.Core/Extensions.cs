using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Triton.Core
{
    public static class Extensions
    {
        public static DataTable ToDataTableFromList<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
            prop.PropertyType) ?? prop.PropertyType);

            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static DataTable ToDataTableFromObject<T>(this T data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name,
            Nullable.GetUnderlyingType(
            prop.PropertyType) ?? prop.PropertyType);

            }
            object[] values = new object[props.Count];
            for (int i = 0; i < values.Length; i++)
            {
                    values[i] = props[i].GetValue(data);
            }
            table.Rows.Add(values);
            return table;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> self, string tableName, bool? removeFirstColumn)
        {
            var properties = typeof(T).GetProperties();

            var dataTable = new DataTable();
            foreach (var info in properties)
                dataTable.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.PropertyType)
                                                 ?? info.PropertyType);

            foreach (var entity in self)
                dataTable.Rows.Add(properties.Select(p => p.GetValue(entity)).ToArray());

            if (removeFirstColumn == true)
                dataTable.Columns.RemoveAt(0);

            dataTable.TableName = tableName;
            return dataTable;
        }

        public static DataTable ToDataTableFromList<T>(this IList<T> data, bool removeFirstColumn)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            if (removeFirstColumn == true)
                table.Columns.RemoveAt(0);
            return table;
        }

        public static IEnumerable<T> FromSingleItem<T>(T item)
        {
            yield return item;
        }

        public static DataTable ToDataTable<T>(T item, string tableName, bool? removeFirstColumn)
        {
            return ToDataTable(FromSingleItem(item), tableName, removeFirstColumn);
        }

        // for generic interface IEnumerable<T>
        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            string[] array = source.Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }

        // for interface IEnumerable
        public static string ToString(this IEnumerable source, string separator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Parameter separator can not be null or empty.");

            string[] array = source.Cast<object>().Where(n => n != null).Select(n => n.ToString()).ToArray();

            return string.Join(separator, array);
        }

         public static string ToAgeString(this DateTime dob)
        {
            DateTime today = DateTime.Today;

            int months = today.Month - dob.Month;
            int years = today.Year - dob.Year;

            if (today.Day < dob.Day)
            {
                months--;
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            int days = (today - dob.AddMonths((years * 12) + months)).Days;

            return string.Format("{0} year{1}, {2} month{3} and {4} day{5}",
                                 years, (years == 1) ? "" : "s",
                                 months, (months == 1) ? "" : "s",
                                 days, (days == 1) ? "" : "s");
        }
    }



}
