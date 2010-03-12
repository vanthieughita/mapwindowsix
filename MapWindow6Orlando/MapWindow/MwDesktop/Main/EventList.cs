//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The core libraries for the MapWindow 6.0 project.
//
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/ 
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specificlanguage governing rights and 
// limitations under the License. 
//
// The Original Code is MapWindow.dll
//
// The Initial Developer of this Original Code is Ted Dunsford. Created in August, 2007.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MapWindow.Components;
using MapWindow.Main;
namespace MapWindow.Main
{
    /// <summary>
    /// A list that also includes several events during its existing activities.
    /// List is fussy about inheritance, unfortunately, so this wraps a list
    /// and then makes this class much more inheritable
    /// </summary>
    public class EventList<T> : IEventList<T>
    {
       
        

        #region variables
        
        /// <summary>
        /// The internal list of items
        /// </summary>
        private List<T> _list;

        /// <summary>
        /// Decides whether or not this list can be changed.  This characteristic
        /// can be set, so that once it becomes readonly, methods that would normally
        /// change the sequence or number of members will be prevented and throw an 
        /// Application error.
        /// </summary>
        private bool _isReadOnly;

        #endregion

        #region Constructors

        /// <summary>
        ///Initializes a new instance of the EventList&lt;T&gt; class that is empty and has the default initial capacity.
        /// </summary>
        public EventList()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the EventList&lt;T&gt; class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="Capactiy"> The number of elements that the new list can initially store.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">capacity is less than 0.</exception>
        public EventList(int Capactiy)
        {
            _list = new List<T>(Capactiy);
        }

        /// <summary>
        /// Initializes a new instance of the EventList&lt;T&gt; class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="System.ArgumentNullException">collection is null.</exception>
        public EventList(System.Collections.Generic.IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        #endregion

        #region Methods

        #region Add

        /// <summary>
        /// Adds an object to the end of the System.Collections.Generic.List&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to be added to the end of the System.Collections.Generic.List&lt;T&gt;.
        /// The value can be null for reference types.</param>
        /// <exception cref="System.ApplicationException">Unable to add while the ReadOnly property is set to true.</exception>
        public virtual void Add(T item)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to add while the ReadOnly property is set to true.");
            }
            OnBeforeItemAdded(item);
            _list.Add(item);
            OnAfterItemAdded(item, _list.Count - 1);
            OnListChanged();
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the System.Collections.Generic.List&lt;T&gt;
        /// </summary>
        /// <param name="collection">collection: The collection whose elements should be added to the end of the
        /// System.Collections.Generic.List&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, 
        /// if type T is a reference type.</param>
        /// <exception cref="System.ApplicationException">Unable to add while the ReadOnly property is set to true.</exception>
        public virtual void AddRange(System.Collections.Generic.IEnumerable<T> collection)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to add while the ReadOnly property is set to true.");
            }

            if (OnBeforeRangeAdded(collection) == true) return;  // Canceled if true
            int indx = _list.Count;
            _list.AddRange(collection);
            
