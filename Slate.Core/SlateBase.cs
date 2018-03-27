using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Slate.Core
{
    public abstract class SlateBase : ISlate
    {
        protected Subject<Update> _updates;
        private readonly object _modifyLock;

        public virtual Point Size { get; protected set; }

        public virtual Point ScrollableSize => Size;

        public IObservable<Update> Updates => _updates;

        protected SlateBase()
        {
            _modifyLock = new object();
            Size = Point.Zero;
            _updates = new Subject<Update>();
        }

        public abstract Cell GetCell(Point at);

        public IDisposable Lock()
        {
            System.Threading.Monitor.Enter(_modifyLock);
            return Disposable.Create(() => System.Threading.Monitor.Exit(_modifyLock));
        }

        public virtual void SetScrollOffset(Point offset)
        {
            // no op
        }

        public virtual void SetVisibleRegions(Region[] visibleRegions)
        {
            // no op
        }

        public virtual void KeyDown(Key key, ModifierKeys modifierKeys)
        {
            // no op
        }

        public virtual void KeyUp(Key key, ModifierKeys modifierKeys)
        {
            // no op
        }

        public virtual void MouseDown(Point cell, MouseButton button, ModifierKeys modifierKeys)
        {
            // no op
        }

        public virtual void MouseUp(Point cell, MouseButton button, ModifierKeys modifierKeys)
        {
            // no op
        }

        public virtual void MouseMove(Point cell, ModifierKeys modifierKeys)
        {   
            // no op         
        }
    }
}