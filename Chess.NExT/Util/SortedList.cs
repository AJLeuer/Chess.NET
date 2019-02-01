using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Utility
{
    public class SortedList<T> : List<T> where T : IComparable
    {
        public SortedList() :
            base()
        {
            Sort();
        }
        
        public SortedList(IEnumerable<T> collection) :
            base(collection)
        {
            Sort();
        }

        public new virtual T this[int index]
        {
            get { return base[index];}

            set
            {
                base[index] = value;
                Sort();
            }
        }

        public virtual void Add(T item, bool sort = true)
        {
            base.Add(item);
            
            if (sort) {
                Sort();
            }
        }

        public new virtual void Insert(int index, T item)
        {
            base.Insert(index, item);
            Sort();
        }
        
        public new virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            Sort();
        }

        public new virtual bool Remove(T item)
        {
            bool result = base.Remove(item);
            Sort();
            return result;
        }
        
        public new virtual void RemoveAt(int index)
        {
            base.RemoveAt(index);
            Sort();
        }

    }

    public class CustomSortedList<T> : List<T>
    {
        protected readonly Comparison<T> Comparator;
        
        public CustomSortedList(Comparison<T> comparator) :
            base()
        {
            this.Comparator = comparator;
            Sort(comparator);
        }
        
        public CustomSortedList(Comparison<T> comparator, IReadOnlyCollection<T> collection) :
            base(collection)
        {
            this.Comparator = comparator;
            Sort(comparator);
        }
        
        public CustomSortedList(Comparison<T> comparator, IEnumerable<T> collection) :
            base(collection)
        {
            this.Comparator = comparator;
            Sort(comparator);
        }
        
        public new virtual T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
                Sort(Comparator);
            }
        }

        public new virtual void Add(T item)
        {
            base.Add(item);
            Sort(Comparator);
        }

        public new virtual void Insert(int index, T item)
        {
            base.Insert(index, item);
            Sort(Comparator);
        }

        public new virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            Sort(Comparator);
        }

        public new virtual bool Remove(T item)
        {
            bool result = base.Remove(item);
            Sort(Comparator);
            return result;
        }

        public new virtual void RemoveAt(int index)
        {
            base.RemoveAt(index);
            Sort(Comparator);
        }
    }

    public class CustomSortedQueue<T> : CustomSortedList<T>
    {
        public CustomSortedQueue(Comparison<T> comparator) :
            base(comparator)
        {
        }
        
        public CustomSortedQueue(Comparison<T> comparator, IEnumerable<T> collection) :
            base(comparator, collection)
        {
        }

        public virtual void Enqueue(T item)
        {
            Add(item);
        }

        public virtual T Dequeue()
        {
            T val = this.First();
            RemoveAt(0);
            return val;
        }

        public virtual T Peek()
        {
            return this.First();
        }
    }
}