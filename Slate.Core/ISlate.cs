using System;

namespace Slate.Core
{
    public interface ISlate
    {
        #region Read Access - if called from UI thread, should be called within a using(Lock()){ ... }
        
        Point Size { get; }
        Point ScrollableSize { get; }        
        Cell GetCell(Point at);

        #endregion

        #region Write Access - if called from UI thread, should be called within a using(Lock()){ ... }

        void SetScrollOffset(Point offset);

        #endregion

        #region Update Notification

        IObservable<Update> Updates { get; }

        #endregion

        IDisposable Lock();
    }
}