using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reflection;

namespace Slate.Core.Controls.DataGrid
{


    public abstract class PropertyColumn<TRow> : ColumnBase<TRow>
    {
        private HashSet<TRow> _activeRows = new HashSet<TRow>();

        private bool _isTRowINotifyPropertyChanged;

        protected readonly PropertyInfo _property;        
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
            _header = new Cell($"{(header ?? _property.Name).ToUpper()} ▲▼", Color.Black, TextAlignment.Center, true);
            _color = color;
            _alignment = alignment;
            IsFixed = isFixed;
            _isTRowINotifyPropertyChanged = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TRow));
        }

        public override bool IsFixed { get; }

        public override Cell GetCell(TRow row)
        {
            var val = _property.GetValue(row);
            var text = ValueToString(val);
            return new Cell(text, _activeRows.Contains(row) ? _color : Color.Black, _alignment);
        }

        public override Cell GetHeader() => _header;

        public override void UserInput(IEnumerable<TRow> rows, string input)
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

        public override void ActivateRow(TRow row)
        {
            if(_activeRows.Contains(row)) throw new Exception();
            _activeRows.Add(row);

            if(_isTRowINotifyPropertyChanged)
                (row as INotifyPropertyChanged).PropertyChanged += HandleRowPropertyChanged;
        }

        public override void DeactivateRow(TRow row)
        {   
            if(!_activeRows.Contains(row)) throw new Exception();
            _activeRows.Remove(row);

            if(_isTRowINotifyPropertyChanged)
                (row as INotifyPropertyChanged).PropertyChanged -= HandleRowPropertyChanged;
        }

        private void HandleRowPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != _property.Name) return;
            _updates.OnNext((TRow)sender);
        }
    }
}