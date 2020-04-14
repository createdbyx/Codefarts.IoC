namespace Codefarts.IoC.Tests
{
    using System;

    public class MockDisposableRepository : MockRepository, IDisposable
    {
        public Action WasDisposed { get; set; }

        public void Dispose()
        {
            var callback = this.WasDisposed;
            if (callback != null)
            {
                callback();
            }
        }
    }
}