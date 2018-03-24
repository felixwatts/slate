using System;
using System.Collections.Generic;
using System.Reflection;

namespace Slate.Core.Controls.DataGrid
{
    public class MethodColumn<TRow> : ColumnBase<TRow>
    {
        private static readonly Cell _header = new Cell(string.Empty, Color.Black);
        private readonly MethodInfo _method;
        private readonly Cell _cell;

        public MethodColumn(
            string methodName,
            uint color,
            bool isFixed = false,
            string header = null)
            {
                _method = typeof(TRow).GetMethod(methodName, new Type[0]);
                _cell = new Cell(header??methodName, color, TextAlignment.Center, true);
                IsFixed = isFixed;
            }

        public override bool IsFixed { get; }

        public override Cell GetCell(TRow row) => _cell;

        public override Cell GetHeader() => _header;

        public override void MouseUp(TRow row, MouseButton button, ModifierKeys modifierKeys)
        {
            _method.Invoke(row, new object[0]);
        }
    }
}