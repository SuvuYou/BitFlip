using System;
using System.Collections.Generic;
using System.Linq;

public interface ISnapshotable<T>
{
    public abstract T TakeSnapshot();
}

public class SnapshotManager<T>
{
    public readonly Stack<T> SnapshotHistory = new();

    private ISnapshotable<T> _snapshotable;

    private List<T> _snapshotList = new();
    private int _currentReadingIndex = 0;
    private int _maxReadingIndex, _minReadingIndex;

    public SnapshotManager(ISnapshotable<T> snapshotable)
    {
        _snapshotable = snapshotable;
    }

    public void Snapshot()
    {
        SnapshotHistory.Push(_snapshotable.TakeSnapshot());
    }

    public void Read()
    {
        _snapshotList = SnapshotHistory.ToList();
        _snapshotList.Reverse(); 

        _maxReadingIndex = _snapshotList.Count() - 1;
        _minReadingIndex = 0;
    }

    public T Next() 
    {
        _currentReadingIndex = Math.Clamp(++_currentReadingIndex, _minReadingIndex, _maxReadingIndex);

        return _snapshotList[_currentReadingIndex];
    }

    public T Prev() 
    {
        _currentReadingIndex = Math.Clamp(--_currentReadingIndex, _minReadingIndex, _maxReadingIndex);

        return _snapshotList[_currentReadingIndex];
    }
}