namespace Slate.Core.Controls.DataGrid
{
    public class StringPropertyColumn<TRow> : PropertyColumn<TRow>
    {
        public StringPropertyColumn(
            string propertyName, 
            string header = null, 
            bool isFixed = false, 
            uint? color = null, 
            TextAlignment alignment = TextAlignment.Left) 
            : base(propertyName, header, isFixed, color ?? Color.White, alignment)
        {
        }

        public override bool CanSort => true;

        public override int Compare(TRow r1, TRow r2)
        {
            var s1 = Get<string>(r1);
            var s2 = Get<string>(r2);

            return string.CompareOrdinal(s1, s2);
        }

        protected override bool TryParseValue(string str, out object val)
        {
            val = str;
            return true;
        }

        protected override string ValueToString(object val)
        {
            return (string)val;
        }
    }
}