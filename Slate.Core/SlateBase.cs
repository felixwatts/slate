using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Slate.Core
{
    public abstract class SlateBase : ISlate
    {
        protected Subject<Update> updates;
        private readonly object modifyLock;

        public virtual Point Size { get; protected set; }

        public virtual Point ScrollableSize => Size;

        public IObservable<Update> Updates => updates;

        protected SlateBase()
        {
            modifyLock = new object();
            Size = Point.Zero;
            updates = new Subject<Update>();
        }

        public abstract Cell GetCell(Point at);

        public IDisposable Lock()
        {
            System.Threading.Monitor.Enter(modifyLock);
            return Disposable.Create(() => System.Threading.Monitor.Exit(modifyLock));
        }

        public virtual void SetScrollOffset(Point offset)
        {
            // no op
        }
    }
}