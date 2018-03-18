namespace Slate.Core.Controls.DataGrid
{
    public class IntPropertyColumn<TRow> : PropertyColumn<TRow>
    {
        private string _formatString;

        public IntPropertyColumn(
            string propertyName, 
            string header = null, 
            bool isFixed = false, 
            uint? color = null, 
            string formatString = null) 
            : base(propertyName, header, isFixed, color ?? Color.White, TextAlignment.Right)
        {
            _formatString = formatString ?? "G";
        }

        public override bool CanSort => true;

        public override int Compare(TRow r1, TRow r2)
        {
            var i1 = (int)_property.GetValue(r1);
            var i2 = (int)_property.GetValue(r2);

            return i1.CompareTo(i2);
        }

        protected override bool TryParseValue(string str, out object val)
        {
            if(int.TryParse(str, out var i))
            {
                val = i;
                return true;
            }
            else
            {
                val = null;
                return false;
            }
        }

        protected override string ValueToString(object val)
        {
            return ((int)val).ToString(_formatString);
        }
    }
}