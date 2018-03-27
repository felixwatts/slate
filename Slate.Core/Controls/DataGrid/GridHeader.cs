namespace Slate.Core.Controls.DataGrid
{
    public class GridHeader<TRow> : SlateBase
    {
        private readonly IColumn<TRow>[] _columns;
        private readonly string[] _filterStrings;

        public GridHeader(IColumn<TRow>[] columns)
        {
            _columns = columns;
            _filterStrings = new string[_columns.Length];
            Size = new Point(_columns.Length, 1);
        }

        public override Cell GetCell(Point at)
        {
            if(at.X < 0 || at.X >= Size.X || at.Y != 0) return null;

            return new Cell(_filterStrings[at.X] ?? string.Empty, Color.White);
        }
    }
}