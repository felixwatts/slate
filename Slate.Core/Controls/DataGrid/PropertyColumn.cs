using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;

namespace Slate.Core.Controls.DataGrid
{
    public class IntPropertyColumn<TRow> : PropertyColumn<TRow>
    {
        private string _formatString;

        protected IntPropertyColumn(
            string propertyName, string header, 
            bool isFixed, 
            uint color, 
            TextAlignment alignment,
            string formatString) 
            : base(propertyName, header, isFixed, color, alignment)
        {
            _formatString = formatString;
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

    public abstract class PropertyColumn<TRow> : IColumn<TRow>
    {
        protected readonly PropertyInfo _property;
        private readonly Subject<TRow> _updates;
        private readonly Cell _header;
        private readonly uint _color;
        private readonly TextAlignment _alignment;

        protected PropertyColumn(string propertyName, string header, bool isFixed, uint color, TextAlignment alignment)
        {
            _property = typeof(TRow).GetProperty(propertyName);
            _updates = new Subject<TRow>();
            _header = new Cell(header.ToUpper(), Color.Black, TextAlignment.Center);
            _color = color;
            _alignment = alignment;
            IsFixed = isFixed;
        }

        public bool IsFixed { get; }

        public abstract bool CanSort { get; }

        public IObservable<TRow> Updates => _updates;

        public abstract int Compare(TRow r1, TRow r2);

        public Cell GetCell(TRow row)
        {
            var val = _property.GetValue(row);
            var text = ValueToString(val);
            return new Cell(text, _color, _alignment);
        }

        public Cell GetHeader() => _header;

        public void UserInput(IEnumerable<TRow> rows, string input)
        {
            if(!_property.CanWrite) return;

            if(!TryParseValue(input, out var val)) return;

            foreach(var row in rows)
            {
                _property.SetValue(row, val);
            }
        }

        protected abstract string ValueToString(object val);
        protected abstract bool TryParseValue(string str, out object val);
    }
}