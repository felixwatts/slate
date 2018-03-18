using System;

namespace Slate.Core.Controls.DataGrid
{
    public class DefaultPropertyColumn<TRow> : PropertyColumn<TRow>
    {
        public DefaultPropertyColumn(
            string propertyName, 
            string header = null, 
            bool isFixed = false, 
            uint? color = null, 
            TextAlignment alignment = TextAlignment.Right) 
            : base(propertyName, header, isFixed, color ?? Color.White, alignment)
        {
        }

        public override bool CanSort => false;

        public override int Compare(TRow r1, TRow r2) => throw new NotSupportedException();        

        protected override bool TryParseValue(string str, out object val)
        {
            val = null;
            return false;
        }

        protected override string ValueToString(object val)
        {
            return val?.ToString() ?? string.Empty;
        }
    }
}