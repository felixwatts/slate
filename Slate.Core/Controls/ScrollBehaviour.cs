namespace Slate.Core.Controls
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