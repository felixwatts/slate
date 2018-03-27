using System;
using System.Linq;

namespace Slate.Core.Behaviours
{
    public class ScrollBehaviour : SlateMutation
    {        
        private readonly int _numFixedRows, _numFixedColumns;
        private Point _scrollOffset;

        public override Point Size => _source.Size - _scrollOffset;
        public override Point ScrollableSize => _source.Size - new Point(_numFixedColumns, _numFixedRows);

        public ScrollBehaviour(ISlate source, int numFixedRows, int numFixedColumns) : base(source)
        {
            _numFixedRows = numFixedRows;
            _numFixedColumns = numFixedColumns;
            _scrollOffset = Point.Zero;
        }

        public override Cell GetCell(Point at)
        {
            return _source.GetCell(ToSource(at));
        }

        public override void SetScrollOffset(Point offset)
        {
            _scrollOffset = offset;   

            _updates.OnNext(Update.SizeChanged);
            _updates.OnNext(Update.RegionDirty(Region.FromBottomRight(Size)));       

            RefreshSourceVisibleRegions();
        }

        public override void KeyDown(Key key, ModifierKeys modifierKeys)
        {
            Console.WriteLine(key);

            if(modifierKeys.HasFlag(ModifierKeys.Ctrl))
            {
                switch(key)
                {
                    case Key.Up:
                        if(_scrollOffset.Y > 0)
                        {
                            SetScrollOffset(_scrollOffset + new Point(0, -1));
                            return;
                        }
                        break;
                    case Key.Down:
                        if(_scrollOffset.Y < _source.ScrollableSize.Y-1)
                        {
                            SetScrollOffset(_scrollOffset + new Point(0, 1));
                            return;
                        }
                        break;
                    case Key.Left:
                        if(_scrollOffset.X > 0)
                        {
                            SetScrollOffset(_scrollOffset + new Point(-1, 0));
                            return;
                        }
                        break;
                    case Key.Right:
                        if(_scrollOffset.Y < _source.ScrollableSize.X-1)
                        {
                            SetScrollOffset(_scrollOffset + new Point(0, 1));
                            return;
                        }
                        break;
                }
            }

            base.KeyDown(key, modifierKeys);
        }   

        public override void MouseDown(Point cell, MouseButton button, ModifierKeys modifierKeys)
        {
            _source.MouseDown(ToSource(cell), button, modifierKeys);
        }     

        public override void MouseUp(Point cell, MouseButton button, ModifierKeys modifierKeys)
        {
            _source.MouseUp(ToSource(cell), button, modifierKeys);
        } 

        private Region[] _sinkVisibleRegions;

        public override void SetVisibleRegions(Region[] visibleRegions)
        {
            _sinkVisibleRegions = visibleRegions;
            RefreshSourceVisibleRegions();
        }

        private void RefreshSourceVisibleRegions()
        {
            if(_numFixedRows == 0 && _numFixedColumns == 0) 
            {
                base.SetVisibleRegions(_sinkVisibleRegions);
                return;
            }

            var maxX = _sinkVisibleRegions.Max(r => r.BottomRight.X + _scrollOffset.X);
            var maxY = _sinkVisibleRegions.Max(r => r.BottomRight.Y + _scrollOffset.Y);

            var fixedBothRegion = new Region(Point.Zero, new Point(_numFixedColumns, _numFixedRows));
            var fixedColumnRegion = new Region(new Point(0, _numFixedRows + _scrollOffset.Y), new Point(_numFixedColumns, maxY));
            var fixedRowRegion = new Region(new Point(_numFixedColumns + _scrollOffset.X, 0), new Point(maxX, _numFixedRows));
            var dataRegion = new Region(new Point(_numFixedColumns + _scrollOffset.X, _numFixedRows + _scrollOffset.Y), new Point(maxX, maxY));

            var sourceRegions = _sinkVisibleRegions.SelectMany(SplitRegion).Where(r => !r.IsEmpty).ToArray();
            _source.SetVisibleRegions(sourceRegions);

            Region[] SplitRegion(Region r)
            {
                return new Region[]
                {
                    r.IntersectionWith(fixedBothRegion),
                    r.IntersectionWith(fixedColumnRegion),
                    r.IntersectionWith(fixedRowRegion),
                    r.IntersectionWith(dataRegion),
                };
            }
        }

        protected override void HandleUpdate(Update update)
        {
            switch(update.Type)
            {
                case UpdateType.RegionDirty:
                    _updates.OnNext(
                        Update.RegionDirty(
                            new Region(
                                ToSink(update.Region.TopLeft), 
                                ToSink(update.Region.BottomRight))));
                    break;
                default: 
                    base.HandleUpdate(update); 
                    break;
            }
        }

        private Point ToSource(Point sinkPoint)
        {
            return new Point(
                ExpandRange(sinkPoint.X, _numFixedColumns, _scrollOffset.X),
                ExpandRange(sinkPoint.Y, _numFixedRows, _scrollOffset.Y));
        }

        private Point ToSink(Point sourcePoint)
        {
            return new Point(
                CollapseRange(sourcePoint.X, _numFixedColumns, _scrollOffset.X),
                CollapseRange(sourcePoint.Y, _numFixedRows, _scrollOffset.Y));
        }

        private int CollapseRange(int val, int rangeStart, int rangeLength)
        {
            if(val < rangeStart) return val;
            if(val < rangeStart + rangeLength) return rangeStart;
            return val - rangeLength;
        }

        private int ExpandRange(int val, int rangeStart, int rangeLength)
        {
            if(val < rangeStart) return val;
            return val + rangeLength;
        }
    }
}