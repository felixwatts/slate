using System;
using System.Collections.Generic;

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

        void ActivateRow(TRow row);
        void DeactivateRow(TRow row);

        void MouseDown(TRow row, MouseButton button, ModifierKeys modifierKeys);
        void MouseUp(TRow row, MouseButton button, ModifierKeys modifierKeys);

        IObservable<TRow> Updates { get; }
    }
}