using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 优先队列
/// </summary>
public class PriorityQueue<T> : IReadOnlyCollection<T>, ICollection<T>
{
    private readonly List<T> _heap;
    private readonly IComparer<T> _compare;
    public int Count { get; private set; }
    public bool IsReadOnly { get; } = false;
    public T this[int index] { get => _heap[index]; set => _heap[index] = value; }

    public PriorityQueue() : this(0, null) { }

    public PriorityQueue(IComparer<T> compare) : this(0, compare) { }

    public PriorityQueue(int capacity, IComparer<T> compare)
    {
        _heap = new List<T>(capacity);
        _compare = compare ?? Comparer<T>.Default;
    }

    /// <summary>
    /// 入队列
    /// </summary>
    public void Enqueue(T item)
    {
        _heap.Add(item);
        var n = Count++;
        var v = _heap[n];
        for (var n2 = n / 2; n > 0 && _compare.Compare(v, _heap[n2]) > 0; n = n2, n2 /= 2)
        {
            _heap[n] = _heap[n2];
        }

        _heap[n] = v;
    }

    /// <summary>
    /// 返回队列头
    /// </summary>
    /// <exception cref="InvalidOperationException">队列为空</exception>
    public T Peek()
    {
        if (Count > 0)
        {
            return _heap[0];
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// 队列是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() { return Count == 0; }

    /// <summary>
    /// 出队列
    /// </summary>
    public T Dequeue()
    {
        var result = Peek();
        _heap[0] = _heap[--Count];
        var n = 0;
        if (Count <= 0)
        {
            return result;
        }

        var v = _heap[n];
        for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
        {
            if (n2 + 1 < Count && _compare.Compare(_heap[n2 + 1], _heap[n2]) > 0)
            {
                n2++;
            }

            if (_compare.Compare(v, _heap[n2]) >= 0)
            {
                break;
            }

            _heap[n] = _heap[n2];
        }

        _heap[n] = v;
        _heap.RemoveAt(_heap.Count - 1);
        return result;
    }

    private void Swap(int index1, int index2)
    {
        var temp = _heap[index1];
        _heap[index1] = _heap[index2];
        _heap[index2] = temp;
    }

    /// <summary>
    /// 二分查找
    /// </summary>
    /// <returns>元素位置</returns>
    public int BinarySearch(T item) { return _heap.BinarySearch(item); }

    public void TrimExcess() { _heap.TrimExcess(); }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var e in _heap)
        {
            yield return e;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    void ICollection<T>.Add(T item) { Enqueue(item); }

    public void Clear() { _heap.Clear(); }

    public bool Contains(T item) { return _heap.Contains(item); }

    public void CopyTo(T[] array, int arrayIndex) { _heap.CopyTo(array, arrayIndex); }

    bool ICollection<T>.Remove(T item) { throw new NotSupportedException(); }
}