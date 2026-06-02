// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Utils/Bag.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bag.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without modification, are
//     permitted provided that the following conditions are met:
//
//        1. Redistributions of source code must retain the above copyright notice, this list of
//           conditions and the following disclaimer.
//
//        2. Redistributions in binary form must reproduce the above copyright notice, this list
//           of conditions and the following disclaimer in the documentation and/or other materials
//           provided with the distribution.
//
//     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
//     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
//     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     The views and conclusions contained in the software and documentation are those of the
//     authors and should not be interpreted as representing official policies, either expressed
//     or implied, of GAMADU.COM.
// </copyright>
// <summary>
//   Class Bag.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    /// <summary>
    /// A dynamic collection that provides efficient storage and retrieval of elements with automatic capacity management.
    /// The Bag collection grows dynamically as elements are added and maintains a compact representation
    /// by reusing slots when elements are removed.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the bag.</typeparam>
    public class Bag<T>
        : IEnumerable<T>,
          ICollection<T>,
          IList<T>,
          IReadOnlyCollection<T>,
          IReadOnlyList<T>,
          ICollection,
          IList
    {
        private T[] _items;
        private int _version;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Bag{T}"/> is empty.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="Bag{T}"/> contains no elements; otherwise, <see langword="false"/>.</value>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Bag{T}"/>.
        /// </summary>
        /// <value>The number of elements in the <see cref="Bag{T}"/>.</value>
        public int Count { get; private set; }

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without allocating more space.
        /// </summary>
        /// <value>The capacity of the <see cref="Bag{T}"/>.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the capacity is set to a value less than the current count of elements.</exception>
        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException("capacity was less than the current size.");
                
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (Count > 0)
                        {
                            Array.Copy(_items, newItems, Count);
                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = Array.Empty<T>();
                    }
                }
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        bool IList.IsFixedSize => true;

        bool IList.IsReadOnly => false;

        object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bag{T}"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the <see cref="Bag{T}"/>. Defaults to 16.</param>
        public Bag(int capacity = 16)
        {
            _items = new T[capacity];
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index, or the default value if the index is out of current range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value at an index greater than or equal to the current count.</exception>
        public T this[int index]
        {
            get => index >= _items.Length ? default(T) : _items[index];
            set
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");
                }

                EnsureCapacity(index + 1);
                _items[index] = value;
                _version++;
            }
        }

        /// <summary>
        /// Adds an element to the end of the <see cref="Bag{T}"/>.
        /// </summary>
        /// <param name="element">The element to add to the <see cref="Bag{T}"/>.</param>
        public void Add(T element)
        {
            _version++;
            EnsureCapacity(Count + 1);
            _items[Count] = element;
            ++Count;
        }

        /// <summary>
        /// Adds all elements from another <see cref="Bag{T}"/> to this <see cref="Bag{T}"/>.
        /// </summary>
        /// <param name="range">The bag containing elements to add.</param>
        public void AddRange(Bag<T> range)
        {
            _version++;
            for (int index = 0, j = range.Count; j > index; ++index)
            {
                Add(range[index]);
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="Bag{T}"/>.
        /// </summary>
        public void Clear()
        {
            if (Count == 0)
            {
                return;
            }
            // non-primitive types are cleared so the garbage collector can release them
            if (!typeof(T).IsPrimitive)
            {
                Array.Clear(_items, 0, Count);
            }

            _version++;
            Count = 0;
        }

        /// <summary>
        /// Determines whether the <see cref="Bag{T}"/> contains a specific element.
        /// </summary>
        /// <param name="element">The element to locate in the <see cref="Bag{T}"/>.</param>
        /// <returns>true if the element is found in the <see cref="Bag{T}"/>; otherwise, false.</returns>
        public bool Contains(T element)
        {
            return Count != 0 && IndexOf(element) > 0;
        }

        /// <summary>
        /// Removes and returns the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <returns>The element that was removed from the <see cref="Bag{T}"/>.</returns>
        public T RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");

            var result = _items[index];
            Count--;
            _version++;
            if (index == Count)
            {
                _items[index] = default(T);
                return result;
            }
            _items[index] = _items[Count];
            _items[Count] = default(T);
            return result;
        }

        void IList<T>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        /// <summary>
        /// Removes the first occurrence of the specified element from the <see cref="Bag{T}"/>.
        /// </summary>
        /// <param name="element">The element to remove from the <see cref="Bag{T}"/>.</param>
        /// <returns>true if the element was successfully removed; false if the element was not found.</returns>
        public bool Remove(T element)
        {
            var index = IndexOf(element);
            if (index < 0)
                return false;
            Count--;
            _version++;
            if (index == Count)
            {
                _items[index] = default(T);
                return true;
            }
            _items[index] = _items[Count];
            _items[Count] = default(T);
            return true;
        }

        /// <summary>
        /// Removes all elements from the <see cref="Bag{T}"/> that are contained in the specified <see cref="Bag{T}"/>.
        /// </summary>
        /// <param name="bag">The <see cref="Bag{T}"/> containing elements to remove.</param>
        /// <returns>true if at least one element was removed; false if no elements were removed.</returns>
        public bool RemoveAll(Bag<T> bag)
        {
            if (bag == this)
            {
                var countainAny = !IsEmpty;
                Clear();
                return countainAny;
            }    

            var isResult = false;
            var count = bag.Count;
            for (var index = bag.Count - 1; index >= 0; --index)
            {
                if (Remove(bag[index]))
                {
                    isResult = true;
                }
            }

            _version++;
            return isResult;
        }

        /// <summary>
        /// Removes excess capacity from the <see cref="Bag{T}"/>, reducing memory usage if the <see cref="Bag{T}"/> is using less than 90% of its capacity.
        /// </summary>
        public void TrimExcess()
        {
            int threshold = (int)(_items.Length * 0.9);
            if (Count < threshold)
            {
                Capacity = Count;
            }
        }

        /// <summary>
        /// Copies all elements of the <see cref="Bag{T}"/> to the specified array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="Bag{T}"/>.</param>
        /// <exception cref="ArgumentException">Thrown when the target array type is not compatible with the type of items in the collection.</exception>
        public void CopyTo(T[] array)
            => CopyTo(array, 0);

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            // The summary of ICollection.CopyTo do not mention ArrayTypeMismatchException, RankException. 
            // We catch and throw ArgumentException
            if (array != null && array.Rank != 1)
            {
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
            }

            try
            {
                Array.Copy(_items, 0, array, arrayIndex, Count);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
            }
        }

        /// <summary>
        /// Copies a range of elements from the <see cref="Bag{T}"/> to the specified array, starting at the specified index in the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the <see cref="Bag{T}"/> at which copying begins.</param>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="Bag{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in the target array at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">Thrown when the offset and length are out of bounds for the array or count is greater than the number of elements from index to the end of the collection.</exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (Count - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            Array.Copy(_items, index, array, arrayIndex, count);
        }

        /// <summary>
        /// Copies all elements of the <see cref="Bag{T}"/> to the specified array, starting at the specified index in the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="Bag{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in the target array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, Count);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the <see cref="Bag{T}"/>.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="Bag{T}"/>.</param>
        /// <returns>The zero-based index of the first occurrence of the element, or -1 if the element is not found.</returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, Count);
        }

        void IList<T>.Insert(int index, T item)
        {
            if (index > Count)
            {
                throw new ArgumentOutOfRangeException("Index must be within the bounds of the bag, any value under Count (include) and over 0 (include).");
            }
            else if (index < Count)
            {
                Add(_items[index]);
                _items[index] = item;
            }
            else
            {
                Add(item);
            }
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity < _items.Length)
            {
                return;
            }

            var newCapacity = Math.Max((int)(_items.Length * 1.5), capacity);
            var oldElements = _items;
            _items = new T[newCapacity];
            Array.Copy(oldElements, 0, _items, 0, oldElements.Length);
            List<int> ints = new List<int>();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new BagEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BagEnumerator(this);
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return Contains((T)value);
            }
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((T)value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            ((IList<T>)this).Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        private static bool IsCompatibleObject(object value)
        {
            return value is T || (value == null && default(T) == null);
        }

        internal struct BagEnumerator : IEnumerator<T>
        {
            private volatile Bag<T> _bag;
            private volatile int _index;
            private int _version;

            public BagEnumerator(Bag<T> bag)
            {
                _version = bag._version;
                _bag = bag;
                _index = -1;
            }

            T IEnumerator<T>.Current => _bag[_index];
            object IEnumerator.Current => _bag[_index];

            public bool MoveNext()
            {
                if (_version != _bag._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                return ++_index < _bag.Count;
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}
