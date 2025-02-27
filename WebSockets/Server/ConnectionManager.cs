using System.Collections.Concurrent;

namespace Server
{
    internal class ConnectionManager
    {
        private readonly ConcurrentDictionary<string, ClientConnection> _connections = new ConcurrentDictionary<string, ClientConnection>();

        public void AddConnection(ClientConnection connection)
        {
            _connections.TryAdd(connection.GetHashCode().ToString(), connection);
        }

        public void RemoveConnection(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);
        }
    }
}
