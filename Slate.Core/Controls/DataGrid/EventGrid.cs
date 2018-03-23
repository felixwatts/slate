using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Slate.Core;

namespace Slate.Core.Controls.DataGrid
{
    /// <summary>  
    /// A data grid for displaying a stream of events. Each new event is a new row at the top of the grid.
    /// Supports filtering but not sorting.
    /// </summary>  
    public class EventGrid<TEvent> : SlateBase, IDisposable
    {
        const int Y_HEADER = 0;
        const int Y_DATA = 1;

        private readonly IColumn<TEvent>[] _columns;
        private readonly IDisposable _disposable;
        private readonly EventCollection _content;
        
        private Region[] _visibleRegions;

        public override Point Size => new Point(_columns.Length, _content.Length + Y_DATA);

        public EventGrid(IObservable<TEvent> source, IEnumerable<IColumn<TEvent>> columns)
        {
            if(!columns.Any()) 
            {
                throw new Exception();
            }

            _content = new EventCollection();
            _columns = columns.Where(c => c.IsFixed).Union(columns.Where(c => !c.IsFixed)).ToArray();
            _visibleRegions = new Region[0];

            var disposables = new List<IDisposable>();
            
            for(int i = 0; i < _columns.Length; i++)
            {
                var column = _columns[i];
                var subscription = column.Updates.Subscribe(r => HandleRowUpdate(i, r));
                disposables.Add(subscription);
            }

            disposables.Add(source.Subscribe(HandleNewEvent));

            _disposable = disposables.ToDisposeAll();
        }        

        public override Cell GetCell(Point at)
        {
            if (at.X < 0 || at.Y < 0 || at.X >= Size.X || at.Y >= Size.Y) return null;

            if(at.Y == Y_HEADER) return _columns[at.X].GetHeader();

            var row = _content[at.Y - Y_DATA];
            var column = _columns[at.X];
            var cell = column.GetCell(row);

            return cell;
        }

        public override void SetVisibleRegions(Region[] visibleRegions)
        {
            Console.WriteLine($"Set visible regions: {string.Join(", ", visibleRegions.Select(r => r.ToString()))}");

            var newCells = visibleRegions.SelectMany(x => x);
            var oldCells = _visibleRegions.SelectMany(x => x);

            _visibleRegions = visibleRegions;

            var deactivatedCells = new HashSet<Point>(oldCells);
            deactivatedCells.ExceptWith(newCells);

            var activatedCells = new HashSet<Point>(newCells);
            deactivatedCells.ExceptWith(oldCells);

            foreach(var cell in deactivatedCells)
            {
                if (cell.X < 0 || cell.Y < 0 || cell.X >= Size.X || cell.Y >= Size.Y) continue;

                var row = _content[cell.Y - Y_DATA];
                var column = _columns[cell.X];

                column.DeactivateRow(row);
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void HandleRowUpdate(int columnIndex, TEvent row)
        {
            var rowIndex = _content.IndexOf(row)+1;
            var dirtyRegion = Region.FromCell(new Point(columnIndex, rowIndex));
            updates.OnNext(Update.RegionDirty(dirtyRegion));
        }

        private void HandleNewEvent(TEvent item)
        {
            using(Lock())
            {
                _content.AddFirst(item);
                Size = new Point(_columns.Length, _content.Length);                
            }

            foreach(var region in _visibleRegions)
            {
                var activatedRowIndex = Math.Max(region.TopLeft.Y-Y_DATA, 0);
                Console.WriteLine($"Activated row index: {activatedRowIndex}");            
                var activatedRow = _content[activatedRowIndex];
                for(var activatedCellX = Math.Max(0, region.TopLeft.X); activatedCellX < Math.Min(_columns.Length, region.BottomRight.X); activatedCellX++)
                    _columns[activatedCellX].ActivateRow(activatedRow);

                var deactivatedRowIndex = region.BottomRight.Y-Y_DATA;
                Console.WriteLine($"Deactivated row index: {deactivatedRowIndex}"); 
                if(deactivatedRowIndex < _content.Length)
                {                                       
                    var deactivatedRow = _content[deactivatedRowIndex];
                    for(var deactivatedCellX = Math.Max(0, region.TopLeft.X); deactivatedCellX < Math.Min(_columns.Length, region.BottomRight.X); deactivatedCellX++)
                        _columns[deactivatedCellX].DeactivateRow(deactivatedRow);
                }
            }

            updates.OnNext(Update.BeginBulkUpdate);
            updates.OnNext(Update.SizeChanged);
            updates.OnNext(Update.ScrollableSizeChanged);
            updates.OnNext(Update.RegionDirty(Region.FromBottomRight(Size)));
            updates.OnNext(Update.EndBulkUpdate);
        }

        private class EventCollection
        {
            private List<TEvent> _content;
            private Dictionary<TEvent, int> _indexByItem;

            public EventCollection()
            {
                _content = new List<TEvent>();
                _indexByItem = new Dictionary<TEvent, int>();
            }

            public void AddFirst(TEvent item)
            {
                _indexByItem.Add(item, _content.Count);
                _content.Add(item);                
            }

            public TEvent this[int i] => _content[MapIndex(i)];

            public int IndexOf(TEvent item) => MapIndex(_indexByItem[item]);
            public int Length => _content.Count;

            private int MapIndex(int i) => _content.Count - i - 1;
        }
    }
}