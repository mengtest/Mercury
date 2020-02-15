using System;
using System.Collections;
using System.Collections.Generic;

public class DisorderList<T> : IList<T> where T : IEquatable<T>
{
    private readonly List<T> _list;
    private readonly Func<T, T, bool> _isEqual;
    private int _pivot;

    public int Count { get; private set; }
    public bool IsReadOnly { get; } = false;

    public DisorderList(int capacity = 0, Func<T, T, bool> isEqual = null)
    {
        _list = new List<T>(capacity);
        _isEqual = isEqual;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var e in _list)
        {
            if (!IsValEqual(e, default))
            {
                yield return e;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    public void Add(T item)
    {
        if (_pivot + 1 >= _list.Count)
        {
            _list.Add(default);
        }

        _list[_pivot] = item;
        Count += 1;
        while (!IsValEqual(_list[_pivot], default))
        {
            if (_pivot + 1 >= _list.Count)
            {
                _list.Add(default);
            }

            _pivot++;
        }
    }

    public void Clear()
    {
        _list.Clear();
        Count = 0;
    }

    public bool Contains(T item) { return _list.Contains(item); }

    public void CopyTo(T[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }

    public bool Remove(T item)
    {
        var i = IndexOf(item);
        if (i == -1)
        {
            return false;
        }

        _list[i] = default;
        Count -= 1;
        if (i < _pivot)
        {
            _pivot = i;
        }

        return true;
    }

    public int IndexOf(T item)
    {
        for (var i = 0; i < _list.Count; i++)
        {
            if (IsValEqual(_list[i], item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        _list[index] = item;
        Count += 1;
    }

    public void RemoveAt(int index)
    {
        if (!IsValEqual(_list[index], default))
        {
            return;
        }

        _list[index] = default;
        Count -= 1;
    }

    public T this[int index] { get => _list[index]; set => throw new InvalidOperationException(); }

    private bool IsValEqual(T l, T r)
    {
        var del = _isEqual?.Invoke(l, r);
        return del ?? l.Equals(r);
    }
}