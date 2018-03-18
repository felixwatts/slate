using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;

namespace Slate.Core.Controls.DataGrid
{    
    public interface IColumn<TRow>
    {
        Cell GetHeader();
        Cell GetCell(TRow row);

        bool IsFixed { get; }

        bool CanSort { get; }
        int Compare(TRow r1, TRow r2);

        void UserInput(IEnumerable<TRow> rows, string input);

        IObservable<TRow> Updates { get; }
    }

    public abstract class PropertyColumn<TRow> : IColumn<TRow>
    {
        private readonly PropertyInfo _property;
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