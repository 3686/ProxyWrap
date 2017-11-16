using System;
using SharpReverseProxy;

namespace SharpProxy
{
    public class HistoryItem
    {
        public DateTimeOffset When { get; set; }
        public ProxyResult ProxyResult { get; set; }
    }
}