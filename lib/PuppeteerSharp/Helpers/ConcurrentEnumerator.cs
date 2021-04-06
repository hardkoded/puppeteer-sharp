using System.Collections;
using System.Collections.Generic;
using System.Threading;

// https://stackoverflow.com/a/23446622

internal class ConcurrentEnumerator<T> : IEnumerator<T>
{
    #region Fields
    private readonly IEnumerator<T> _inner;
    private readonly ReaderWriterLockSlim _lock;
    #endregion

    #region Constructor
    public ConcurrentEnumerator(IEnumerable<T> inner, ReaderWriterLockSlim @lock)
    {
        this._lock = @lock;
        this._lock.EnterReadLock();
        this._inner = inner.GetEnumerator();
    }
    #endregion

    #region Methods
    public bool MoveNext()
    {
        return _inner.MoveNext();
    }

    public void Reset()
    {
        _inner.Reset();
    }

    public void Dispose()
    {
        this._lock.ExitReadLock();
        this._inner.Dispose();
    }
    #endregion

    #region Properties
    public T Current
    {
        get { return _inner.Current; }
    }

    object IEnumerator.Current
    {
        get { return _inner.Current; }
    }
    #endregion
}
