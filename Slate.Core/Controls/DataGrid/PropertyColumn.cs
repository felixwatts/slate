using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reflection;

namespace Slate.Core.Controls.DataGrid
{

    public abstract class PropertyColumn<TRow> : IColumn<TRow>
    {
        private bool isTRowINotifyPropertyChanged;

        protected readonly PropertyInfo _property;
        private readonly Subject<TRow> _updates;
        private readonly Cell _header;
        protected readonly uint _color;
        protected readonly TextAlignment _alignment;

        protected PropertyColumn(
            string propertyName, 
            string header, 
            bool isFixed, 
            uint color, 
            TextAlignment alignment)
        {
            _property = typeof(TRow).GetProperty(propertyName);
            _updates = new Subject<TRow>();
            _header = new Cell($"{(header ?? _property.Name).ToUpper()} ▲▼", Color.Black, TextAlignment.Center, true);
            _color = color;
            _alignment = alignment;
            IsFixed = isFixed;
            isTRowINotifyPropertyChanged = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TRow));
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

        protected T Get<T>(TRow row)
        {
            return (T)_property.GetValue(row);
        }

        public void ActivateRow(TRow row)
        {
            _numActiveCells++;
            Console.WriteLine(_numActiveCells);

            if(isTRowINotifyPropertyChanged)
                (row as INotifyPropertyChanged).PropertyChanged += HandleRowPropertyChanged;
        }

        public void DeactivateRow(TRow row)
        {
            _numActiveCells--;
            Console.WriteLine(_numActiveCells);

            if(isTRowINotifyPropertyChanged)
                (row as INotifyPropertyChanged).PropertyChanged -= HandleRowPropertyChanged;
        }

        private static int _numActiveCells;

        private void HandleRowPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != _property.Name) return;
            _updates.OnNext((TRow)sender);
        }
    }
}