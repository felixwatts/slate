namespace Slate.Core.Controls.DataGrid
{
    public class GridFilterRow<TRow> : SlateBase
    {
        private IColumn<TRow>[] _columns;

        public GridFilterRow(IColumn<TRow>[] columns)
        {
            Size = new Point(_columns.Length, 1);
        }

        public override Cell GetCell(Point at)
        {
            if(at.X < 0 || at.X >= Size.X || at.Y != 0) return null;

            return _columns[at.X].GetHeader();
        }
    }
}