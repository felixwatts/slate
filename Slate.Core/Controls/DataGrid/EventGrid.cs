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
        private readonly IColumn<TEvent>[] _columns;
        private readonly IDisposable _disposable;
        private readonly EventCollection _content;

        public EventGrid(IObservable<TEvent> source, IEnumerable<IColumn<TEvent>> columns)
        {
            _content = new EventCollection();

            _columns = columns.Where(c => c.IsFixed).Union(columns.Where(c => !c.IsFixed)).ToArray();

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

            var row = _content[at.Y];
            var column = _columns[at.X];
            var cell = column.GetCell(row);

            return cell;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void HandleRowUpdate(int columnIndex, TEvent row)
        {
            var rowIndex = _content.IndexOf(row);
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