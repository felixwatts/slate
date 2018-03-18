using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slate.Core.Controls.DataGrid
{
    public static class ColumnFactory
    {
        public static IEnumerable<IColumn<TRow>> FromProperties<TRow>()
        {
            return typeof(TRow)
                .GetProperties(BindingFlags.Public)
                .Select(p => FromProperty<TRow>(p.Name))
                .ToArray();

        }

        public static IColumn<TRow> FromProperty<TRow>(string propertyName, uint? color = null)
        {
            var property = typeof(TRow).GetProperty(propertyName);

            if(property.PropertyType == typeof(int))
                return new IntPropertyColumn<TRow>(propertyName, null, false, null, null);
            else if(property.PropertyType == typeof(string))
                return new StringPropertyColumn<TRow>(propertyName);
            else return new DefaultPropertyColumn<TRow>(propertyName);

        }
    }
}