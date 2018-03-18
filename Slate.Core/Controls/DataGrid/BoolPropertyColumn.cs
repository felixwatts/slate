namespace Slate.Core.Controls.DataGrid
{
    public class BoolPropertyColumn<TRow> : PropertyColumn<TRow>
    {
        protected BoolPropertyColumn(
            string propertyName, 
            string header, 
            bool isFixed, 
            uint color) 
            : base(propertyName, header, isFixed, color, TextAlignment.Center)
        {
        }

        public override bool CanSort => true;

        public override int Compare(TRow r1, TRow r2)
        {
            var b1 = Get<bool>(r1);
            var b2 = Get<bool>(r2);

            return b1.CompareTo(b2); 
        }

        protected override bool TryParseValue(string str, out object val)
        {
            val = null;
            return false;
        }

        protected override string ValueToString(object val)
        {
            return (bool)val ? "■" : "□";
        }
    }
}