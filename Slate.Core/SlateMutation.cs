using System;
using System.Reactive.Subjects;

namespace Slate.Core
{
    public abstract class SlateMutation : ISlate, IDisposable
    {
        protected readonly ISlate _source;
        protected Subject<Update> _updates;
        private readonly IDisposable _disposable;

        public SlateMutation(ISlate source)
        {
            _source = source;
            _updates = new Subject<Update>();
            _disposable = source.Updates.Subscribe(HandleUpdate);
        }

        public virtual Point Size => _source.Size;

        public virtual Point ScrollableSize => _source.ScrollableSize;

        public virtual IObservable<Update> Updates => _updates;

        public virtual Cell GetCell(Point at)
        {
            return _source.GetCell(at);
        }

        public virtual IDisposable Lock()
        {
            return _source.Lock();
        }

        public virtual void SetScrollOffset(Point offset)
        {
            _source.SetScrollOffset(offset);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        protected virtual void HandleUpdate(Update update)
        {
            _updates.OnNext(update);
        }
    }
}