using System;
using System.Collections;
using System.Collections.Generic;

public class DisorderList<T> : IList<T> where T : IEquatable<T>
{
    private readonly List<T> _list;
    private readonly T _default;
    private int _pivot;

    public int Count { get; private set; }
    public bool IsReadOnly { get; } = false;

    public DisorderList() : this(default) { }

    public DisorderList(T defaultData, int capacity = 0)
    {
        _list = new List<T>(capacity);
        _default = defaultData;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var e in _list)
        {
            if (!e.Equals(_default))
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
            _list.Add(_default);
        }

        _list[_pivot] = item;
        Count += 1;
        while (!_list[_pivot].Equals(_default))
        {
            if (_pivot + 1 >= _list.Count)
            {
                _list.Add(_default);
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

        _list[i] = _default;
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
            if (_list[i].Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        if (_default.Equals(item))
        {
            return;
        }

        _list[index] = item;
        Count += 1;
    }

    public void RemoveAt(int index)
    {
        if (!_list[index].Equals(_default))
        {
            _list[index] = _default;
            Count -= 1;
        }
    }

    public T this[int index] { get => _list[index]; set => throw new InvalidOperationException(); }
}