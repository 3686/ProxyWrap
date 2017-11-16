using System.Collections.Concurrent;

namespace SharpProxy
{
    public class History
    {
        private const int Limit = 2;
        private readonly ConcurrentStack<HistoryItem> _internal;

        public History()
        {
            _internal = new ConcurrentStack<HistoryItem>();
        }

        public void Add(HistoryItem historyItem)
        {
            while (_internal.Count > Limit)
            {
                _internal.TryPop(out _);
            }
            _internal.Push(historyItem);
        }

        public HistoryItem[] Get()
        {
            return _internal.ToArray();
        }
    }
}