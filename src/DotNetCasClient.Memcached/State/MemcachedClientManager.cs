using System;
using Enyim.Caching;

namespace DotNetCasClient.State
{
    /// <summary>
    /// Represents a singleton implementation of the MemcachedClient.
    /// </summary>
    public class MemcachedClientManager
    {
        private static readonly Lazy<MemcachedClient> lazy = new Lazy<MemcachedClient>(() => new MemcachedClient());

        /// <summary>
        /// Gets the instance of MemcachedClient.
        /// </summary>
        public static MemcachedClient Instance { get { return lazy.Value; } }

        private MemcachedClientManager()
        {
        }
    }
}
