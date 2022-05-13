//@BaseCode
//MdStart
using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonBase.ThreadSafe
{
    public partial class ThreadSafeList<T> : IList<T>
    {
        protected static object _lock = new object();
        protected List<T> internalList = new List<T>();

        public T this[int index]
        {
            get
            {
                var result = default(T);

                lock (_lock)
                {
                    result = internalList[index];
                }
                return result;
            }
            set
            {
                lock (_lock)
                {
                    internalList[index] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                var result = 0;

                lock (_lock)
                {
                    result = internalList.Count;
                }
                return result;
            }
        }

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            lock (_lock)
            {
                internalList.Add(item);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                internalList.Clear();
            }
        }

        public bool Contains(T item)
        {
            var result = false;

            lock (_lock)
            {
                result = internalList.Contains(item);
            }
            return result;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                internalList.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        public int IndexOf(T item)
        {
            var result = -1;

            lock (_lock)
            {
                result = internalList.IndexOf(item);
            }
            return result;
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                internalList.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            var result = false;

            lock (_lock)
            {
                result = internalList.Remove(item);
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                internalList.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        public List<T> Clone()
        {
            List<T> newList = new List<T>();

            lock (_lock)
            {
                internalList.ForEach(x => newList.Add(x));
            }
            return newList;
        }
    }
}
//MdEnd