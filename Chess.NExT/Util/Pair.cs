using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Chess.Util
{
    public class Pair<T> : IList<T>, ITuple
    {
        private List<T> values;
        
        public Pair(T first, T second)
        {
            values = new List<T> {first, second};
        }

        public T first
        {
            get { return this[0]; }

            set { this[0] = value; }
        }
        
        public T second
        {
            get { return this[1]; }

            set { this[1] = value; }
        }

        public void ForEach(Action<T> action)
        {
            values.ForEach(action);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            values.Clear();
        }

        public bool Contains(T item)
        {
            return values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            values.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return values.Remove(item);
        }

        public int Count
        {
            get { return values.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public int IndexOf(T item)
        {
            return values.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if ((index < 0) || (index > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Pairs have two indices: 0 and 1");
            }
            
            values.Insert(index, item);
        }
        
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Pairs have two indices: 0 and 1");
            }
            
            values.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                if ((index < 0) || (index > 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Pairs have two indices: 0 and 1");
                }

                return values[index];
            }


            set
            {
                if ((index < 0) || (index > 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Pairs have two indices: 0 and 1");
                }

                values[index] = value;
            }
        }

        public int Length
        {
            get { return 2; }
        }

        object ITuple.this[int index]
        {
            get { return this[index]; }
        }
    }
}