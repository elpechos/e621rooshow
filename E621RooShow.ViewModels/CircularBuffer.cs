using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E621RooShow.ViewModels
{
    public class CircularBuffer<T> : IList<T>
    {
        private int tail { get; set; }
        private int head { get; set; }

        public int Tail
        {
            get
            {
                return this.tail;
            }
        }

        private T[] buffer;
        public CircularBuffer(int size)
        {
            buffer = new T[size];
        }
        public int Count
        {
            get
            {
                if (head >= tail)
                    return head - tail;
                else
                    return (buffer.Length + head - tail);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T Current
        {
            get
            {
                return buffer[Tail];
            }
        }

        public void MoveNext()
        {
            if (Count == 0 || Count == 1) //No items left, can't move next
                return;

            tail += 1;
            tail = tail % buffer.Length;
        }

        public void MovePrevious()
        {
            var newTail = Tail - 1;
            if (newTail < 0)
                newTail = buffer.Length - 1;

            if (newTail == head) //If the new tail would step us back onto head that's bad
                return;

            if (buffer[newTail] == null)
                return;

            tail = newTail;
        }

        public T this[int index]
        {
            get
            {
                return buffer[index % buffer.Length];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            buffer[head] = item;
            head += 1;
            head = head % buffer.Length;
        }

        public void Clear()
        {
            head = 0;
            tail = 0;
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
