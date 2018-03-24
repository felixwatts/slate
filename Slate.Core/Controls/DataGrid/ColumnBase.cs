using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Slate.Core.Controls.DataGrid
{
    public abstract class ColumnBase<TRow> : IColumn<TRow>
    {
        protected Subject<TRow> _updates;

        public virtual bool IsFixed => false;

        public virtual bool CanSort => false;

        public IObservable<TRow> Updates => _updates;

        protected ColumnBase()
        {
            _updates = new Subject<TRow>();
        }

        public virtual void ActivateRow(TRow row)
        {
            // no op
        }

        public virtual int Compare(TRow r1, TRow r2)
        {
            throw new NotSupportedException();
        }

        public virtual void DeactivateRow(TRow row)
        {
            // no op
        }

        public abstract Cell GetCell(TRow row);

        public abstract Cell GetHeader();

        public virtual void MouseDown(TRow row, MouseButton button, ModifierKeys modifierKeys)
        {
            // no op
        }

        public virtual void MouseUp(TRow row, MouseButton button, ModifierKeys modifierKeys)
        {
            // no op
        }

        public virtual void UserInput(IEnumerable<TRow> rows, string input)
        {
            // no op
        }
    }
}