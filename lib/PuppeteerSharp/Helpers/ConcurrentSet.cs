using System.Collections.Concurrent;

internal class ConcurrentSet<T> : ConcurrentDictionary<T, bool>
{
    public bool Add(T item)
    {
        return TryAdd(item, true);
    }

    public bool Contains(T item)
    {
        return ContainsKey(item);
    }

    public bool Remove(T item)
    {
        return TryRemove(item, out var _);
    }
}
