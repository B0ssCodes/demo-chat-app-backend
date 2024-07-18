using System.Collections.Concurrent;
using ChatApp.Models;

namespace ChatApp.DataService
{
    public class MemoryDb
    {
        // Parallel variant of the dictionary for async requests
        private readonly ConcurrentDictionary<string, UserConnection> 
            _connections = new ConcurrentDictionary<string, UserConnection>();

        // Get public access to the readonly dictionary
        public ConcurrentDictionary<string, UserConnection> connections => _connections;


    }
}
