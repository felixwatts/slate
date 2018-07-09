using System.Collections.Generic;
using System.Linq;

namespace Slate.Core.Controls.DataGrid
{
    /// <summary>  
    /// A data grid that supports sorting and filtering but does not support mutation of sorted fields.null
    /// If you need to mutate a sorted field you must remove the row, mutate the field and then re-add the row
    /// </summary>  
    public class DataGrid<TRowId, TRow> : SlateBase, IComparer<TRowId>
    {
        private readonly SortedList<TRowId, TRow> _rowsSorted;        
        private readonly Dictionary<TRowId, TRow> _rowById;
        private readonly IColumn<TRow>[] _columns;
        private int _sortColumnIndex;
        private bool _isSortAscending;

        public override Point Size => new Point(_columns.Length, _rowById.Count);        

        public DataGrid(IEnumerable<IColumn<TRow>> columns)
        {
            _rowsSorted = new SortedList<TRowId, TRow>(this);
            _rowById = new Dictionary<TRowId, TRow>();
            _columns = columns.ToArray();
            _sortColumnIndex = -1;
        }

        public void AddRow(TRowId id, TRow row)
        {
            _rowById.Add(id, row);
            _rowsSorted.Add(id, row);

            var addedRowIndex = _rowsSorted.IndexOfKey(id);

            _updates.OnNext(Update.BeginBulkUpdate);
            _updates.OnNext(Update.SizeChanged);
            _updates.OnNext(Update.ScrollableSizeChanged);
            _updates.OnNext(
                Update.RegionDirty(
                    new Region(
                        new Point(0, addedRowIndex), 
                        new Point(_columns.Length, _rowById.Count))));
            _updates.OnNext(Update.EndBulkUpdate);
        }

        public void RemoveRow(TRowId id)
        {
            var removedIndex = _rowsSorted.IndexOfKey(id);

            _rowsSorted.Remove(id);
            _rowById.Remove(id);

            _updates.OnNext(Update.BeginBulkUpdate);
            _updates.OnNext(Update.SizeChanged);
            _updates.OnNext(Update.ScrollableSizeChanged);
            _updates.OnNext(
                Update.RegionDirty(
                    new Region(
                        new Point(0, removedIndex), 
                        new Point(_columns.Length, _rowById.Count))));
            _updates.OnNext(Update.EndBulkUpdate);
        }

        public int Compare(TRowId id1, TRowId id2)
        {
            if(_sortColumnIndex < 0) 
            {
                return id1.GetHashCode().CompareTo(id2.GetHashCode());
            }

            var sortColumn = _columns[_sortColumnIndex];

            var row1 = _rowById[id1];
            var row2 = _rowById[id2];

            return sortColumn.Compare(row1, row2);    
        }

        public override Cell GetCell(Point at)
        {
            var x = at.X;
            if(x < 0 || x >= _columns.Length)
            {
                return null;
            }

            var y = at.Y;
            if(y < 0 || y >= _rowsSorted.Count)
            {
                return null;
            }

            var row = _rowsSorted.Values[y];
            var column = _columns[x];

            return column.GetCell(row);
        }
    }
}