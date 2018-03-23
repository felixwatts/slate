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
        void SetVisibleRegions(Region[] visibleRegions);

        void KeyDown(Key key, ModifierKeys modifierKeys);
        void KeyUp(Key key, ModifierKeys modifierKeys);
        void MouseDown(Point cell, MouseButton button, ModifierKeys modifierKeys);
        void MouseUp(Point cell, MouseButton button, ModifierKeys modifierKeys);        
        void MouseMove(Point cell, ModifierKeys modifierKeys);

        #endregion

        #region Update Notification

        IObservable<Update> Updates { get; }

        #endregion

        IDisposable Lock();
    }
}