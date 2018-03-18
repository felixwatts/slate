using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace Slate.Core
{
    internal static class UtilityExtensions
    {
        public static IDisposable ToDisposeAll(this IEnumerable<IDisposable> disposables)
        {
            return Disposable.Create(() =>
            {
                foreach(var d in disposables)
                    d?.Dispose();
            });
        }
    }
}