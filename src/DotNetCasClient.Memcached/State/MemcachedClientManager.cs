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
using Enyim.Caching;

namespace DotNetCasClient.State
{
    /// <summary>
    /// Represents a singleton implementation of the MemcachedClient.
    /// </summary>
    ///<author>Jason Kanaris</author>
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
