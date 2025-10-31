using System;
using System.Collections.Generic;

public class ContextGroup<T>
    where T : class
{
    private Dictionary<Type, T> _typeGroup = new();

    public void Add(T t)
    {
        _typeGroup[t.GetType()] = t;
    }

    public T Get(Type type)
    {
        return _typeGroup[type];
    }

    public M Get<M>()
        where M : class, T
    {
        return _typeGroup[typeof(M)] as M;
    }

    public IEnumerable<T> Group => _typeGroup.Values;
}