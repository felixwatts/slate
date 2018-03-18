namespace Slate.Core
{

    public class Update
    {
        public UpdateType Type { get; }
        public Region Region { get; }

        private Update(UpdateType type, Region region)
        {
            Type = type;
            Region = region;
        }

        public static Update RegionDirty(Region region)
        {
            return new Update(UpdateType.RegionDirty, region);
        }

        public static readonly Update BeginBulkUpdate = new Update(UpdateType.BeginBulkUpdate, null);
        public static readonly Update EndBulkUpdate = new Update(UpdateType.EndBulkUpdate, null);
        public static readonly Update SizeChanged = new Update(UpdateType.SizeChanged, null);
        public static readonly Update ScrollableSizeChanged = new Update(UpdateType.ScrollableSizeChanged, null);

    }
}