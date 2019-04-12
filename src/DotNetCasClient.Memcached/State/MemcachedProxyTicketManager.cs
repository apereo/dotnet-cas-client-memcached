/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using DotNetCasClient.Utils;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace DotNetCasClient.State
{
    ///<summary>
    /// An IProxyTicketManager implementation that relies on Memcached for proxy ticket
    /// storage.  This model allows for distributed caching of proxy tickets in order to
    /// support clustered, load balanced, or round-robin style configurations so that
    /// authentication state can be maintained across multiple servers and recycling of
    /// IIS application pools or server restarts.
    ///</summary>
    ///<author>Matt Borja, Jason Kanaris</author>
    public sealed class MemcachedProxyTicketManager : IProxyTicketManager
    {
        /// <summary>
        /// This prefix is prepended to CAS Proxy Granting Ticket IOU as the key to the cache.
        /// </summary>
        private const string CACHE_PGTIOU_KEY_PREFIX = "PGTIOU::";

        private static readonly TimeSpan DefaultExpiration = new TimeSpan(0, 0, 3, 0); // 180 seconds\

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MemcachedProxyTicketManager()
        {
        }

        /// <summary>
        /// You retrieve CasAuthentication properties in the constructor or else you will cause
        /// a StackOverflow.  CasAuthentication.Initialize() will call Initialize() on all
        /// relevant controls when its initialization is complete.  In Initialize(), you can
        /// retrieve properties from CasAuthentication.
        /// </summary>
        public void Initialize()
        {
            // Do nothing
        }

        /// <summary>
        /// Removes expired PGTIOU-PGT from the ticket store
        /// </summary>
        public void RemoveExpiredMappings()
        {
            // No-op.  The MemcachedClient removes expired entries automatically.
        }

        /// <summary>
        /// Method to save the ProxyGrantingTicket to the backing storage facility.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <param name="proxyGrantingTicket">used as the value</param>
        public void InsertProxyGrantingTicketMapping(string proxyGrantingTicketIou, string proxyGrantingTicket)
        {
            CommonUtils.AssertNotNullOrEmpty(proxyGrantingTicketIou, "proxyGrantingTicketIou parameter cannot be null or empty.");

            CommonUtils.AssertNotNullOrEmpty(proxyGrantingTicket, "proxyGrantingTicket parameter cannot be null or empty.");

            MemcachedClientManager.Instance.Store(StoreMode.Set, GetTicketKey(proxyGrantingTicketIou), proxyGrantingTicket,
                DateTime.Now.Add(DefaultExpiration));
        }

        /// <summary>
        /// Method to retrieve a ProxyGrantingTicket based on the
        /// ProxyGrantingTicketIou.  Implementations are not guaranteed to
        /// return the same result if retieve is called twice with the same
        /// proxyGrantingTicketIou.
        /// </summary>
        /// <param name="proxyGrantingTicketIou">used as the key</param>
        /// <returns>the ProxyGrantingTicket Id or null if it can't be found</returns>
        public string GetProxyGrantingTicket(string proxyGrantingTicketIou)
        {
            CommonUtils.AssertNotNullOrEmpty(proxyGrantingTicketIou, "proxyGrantingTicketIou parameter cannot be null or empty.");

            return MemcachedClientManager.Instance.Get<string>(GetTicketKey(proxyGrantingTicketIou));
        }

        /// <summary>
        /// Converts a CAS Proxy Granting Ticket IOU to its corresponding key in the ticket manager store (cache provider).
        /// </summary>
        /// <param name="proxyGrantingTicketIou">
        /// The CAS Proxy Granting Ticket IOU to convert.
        /// </param>
        /// <returns>
        /// The cache key associated with the corresponding Proxy Granting Ticket IOU
        /// </returns>
        /// <exception cref="ArgumentNullException">proxyGrantingTicketIou is null</exception>
        /// <exception cref="ArgumentException">proxyGrantingTicketIou is empty</exception>
        private static string GetTicketKey(string proxyGrantingTicketIou)
        {
            CommonUtils.AssertNotNullOrEmpty(proxyGrantingTicketIou, "proxyGrantingTicketIou parameter cannot be null or empty.");

            /*
             * Memcached keys are limited to 250 characters (https://github.com/memcached/memcached/blob/master/doc/protocol.txt).
             *
             * CAS Proxy Granting Ticket IOU's may have a length greater than that (https://apereo.github.io/cas/5.3.x/protocol/CAS-Protocol-Specification.html#341-proxy-granting-ticket-iou-properties).
             *
             * Due to the key length limitation in Memcached we must store a shorter unique key.
             *
             * A SHA512 hash of the Proxy Granting Ticket IOU will be used as the Memcached key because it will be 128 characters in length.
             */

            string ticketKey;

            using (SHA512Managed sha512Managed = new SHA512Managed())
            {
                var hash = sha512Managed.ComputeHash(Encoding.UTF8.GetBytes(CACHE_PGTIOU_KEY_PREFIX + proxyGrantingTicketIou));
                ticketKey = BitConverter.ToString(hash).Replace("-", "");
            }

            return ticketKey;
        }
    }
}
