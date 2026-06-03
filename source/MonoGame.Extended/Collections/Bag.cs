using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private int _count;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Bag{T}"/>.
        /// </summary>
        /// <value>The number of elements in the <see cref="Bag{T}"/>.</value>
        public int Count => _count;

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
                if (value < _count)
                    throw new ArgumentOutOfRangeException("Capacity was less than the current size.");

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (_count > 0)
                        {
                            Array.Copy(_items, newItems, _count);
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
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");
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
            EnsureCapacity(_count + 1);
            _items[_count] = element;
            _count++;
        }

        /// <summary>
        /// Adds all elements from another <see cref="Bag{T}"/> to this <see cref="Bag{T}"/>.
        /// </summary>
        /// <param name="range">The bag containing elements to add.</param>
        public void AddRange(Bag<T> range)
        {
            _version++;
            for (int index = 0, j = range._count; j > index; ++index)
                Add(range[index]);
        }

        /// <summary>
        /// Removes all elements from the <see cref="Bag{T}"/>.
        /// </summary>
        public void Clear()
        {
            if(_count == 0)
                return;

            // need to free for the GC
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                Array.Clear(_items, 0, _count);

            _version++;
            _count = 0;
        }

        /// <summary>
        /// Determines whether the <see cref="Bag{T}"/> contains a specific element.
        /// </summary>
        /// <param name="element">The element to locate in the <see cref="Bag{T}"/>.</param>
        /// <returns>true if the element is found in the <see cref="Bag{T}"/>; otherwise, false.</returns>
        public bool Contains(T element)
        {
            return _count != 0 && IndexOf(element) > 0;
        }

        /// <summary>
        /// Removes and returns the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <returns>The element that was removed from the <see cref="Bag{T}"/>.</returns>
        public T RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");

            var result = _items[index];
            _count--;
            _version++;
            if (index == _count)
            {
                _items[index] = default(T);
                return result;
            }
            _items[index] = _items[_count];
            _items[_count] = default(T);
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
            _count--;
            _version++;
            if (index == _count)
            {
                _items[index] = default(T);
                return true;
            }
            _items[index] = _items[_count];
            _items[_count] = default(T);
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
                var countainAny = _count != 0;
                Clear();
                return countainAny;
            }

            var isResult = false;

            foreach (var element in bag)
            {
                if (Remove(element))
                    isResult = true;
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
            if (_count < threshold)
                Capacity = _count;
        }

        /// <summary>
        /// Copies all elements of the <see cref="Bag{T}"/> to the specified array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="Bag{T}"/>.</param>
        /// <exception cref="ArgumentException">Thrown when the target array type is not compatible with the type of items in the collection.</exception>
        public void CopyTo(T[] array)
            => CopyTo(array, 0);

        /// <summary>
        /// Copies all elements of the <see cref="Bag{T}"/> to the specified array, starting at the specified index in the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="Bag{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        /// <exception cref="ArgumentException">Thrown when the target array is multidimensional or the target array type is not compatible with the type of items in the collection.</exception>
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array != null && array.Rank != 1)
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");

            try
            {
                Array.Copy(_items, 0, array, arrayIndex, _count);
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
            if (_count - index < count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            Array.Copy(_items, index, array, arrayIndex, count);
        }

        /// <summary>
        /// Copies all elements of the <see cref="Bag{T}"/> to the specified array, starting at the specified index in the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="Bag{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in the target array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the <see cref="Bag{T}"/>.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="Bag{T}"/>.</param>
        /// <returns>The zero-based index of the first occurrence of the element, or -1 if the element is not found.</returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _count);
        }

        void IList<T>.Insert(int index, T item)
        {
            if (index > _count)
            {
                throw new ArgumentOutOfRangeException("Index must be within the bounds of the bag, any value under Count (include) and over 0 (include).");
            }
            else if (index < _count)
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
                return;

            var newCapacity = Math.Max((int)(_items.Length * 1.5), capacity);
            var oldElements = _items;
            _items = new T[newCapacity];
            Array.Copy(oldElements, 0, _items, 0, oldElements.Length);
            List<int> ints = new List<int>();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Get the <see cref="Enumerator"/> for this <see cref="Bag{T}"/>. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Use this method preferentially over <see cref="IEnumerable.GetEnumerator"/> while enumerating via foreach
        /// to avoid boxing the enumerator on every iteration, which can be expensive in high-performance environments.
        /// </remarks>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return _count - 1;
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
                return Contains((T)value);
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
                return IndexOf((T)value);
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            ((IList<T>)this).Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
                Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        private static bool IsCompatibleObject(object value)
        {
            return value is T || (value == null && default(T) == null);
        }

        /// <summary>
        /// Enumerates the elements of <see cref="Bag{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly Bag<T> _bag;
            private readonly int _version;

            private int _index;
            private T _current;

            internal Enumerator(Bag<T> bag)
            {
                _bag = bag;
                _version = bag._version;
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                Bag<T> localBag = _bag;

                if (_version != _bag._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                
                if ((uint)_index < (uint)localBag._count)
                {
                    _current = localBag._items[_index];
                    _index++;
                    return true;
                }

                _current = default;
                return false;
            }

            /// <inheritdoc/>
            public T Current => _current;

            object IEnumerator.Current => _current;

            void IDisposable.Dispose() { }

            void IEnumerator.Reset() => throw new NotImplementedException();
        }
    }
}
