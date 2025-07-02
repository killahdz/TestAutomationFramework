using System;
using System.Collections.Generic;
using System.Linq;
using ThirdDrawer.Extensions.CollectionExtensionMethods;

namespace Core.Library.WebDriver
{
    public class Pool<T> : IDisposable where T : IDisposable
    {
        private readonly Func<T> _itemFactory;
        private readonly Action<T> _itemReset;
        private readonly Stack<T> _items = new Stack<T>();
        private readonly object _mutex = new object();

        public Pool(Func<T> itemFactory, Action<T> itemReset)
        {
            _itemFactory = itemFactory;
            _itemReset = itemReset;
        }

        public void Dispose()
        {
            _items
                .Do(item => item.Dispose())
                .Done();
        }

        public PooledItem<T> BorrowItem()
        {
            lock (_mutex)
            {
                if (_items.Any())
                {
                    var item = _items.Pop();
                    return new PooledItem<T>(item, this);
                }
            }

            return new PooledItem<T>(_itemFactory(), this);
        }

        public void ReturnItem(T item)
        {
            lock (_mutex)
            {
                _itemReset(item);
                _items.Push(item);
            }
        }

        public void Dispose(Action<T> itemDisposeAction)
        {
            _items
                .Do(item => itemDisposeAction(item))
                .Done();
        }
    }
}