            OnAfterRangeAdded(collection, indx) ;
            OnListChanged();
        }

        #endregion


        #region BinarySearch

        /// <summary>
        /// Searches the entire sorted System.Collections.Generic.List&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.-or-null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <returns>The zero-based index of item in the sorted System.Collections.Generic.List&lt;T&gt; , if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of System.Collections.Generic.List&lt;T&gt;.Count.</returns>
        /// <exception cref="System.InvalidOperationException">comparer is null, and the default comparer 
        /// System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt;generic
        /// interface or the System.IComparable interface for type T.</exception>
        public virtual int BinarySearch(T item, System.Collections.Generic.IComparer<T> comparer)
        {
            return _list.BinarySearch(item, comparer);
        }

        /// <summary>
        /// Searches the entire sorted System.Collections.Generic.List&lt;T&gt; for an element using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>The zero-based index of item in the sorted System.Collections.Generic.List&lt;T&gt;, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of System.Collections.Generic.List&lt;T&gt;.Count.</returns>
        /// <exception cref="System.InvalidOperationException">The default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type T.</exception> 
        public virtual int BinarySearch(T item)
        {
            return _list.BinarySearch(item);
        }

        /// <summary>
        /// Searches a range of elements in the sorted System.Collections.Generic.List&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <returns></returns>
        public virtual int BinarySearch(int index, int count, T item, System.Collections.Generic.IComparer<T> comparer)
        {
            return _list.BinarySearch(index, count, item, comparer);
        }

        #endregion


        /// <summary>
        /// Removes all elements from the EventList&lt;T&gt;.
        /// </summary>
        /// <exception cref="System.ApplicationException">Unable to clear while the ReadOnly property is set to true.</exception>
        public virtual void Clear()
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to clear while the ReadOnly property is set to true.");
            }

            if (OnBeforeListCleared() == true) return; // Canceled if true
            _list.Clear();
            OnAfterListCleared();
            OnListChanged();
        }

        /// <summary>
        /// Converts the elements in the current EventList&lt;T&gt; to another type, and returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">The output type to convert to</typeparam>
        /// <param name="converter">A System.Converter&lt;TInput,TOutput&gt; delegate that converts each element from one type to another type.</param>
        /// <returns>A EventList&lt;T&gt; of the target type containing the converted elements from the current EventList&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">converter is null.</exception>
        public virtual System.Collections.Generic.List<TOutput> ConvertAll<TOutput>(System.Converter<T, TOutput> converter)
        {
            return _list.ConvertAll<TOutput>(converter);
        }

        /// <summary>
        /// Determines whether an element is in the System.Collections.Generic.List&lt;T&gt;.
        /// </summary>
        /// <param name="item"> The object to locate in the System.Collections.Generic.List&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>true if item is found in the System.Collections.Generic.List&lt;T&gt;; otherwise, false.</returns>
        public virtual bool Contains(T item)
        {
            return _list.Contains(item);
        }

        #region CopyTo

        /// <summary>
        /// Copies a range of elements from the EventList&lt;T&gt; to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source EventList&lt;T&gt; at which copying begins</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from EventList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"> index is less than 0.-or-arrayIndex is less than 0.-or-count is less than 0.</exception>
        /// <exception cref="System.ArgumentException">index is equal to or greater than the EventList&lt;T&gt;.Count of the source EventList&lt;T&gt;.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements from index to the end of the source EventList&lt;T&gt; is greater than the available space from arrayIndex to the end of the destination array.</exception> 
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            _list.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Copies the entire System.Collections.Generic.List&lt;T&gt; to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex"> The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentException">System.ArgumentException: arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source System.Collections.Generic.List&lt;T&gt; is greater than the available space from arrayIndex to the end of the destination array. </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0</exception>
        /// <exception cref="System.ArgumentNullException">array is null</exception>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Copies the entire EventList&lt;T&gt; to a compatible one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from EventList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <exception cref="System.ArgumentException">The number of elements in the source EventList&lt;T&gt; is greater than the number of elements that the destination array can contain.</exception>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        public virtual void CopyTo(T[] array)
        {
            _list.CopyTo(array);
        }

        #endregion


        /// <summary>
        /// Determines whether the EventList&lt;T&gt; contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the EventList&lt;T&gt; contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">match is null.</exception>
        public virtual bool Exists(System.Predicate<T> match)
        {
            return _list.Exists(match);
        }

        #region Find

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the EventList&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match"> The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, �1</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex is outside the range of valid indexes for the EventList&lt;T&gt;.-or-count is less than 0.-or-startIndex and count do not specify a valid section in the EventList&lt;T&gt;.</exception>
        /// <exception cref="System.ArgumentNullException">match is null</exception>
        public virtual int FindIndex(int startIndex, int count, System.Predicate<T> match)
        {
            return _list.FindIndex(startIndex, count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the EventList&lt;T&gt; that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex is outside the range of valid indexes for the EventList&lt;T&gt;.</exception>
        /// <exception cref="System.ArgumentNullException">match is null.</exception> 
        public virtual int FindIndex(int startIndex, System.Predicate<T> match)
        {
            return _list.FindIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire EventList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentNullException">match is null.</exception>
        public virtual int FindIndex(System.Predicate<T> match)
        {
            return _list.FindIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire EventList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.</returns>
        /// <exception cref= "System.ArgumentNullException">match is null."</exception>
        public virtual T FindLast(System.Predicate<T> match)
        {
            return _list.FindLast(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the EventList&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match"></param>
        /// <returns>The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex is outside the range of valid indexes for the EventList&lt;T&gt;.-or-count is less than 0.-or-startIndex and count do not specify a valid section in the EventList&lt;T&gt;.</exception>
        /// <exception cref="System.ArgumentNullException">match is null.</exception> 
        public virtual int FindLastIndex(int startIndex, int count, System.Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the EventList&lt;T&gt; that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex"> The zero-based starting index of the backward search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">: startIndex is outside the range of valid indexes for the EventList&lt;T&gt;.</exception>
        public virtual int FindLastIndex(int startIndex, System.Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire EventList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentNullException">match is null.</exception> 
        public virtual int FindLastIndex(System.Predicate<T> match)
        {
            return _list.FindLastIndex(match);
        }


        #endregion


        /// <summary>
        /// Performs the specified action on each element of the EventList&lt;T&gt;.
        /// </summary>
        /// <param name="action"> The System.Action&lt;T&gt; delegate to perform on each element of the EventList&lt;T&gt;.</param>
        /// <exception cref="System.ArgumentNullException"> action is null.</exception>
        public virtual void ForEach(System.Action<T> action)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to perform actions while the ReadOnly property is set to true.");
            }
            _list.ForEach(action);
            OnListChanged();
        }

       
        /// <summary>
        /// Returns an enumerator that iterates through this list
        /// </summary>
        /// <returns></returns>
        public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source EventList&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based EventList&lt;T&gt; index at which the range starts.</param>
        /// <param name="count"> The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source EventList&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-count is less than 0.</exception>
        /// <exception cref="System.ArgumentException">index and count do not denote a valid range of elements in the EventList&lt;T&gt;.</exception>
        public virtual List<T> GetRange(int index, int count)
        {
            return _list.GetRange(index, count);
        }


        #region IndexOf

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the EventList&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the EventList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index"> The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of item within the range of elements in the EventList&lt;T&gt; that starts at index and contains count number of elements, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"> index is outside the range of valid indexes for the EventList&lt;T&gt;.-or-count is less than 0.-or-index and count do not specify a valid section in the EventList&lt;T&gt;.</exception>
        public virtual int IndexOf(T item, int index, int count)
        {
            return _list.IndexOf(item, index, count);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the EventList&lt;T&gt; that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the EventList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index"> The zero-based starting index of the search.</param>
        /// <returns>The zero-based index of the first occurrence of item within the range of elements in the EventList&lt;T&gt; that extends from index to the last element, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is outside the range of valid indexes for the EventList&lt;T&gt;.</exception>
        public virtual int IndexOf(T item, int index)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire System.Collections.Generic.List&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the System.Collections.Generic.List&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire System.Collections.Generic.List&lt;T&gt;, if found; otherwise, �1.</returns>
        public virtual int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        #endregion


        #region Insert

        /// <summary>
        /// Inserts the elements of a collection into the EventList&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the EventList&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is greater than EventList&lt;T&gt;.Count.</exception>
        /// <exception cref="System.ArgumentNullException">collection is null.</exception>
        /// <exception cref="System.ApplicationException">Unable to insert while the ReadOnly property is set to true.</exception>
        public virtual void InsertRange(int index, System.Collections.Generic.IEnumerable<T> collection)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to insert while the ReadOnly property is set to true.");
            }
            if (OnBeforeRangeInserted(collection, index) == true) return; // Canceled if true
            _list.InsertRange(index, collection);
            OnAfterRangeInserted(collection, index);
            OnListChanged();
        }

        /// <summary>
        /// Inserts an element into the System.Collections.Generic.List&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is greater than System.Collections.Generic.List&lt;T&gt;.Count.</exception>
        /// <exception cref="System.ApplicationException">Unable to insert while the ReadOnly property is set to true.</exception>
        public virtual void Insert(int index, T item)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to insert while the ReadOnly property is set to true.");
            }
            if (OnBeforeItemInserted(item, index) == true) return; // Canceled if true
            _list.Insert(index, item);
            OnAfterItemInserted(item, index);
            OnListChanged();
        }

        #endregion


        #region LastIndexOf
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the EventList&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the EventList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the last occurrence of item within the range of elements in the EventList&lt;T&gt; that contains count number of elements and ends at index, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is outside the range of valid indexes for the EventList&lt;T&gt;.-or-count is less than 0.-or-index and count do not specify a valid section in the EventList&lt;T&gt;.</exception>
        public virtual int LastIndexOf(T item, int index, int count)
        {
            return _list.LastIndexOf(item, index, count);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the EventList&lt;T&gt; that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the EventList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>The zero-based index of the last occurrence of item within the range of elements in the EventList&lt;T&gt; that extends from the first element to index, if found; otherwise, �1.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is outside the range of valid indexes for the EventList&lt;T&gt;.</exception>
        public virtual int LastIndexOf(T item, int index)
        {
            return _list.LastIndexOf(item, index, Count);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire EventList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the EventList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the last occurrence of item within the entire the EventList&lt;T&gt;, if found; otherwise, �1.</returns>
        public virtual int LastIndexOf(T item)
        {
            return _list.LastIndexOf(item);
        }



        #endregion

        #region Remove

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.Generic.List&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the System.Collections.Generic.List&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not
        /// found in the System.Collections.Generic.List&lt;T&gt;.</returns>
        /// <exception cref="System.ApplicationException">Unable to remove while the ReadOnly property is set to true.</exception>
        public virtual bool Remove(T item)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to remove while the ReadOnly property is set to true.");
            }
            bool res = false;
            if(OnBeforeItemRemoved(item, _list.IndexOf(item))== true)return false;
            res = _list.Remove(item);
            OnAfterItemRemoved(item);
            OnListChanged();
            return res;
        }

        /// <summary>
        /// Removes the all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the EventList&lt;T&gt;</returns>
        /// <exception cref="System.ArgumentNullException">match is null.</exception>
        /// <exception cref="System.ApplicationException">Unable to remove while the ReadOnly property is set to true.</exception>
        public virtual int RemoveAll(System.Predicate<T> match)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to remove while the ReadOnly property is set to true.");
            }
            List<T> matches = _list.FindAll(match);
            if (OnBeforeAllMatchingRemoved(matches) == true) return 0;
            int NumRemoved = 0;
            NumRemoved = _list.RemoveAll(match);
            OnAfterAllMatchingRemoved(matches);
            OnListChanged();
            return NumRemoved;
        }

        /// <summary>
        /// Removes the element at the specified index of the System.Collections.Generic.List&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ApplicationException">Unable to remove while the ReadOnly property is set to true.</exception>
        public virtual void RemoveAt(int index)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to remove while the ReadOnly property is set to true.");
            }
            T item = _list[index];
            if (OnBeforeItemRemoved(item, index) == true) return;
            _list.RemoveAt(index);
            OnAfterItemRemoved(item);
            OnListChanged();
        }

        /// <summary>
        /// Removes a range of elements from the EventList&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-count is less than 0.</exception>
        /// <exception cref="System.ArgumentException">index and count do not denote a valid range of elements in the EventList&lt;T&gt;.</exception>
        /// <exception cref="System.ApplicationException">Unable to remove while the ReadOnly property is set to true.</exception>
        public virtual void RemoveRange(int index, int count)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to remove while the ReadOnly property is set to true.");
            }
            List<T> range = _list.GetRange(index, count); // I hope this will work.  Hopefully the elements won't be destroyed as they are removed from the list.
            if (OnBeforeRangeRemoved(range, index) == true) return;
            _list.RemoveRange(index, count);
            OnAfterRangeRemoved(range);
            OnListChanged();
        }

        #endregion

        #region Reverse


        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="System.ArgumentException">index and count do not denote a valid range of elements in the EventList&lt;T&gt;.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-count is less than 0.</exception>
        /// <exception cref="System.ApplicationException">Unable to reverse while the ReadOnly property is set to true.</exception>
        public virtual void Reverse(int index, int count)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to reverse while the ReadOnly property is set to true.");
            }
            if (OnBeforeReversed() == true) return; // Cancelled if true
            List<T> range = _list.GetRange(index, count);
            if (OnBeforeRangeReversed(index, range) == true) return; // Cancelled if true
            _list.Reverse(index, count);
            range = _list.GetRange(index, count);
            OnAfterRangeReversed(index, range);
            OnAfterReversed();
            OnListChanged();
        }

        /// <summary>
        /// Reverses the order of the elements in the entire EventList&lt;T&gt;.
        /// </summary>
        /// <exception cref="System.ApplicationException">Unable to reverse while the ReadOnly property is set to true.</exception>
        public virtual void Reverse()
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to reverse while the ReadOnly property is set to true.");
            }
            if (OnBeforeReversed() == true) return; // Cancelled if true;
            if (OnBeforeListReversed() == true) return; // Cancelled if true;
            _list.Reverse();
            OnAfterListReversed();
            OnAfterReversed();
            OnListChanged();

        }

        #endregion



        #region Sort

        /// <summary>
        /// Sorts the elements in the entire EventList&lt;T&gt; using the specified System.Comparison&lt;T&gt;.
        /// </summary>
        /// <param name="comparison">The System.Comparison&lt;T&gt; to use when comparing elements.</param>
        /// <exception cref="System.ArgumentException">The implementation of comparison caused an error during the sort. For example, comparison might not return 0 when comparing an item with itself.</exception>
        /// <exception cref="System.ArgumentNullException">comparison is null.</exception> 
        /// <exception cref="System.ApplicationException">Unable to sort while the ReadOnly property is set to true.</exception>
        public virtual void Sort(System.Comparison<T> comparison)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to sort while the ReadOnly property is set to true.");
            }
            if (OnBeforeSort() == true) return; // Cancelled if true
            if (OnBeforeSortByComparison(comparison) == true) return; // Cancelled if true
            _list.Sort(comparison);
            OnAfterSortByComparison(comparison);
            OnAfterSort();
            OnListChanged();
        }

        /// <summary>
        /// Sorts the elements in a range of elements in EventList&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="index"> The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <exception cref="System.ArgumentException">index and count do not specify a valid range in the EventList&lt;T&gt;.-or-The implementation of comparer caused an error during the sort. For example, comparer might not return 0 when comparing an item with itself.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-count is less than 0.</exception>
        /// <exception cref="System.InvalidOperationException"> comparer is null, and the default comparer
        /// System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find implementation of the System.IComparable&lt;T&gt;
        /// generic interface or the System.IComparable interface for type T.</exception>
        /// <exception cref="System.ApplicationException">Unable to sort while the ReadOnly property is set to true.</exception>
        public virtual void Sort(int index, int count, System.Collections.Generic.IComparer<T> comparer)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to sort while the ReadOnly property is set to true.");
            }
            if (OnBeforeSort() == true) return; // Cancelled if true
            List<T> range = _list.GetRange(index, count);
            if (OnBeforeRangeSorted(index, range, comparer) == true) return; // Cancelled if true
            _list.Sort(index, count, comparer);
            range = _list.GetRange(index, count);
            OnAfterRangeSorted(index, range, comparer);
            OnAfterSort();
            OnListChanged();
        }

        /// <summary>
        /// Sorts the elements in the entire MapWindow.Interfaces.Framework.IEventList&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="comparer"> The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <exception cref="System.ArgumentException">The implementation of comparer caused an error during the sort. For example, comparer might not return 0 when comparing an item with itself.</exception>
        /// <exception cref="System.InvalidOperationException">comparer is null, and the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type T.</exception>
        /// <exception cref="System.ApplicationException">Unable to sort while the ReadOnly property is set to true.</exception>
        public virtual void Sort(System.Collections.Generic.IComparer<T> comparer)
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to sort while the ReadOnly property is set to true.");
            }
            if (OnBeforeSort() == true) return; // Cancelled if true
            if (OnBeforeListSorted(comparer) == true) return; // Cancelled if true
            _list.Sort(comparer);
            OnAfterListSorted(comparer);
            OnAfterSort();
            OnListChanged();
        }

        /// <summary>
        /// Sorts the elements in the entire MapWindow.Interfaces.Framework.IEventList&lt;T&gt; using the default comparer.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type T.</exception>
        /// <exception cref="System.ApplicationException">Unable to sort while the ReadOnly property is set to true.</exception>
        public virtual void Sort()
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to sort while the ReadOnly property is set to true.");
            }
            if (OnBeforeSort() == true) return; // Cancelled if true
            _list.Sort();
            OnAfterSort();
            OnListChanged();
        }

        /// <summary>
        /// Copies the elements of the MapWindow.Interfaces.Framework.IEventList&lt;T&gt; to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the MapWindow.Interfaces.Framework.IEventList&lt;T&gt;.</returns>
        public virtual T[] ToArray()
        {
            return _list.ToArray();
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the MapWindow.Interfaces.Framework.IEventList&lt;T&gt;, if that number is less than a threshold value.
        /// </summary>
        /// <exception cref="System.ApplicationException">Unable to trim while the ReadOnly property is set to true.</exception>
        public virtual void TrimExcess()
        {
            if (_isReadOnly == true)
            {
                throw new ApplicationException("Unable to sort while the ReadOnly property is set to true.");
            }
            _list.TrimExcess();
            OnListChanged();
        }

        /// <summary>
        /// Determines whether every element in the MapWindow.Interfaces.Framework.IEventList&lt;T&gt; matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions to check against the elements.</param>
        /// <returns>true if every element in the MapWindow.Interfaces.Framework.IEventList&lt;T&gt; matches the conditions defined by the specified predicate; otherwise, false. If the list has no elements, the return value is true.</returns>
        /// <exception cref="System.ArgumentNullException">match is null.</exception>
        public virtual bool TrueForAll(System.Predicate<T> match)
        {
            return _list.TrueForAll(match);
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the MapWindow.Interfaces.Framework.IEventList&lt;T&gt; can contain before resizing is required.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">MapWindow.Interfaces.Framework.IEventList&lt;T&gt;.Capacity is set to a value that is less than MapWindow.Interfaces.Framework.IEventList&lt;T&gt;.Count.</exception>
        public virtual int Capacity
        {
            get
            {
                return _list.Capacity;
            }
            set
            {
                _list.Capacity = value;
            }
        }

        /// <summary>
        /// The default, indexed value of type T
        /// </summary>
        /// <param name="index">The numeric index</param>
        /// <returns>An object of type T corresponding to the index value specified</returns>
        public virtual T this[int index]
        {
            get
            {
                if (_list == null) return default(T);
                if (_list.Count == 0) return default(T);
                if (index < 0 || index >= _list.Count)
                {
                    throw new System.IndexOutOfRangeException("Index was outside the range of the list, which should range from 0 to Count - 1");
                }
                return _list[index];
            }
            set
            {
                if (_list == null) _list = new List<T>();
                _list[index] = value;
                OnListChanged();
            }
        }

        /// <summary>
        /// Integer, the total number of items currently stored in the list
        /// </summary>
        public virtual int Count
        {
            get
            {
                return _list.Count;
            }
        }


        #endregion




        #endregion

        #region Properties

        /// <summary>
        /// Gets a boolean property indicating whether this list can be written to.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            protected set
            {
                _isReadOnly = value;
            }
        }
       

        #endregion

        #region Events


        #region Add

        /// <summary>
        /// Occurs after an item has already been added to the list.
        /// The index where the item was added is specified.
        /// </summary>
        public virtual event EventHandler<IndividualIndex<T>> AfterItemAdded;
        
        /// <summary>
        /// Fires the AfterItemAdded Event
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Index"></param>
        protected virtual void OnAfterItemAdded(T Item, int Index)
        {
            if (AfterItemAdded != null) AfterItemAdded(this, new IndividualIndex<T>(Item, Index));
        }

        
        /// <summary>
        /// Occurs after a range has already been added to the list.
        /// This reveals the index where the beginning of the range
        /// was added, but cannot be canceled.
        /// </summary>
        public virtual event EventHandler<CollectiveIndex<T>> AfterRangeAdded;

        /// <summary>
        /// Fires the AfterRangeAdded method
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Index"></param>
        protected virtual void OnAfterRangeAdded(IEnumerable<T> collection, int Index)
        {
            if (AfterRangeAdded != null)
            {
                AfterRangeAdded(this, new CollectiveIndex<T>(collection, Index));
            }
        }

        /// <summary>
        /// Occurs before an item is added to the List.
        /// There is no index yet specified because it will be added to the
        /// end of the list.
        /// </summary>
        public virtual event EventHandler<IndividualCancel<T>> BeforeItemAdded;

        /// <summary>
        /// Fires the BeforeItemAdded Event
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        protected virtual bool OnBeforeItemAdded(T Item)
        {
            if (BeforeItemAdded != null)
            {
                IndividualCancel<T> e = new IndividualCancel<T>(Item);
                BeforeItemAdded(this, e);
                return e.Cancel;
            }
            return false;
        }


        /// <summary>
        /// Occurs before a range of items is added to the list.
        /// There is no index yet, but this event can be cancelled.
        /// </summary>
        public virtual event EventHandler<CollectiveCancel<T>> BeforeRangeAdded;

        /// <summary>
        /// Fires the BeforeRangeAdded Event
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected virtual bool OnBeforeRangeAdded(IEnumerable<T> collection)
        {
            if (BeforeRangeAdded != null)
            {
                CollectiveCancel<T> e = new CollectiveCancel<T>(collection);
                BeforeRangeAdded(this, e);
                return e.Cancel;
            }
            return false;
        }


       

        #endregion


        #region Clear

        /// <summary>
        /// Occurs after the the list is cleared.
        /// </summary>
        public virtual event EventHandler AfterListCleared;

        /// <summary>
        /// Fires the AfterListCleared event
        /// </summary>
        protected void OnAfterListCleared()
        {
            if (AfterListCleared != null) AfterListCleared(this, new System.EventArgs());
        }

        /// <summary>
        /// Occurs before the list is cleared and can cancel the event.
        /// </summary>
        public virtual event CancelEventHandler BeforeListCleared;

        /// <summary>
        /// Fires the BeforeListCleared event
        /// </summary>
        /// <returns></returns>
        protected bool OnBeforeListCleared()
        {
            if (BeforeListCleared != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                BeforeListCleared(this, e);
                return e.Cancel;
            }
            return false;
        }

        #endregion


        #region Insert

        /// <summary>
        /// Occurs after an item is inserted.
        /// Shows the true index of the item after it was actually added.
        /// </summary>
        public virtual event EventHandler<IndividualIndex<T>> AfterItemInserted;

        /// <summary>
        /// Fires the AfterItemInserted event
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Index"></param>
        protected void OnAfterItemInserted(T Item, int Index)
        {
            if (AfterItemInserted != null) AfterItemInserted(this, new IndividualIndex<T>(Item, Index));
        }

        /// <summary>
        /// Occurs after an item is inserted.
        /// Shows the true index of the item after it was actually added.
        /// </summary>
        public virtual event EventHandler<CollectiveIndex<T>> AfterRangeInserted;

        /// <summary>
        /// Fires the AfterRangeInserted event
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Index"></param>
        protected void OnAfterRangeInserted(IEnumerable<T> collection, int Index)
        {
            if (AfterRangeInserted != null) AfterRangeInserted(this, new CollectiveIndex<T>(collection, Index));
        }


        /// <summary>
        /// Occurs before an item is inserted.  The index of the requested
        /// insertion as well as the item being inserted and an option to
        /// cancel the event are specified
        /// </summary>
        public virtual event EventHandler<IndividualIndexCancel<T>> BeforeItemInserted;

        /// <summary>
        /// Fires the BeforeItemInserted event
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        protected bool OnBeforeItemInserted(T Item, int Index)
        {
            if (BeforeItemInserted != null)
            {
                IndividualIndexCancel<T> e = new IndividualIndexCancel<T>(Item, Index);
                BeforeItemInserted(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Occurs before a range is inserted.  The index of the requested
        /// insertion location as well as the item being inserted and an option to
        /// cancel the event are provided in the event arguments
        /// </summary>
        public virtual event EventHandler<CollectiveIndexCancel<T>> BeforeRangeInserted;

        /// <summary>
        /// Fires the BeforeRangeInserted event
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        protected bool OnBeforeRangeInserted(IEnumerable<T> collection, int Index)
        {
            if (BeforeRangeInserted != null)
            {
                CollectiveIndexCancel<T> e = new CollectiveIndexCancel<T>(collection, Index);
                BeforeRangeInserted(this, e);
                return e.Cancel;
            }
            return false;
        }



        #endregion

        /// <summary>
        /// Occurs after a method that sorts or reorganizes the list
        /// </summary>
        public virtual event EventHandler ListChanged;

        /// <summary>
        /// Fires the ListChanged Event
        /// </summary>
        protected void OnListChanged()
        {
            if (ListChanged != null) ListChanged(this, new System.EventArgs()); ;
        }

        #region Reverse

        /// <summary>
        /// Occurs just after the list or any sub portion
        /// of the list is sorted.  This event occurs in
        /// addition to the specific reversal case.
        /// </summary>
        public virtual event EventHandler AfterReversed;

        /// <summary>
        /// fires the AfterReversed event
        /// </summary>
        protected void OnAfterReversed()
        {
            if (AfterReversed == null) return;
            AfterReversed(this, new System.EventArgs());
        }



        /// <summary>
        /// Occurs just before the list or any sub portion
        /// of the list is sorted.  This event occurs in
        /// addition to the specific reversal case.
        /// </summary>
        public virtual event CancelEventHandler BeforeReversed;

        /// <summary>
        /// Fires the BeforeReversed event
        /// </summary>
        /// <returns></returns>
        protected bool OnBeforeReversed()
        {
            if (BeforeReversed == null) return false;
            CancelEventArgs e = new CancelEventArgs();
            BeforeReversed(this, e);
            return e.Cancel;
        }

        /// <summary>
        /// Occurs before a specific range is reversed
        /// </summary>
        public virtual event EventHandler<CollectiveIndexCancel<T>> BeforeRangeReversed;

        /// <summary>
        /// Fires the BeforeRangeReversed event
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        protected bool OnBeforeRangeReversed(int Index, IEnumerable<T> range)
        {
            if (BeforeRangeReversed == null) return false;
            CollectiveIndexCancel<T> e = new CollectiveIndexCancel<T>(range, Index);
            BeforeRangeReversed(this, e);
            return e.Cancel;
        }


        /// <summary>
        /// Occurs after a specific range is reversed
        /// </summary>
        public virtual event EventHandler<CollectiveIndex<T>> AfterRangeReversed;

        /// <summary>
        /// Fires the AfterRangeReversed event
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="collection"></param>
        protected void OnAfterRangeReversed(int Index, IEnumerable<T> collection)
        {
            if(AfterRangeReversed == null)return;
            AfterRangeReversed(this, new CollectiveIndex<T>(collection, Index));
        }


        /// <summary>
        /// Occurs before the entire list is reversed
        /// </summary>
        public virtual event CancelEventHandler BeforeListReversed;

        /// <summary>
        /// Fires the BeforeLiestReversed event
        /// </summary>
        /// <returns></returns>
        protected bool OnBeforeListReversed()
        {
            if (BeforeListReversed == null) return false;
            CancelEventArgs e = new CancelEventArgs();
            BeforeListReversed(this, e);
            return e.Cancel;
        }


        /// <summary>
        /// Occurs after the entire list is reversed
        /// </summary>
        public virtual event EventHandler AfterListReversed;

        /// <summary>
        /// fires the AfterListReversed Event
        /// </summary>
        protected void OnAfterListReversed()
        {
            if (AfterListReversed == null) return;
            AfterListReversed(this, new System.EventArgs());
        }

        #endregion

        #region Remove

        /// <summary>
        /// Occurs before an item is removed from the List.
        /// Specifies the item, the current index and an option to cancel.
        /// </summary>
        public virtual event EventHandler<IndividualIndexCancel<T>> BeforeItemRemoved;

        /// <summary>
        /// Fires the BeforeItemRemoved event
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        protected bool OnBeforeItemRemoved(T Item, int Index)
        {
            if (BeforeItemRemoved != null)
            {
                IndividualIndexCancel<T> e = new IndividualIndexCancel<T>(Item, Index);
                BeforeItemRemoved(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Occurs before a range is removed from the List.
        /// Specifies the range, the current index and an option to cancel.
        /// </summary>
        public virtual event EventHandler<CollectiveIndexCancel<T>> BeforeRangeRemoved;

        /// <summary>
        /// Fires the BeforeRangeRemoved event
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        protected bool OnBeforeRangeRemoved(IEnumerable<T> collection, int Index)
        {
            if (BeforeRangeRemoved != null)
            {
                CollectiveIndexCancel<T> e = new CollectiveIndexCancel<T>(collection, Index);
                BeforeRangeRemoved(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Occurs before all the elements that match a predicate are removed.
        /// Supplies an IEnumerable list in the event args of all the items
        /// that will match the expression.  This action can be cancelled.
        /// </summary>
        public virtual event EventHandler<CollectiveCancel<T>> BeforeAllMatchingRemoved;

        /// <summary>
        /// Fires the BeforeAllMatchingRemoved event
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected virtual bool OnBeforeAllMatchingRemoved(IEnumerable<T> collection)
        {
            if (BeforeAllMatchingRemoved != null)
            {
                CollectiveCancel<T> e = new CollectiveCancel<T>(collection);
                BeforeAllMatchingRemoved(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Occurs after all the elements that matched a predicate were 
        /// removed.  The values are the items that were successfully removed.
        /// The action has already happened, and so cannot be cancelled here.
        /// </summary>
        public virtual event EventHandler<Collective<T>> AfterAllMatchingRemoved;

        /// <summary>
        /// Fires the AfterAllMatchingRemoved event
        /// </summary>
        /// <param name="collection"></param>
        protected virtual void OnAfterAllMatchingRemoved(IEnumerable<T> collection)
        {
            if (AfterAllMatchingRemoved != null)
            {
                AfterAllMatchingRemoved(this, new Collective<T>(collection));
            }

        }
       

        /// <summary>
        /// Occurs after an item is removed from the List.
        /// Gives a handle to the item that was removed, but it no longer
        /// has a meaningful index value or an option to cancel.
        /// </summary>
        public virtual event EventHandler<Individual<T>> AfterItemRemoved;

        /// <summary>
        /// Fires the AfterItemRemoved event
        /// </summary>
        /// <param name="Item"></param>
        protected void OnAfterItemRemoved(T Item)
        {
            if (AfterItemRemoved != null) AfterItemRemoved(this, new Individual<T>(Item));
        }


        /// <summary>
        /// Occurs after an item is removed from the List.
        /// Gives a handle to the range that was removed, but it no longer
        /// has a meaningful index value or an option to cancel.
        /// </summary>
        public virtual event EventHandler<Collective<T>> AfterRangeRemoved;

        /// <summary>
        /// Fires the AfterRangeRemoved Event
        /// </summary>
        /// <param name="collection"></param>
        protected void OnAfterRangeRemoved(IEnumerable<T> collection)
        {
            if (AfterRangeRemoved != null)AfterRangeRemoved(this, new Collective<T>(collection));
        }

        #endregion

        #region Sort
        /// <summary>
        /// Occurs just before any of the specific sorting methodsin addition
        /// to the event associated with the specific method.
        /// </summary>
        public virtual event CancelEventHandler BeforeSort;

        /// <summary>
        /// Fires the BeforeSort event
        /// </summary>
        /// <returns></returns>
        protected bool OnBeforeSort()
        {
            if (BeforeSort == null) return false;
            CancelEventArgs e = new CancelEventArgs();
            BeforeSort(this, e);
            return e.Cancel;
        }
        
        /// <summary>
        /// Occurs after any of the specific sorting methods in addition
        /// to the event associated with the specific method.
        /// </summary>
        public virtual event EventHandler AfterSort;

        /// <summary>
        /// Fires the AfterSort event
        /// </summary>
        protected void OnAfterSort()
        {
            if (AfterSort == null) return;
            AfterSort(this, new EventArgs());
        }


        /// <summary>
        /// Occurs just before the entire list is sorted
        /// </summary>
        public virtual event EventHandler<CompareCancel<T>> BeforeListSorted;

        /// <summary>
        /// Fires the BeforeListSorted event
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected bool OnBeforeListSorted(System.Collections.Generic.IComparer<T> comparer)
        {
            if (BeforeListSorted == null) return false;
            CompareCancel<T> e = new CompareCancel<T>(comparer);
            BeforeListSorted(this, e);
            return e.Cancel;
        }

        /// <summary>
        /// Occurs after the entire list has been sorted
        /// </summary>
        public virtual event EventHandler<Compare<T>> AfterListSorted;

        /// <summary>
        /// Fires the AfterListSorted event
        /// </summary>
        /// <param name="comparer"></param>
        protected void OnAfterListSorted(System.Collections.Generic.IComparer<T> comparer)
        {
            if (AfterListSorted == null) return;
            AfterListSorted(this, new Compare<T>(comparer));
        }



        /// <summary>
        /// Occurs just before the Sort method that uses a System.Comparison&lt;T&gt;
        /// This event can cancel the action.
        /// </summary>
        public virtual event EventHandler<ComparisonCancel<T>> BeforeSortByComparison;

        /// <summary>
        /// Fires the BeforeSortByComparison
        /// </summary>
        /// <param name="comparison"></param>
        /// <returns></returns>
        protected bool OnBeforeSortByComparison(System.Comparison<T> comparison)
        {
            if (BeforeSortByComparison == null) return false;
            ComparisonCancel<T> e = new ComparisonCancel<T>(comparison);
            BeforeSortByComparison(this, e);
            return e.Cancel;
        }

        /// <summary>
        /// Occurs just after the Sort method that uses a System.Comparison&lt;T&gt;
        /// </summary>
        public virtual event EventHandler<ComparisonArgs<T>> AfterSortByComparison;

        /// <summary>
        /// Fires the AfterSortByComparison event
        /// </summary>
        /// <param name="comparison"></param>
        protected void OnAfterSortByComparison(System.Comparison<T> comparison)
        {
            if (AfterSortByComparison == null) return;
            AfterSortByComparison(this, new ComparisonArgs<T>(comparison));
        }

        /// <summary>
        /// Occurs just before the Sort method that only sorts a specified range.
        /// This event can cancel the action.
        /// </summary>
        public virtual event EventHandler<CollectiveIndexCompareCancel<T>> BeforeRangeSorted;

        /// <summary>
        /// Fires the BeforeRangeSorted event
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="collection"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected bool OnBeforeRangeSorted(int Index, IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (BeforeRangeSorted == null) return false;
            CollectiveIndexCompareCancel<T> e = new CollectiveIndexCompareCancel<T>(collection, comparer, Index);
            BeforeRangeSorted(this, e);
            return e.Cancel;
        }


        /// <summary>
        /// Occurs just after the Sort method that only sorts a specified range.
        /// </summary>
        public virtual event EventHandler<CollectiveIndexCompare<T>> AfterRangeSorted;

        /// <summary>
        /// Fires the AfterRangeSorted event
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="collection"></param>
        /// <param name="comparer"></param>
        protected void OnAfterRangeSorted(int Index, IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (AfterRangeSorted == null) return;
            AfterRangeSorted(this, new CollectiveIndexCompare<T>(collection, comparer, Index));
        }


        #endregion


        #endregion


        #region IEventList<T> Members

        /// <summary>
        /// Converts between one output an another
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        List<TOutput> IEventList<T>.ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return _list.ConvertAll<TOutput>(converter);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an Enumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }

   
}
