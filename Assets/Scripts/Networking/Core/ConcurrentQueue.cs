using System.Collections.Generic;

public class ConcurrentQueue<T> {
    private readonly Queue<T> _internalQueue = new Queue<T>();

    private static readonly object LockObject = new object();

    public int Count {
        get {
            lock (LockObject) {
                return _internalQueue.Count;
            }
        }
    }

    public bool TryDequeue(out T result) {
        lock (LockObject) {
            if (_internalQueue.Count > 0) {
                result = _internalQueue.Dequeue();
                return true;
            }
            result = default(T);
            return false;
        }
    }

    public void Enqueue(T item) {
        lock (LockObject) {
            _internalQueue.Enqueue(item);
        }
    }

    public void Clear() {
        lock (LockObject) {
            _internalQueue.Clear();
        }
    }
}