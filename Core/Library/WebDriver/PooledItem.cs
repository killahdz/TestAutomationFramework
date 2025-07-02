using System;

namespace Core.Library.WebDriver
{
    public class PooledItem<T> : IDisposable where T : IDisposable
    {
        private readonly Pool<T> _pool;

        public PooledItem(T item, Pool<T> pool)
        {
            Value = item;
            _pool = pool;
        }

        public T Value { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            _pool.ReturnItem(Value);
        }

        public static implicit operator T(PooledItem<T> item)
        {
            return item.Value;
        }
    }
}