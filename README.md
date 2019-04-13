# Memcached backed Proxy/Service Ticket Managers for the Apereo .NET CAS Client #

[![Build status](https://ci.appveyor.com/api/projects/status/rg6pgx727ut243rl?svg=true)](https://ci.appveyor.com/project/mmoayyed/dotnet-cas-client-memcached/branch/master)
[![Stable nuget](https://img.shields.io/nuget/v/DotNetCasClient.Memcached.svg?label=stable%20nuget)](https://www.nuget.org/packages/DotNetCasClient.Memcached/)
[![Pre-release nuget](https://img.shields.io/myget/dotnetcasclient-prerelease/vpre/dotnetcasclient.memcached.svg?label=pre-release%20nuget)](https://www.myget.org/feed/dotnetcasclient-prerelease/package/nuget/DotNetCasClient.Memcached)
[![Unstable nuget](https://img.shields.io/myget/dotnetcasclient-ci/vpre/dotnetcasclient.memcached.svg?label=unstable%20nuget)](https://www.myget.org/feed/dotnetcasclient-ci/package/nuget/DotNetCasClient.Memcached)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

[![Gitter](https://img.shields.io/gitter/room/apereo/cas.svg)](https://gitter.im/apereo/dotnet-cas-client)
[![Stack Overflow](https://img.shields.io/badge/stackoverflow-cas%20%2B%20.net-orange.svg)](https://stackoverflow.com/questions/tagged/cas%2b.net)

## Introduction ##

This project is an add-on to the Apereo .NET CAS Client that implements the proxy and service ticket managers backed by a Memcached data store.

By storing your proxy and service tickets in a centralized data store your applications running in a distributed, clustered or load balanced environment will all have access to the same proxy and service ticket data.  This is not possible to achieve with the default in-memory proxy and service ticket managers that ships with the Apereo .NET CAS Client.

## Configuration ##

You will need to make modifications to your application's web.config file after installing this NuGet package.

### First Modification: ###

The first modification will be to add the pertinent Memcached configuration elements.  This project has a dependency on [EnyimMemcached](https://github.com/enyim/EnyimMemcached) in order to integrate with Memcached.  Please see the [EnyimMemcached configuration documentation](https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Configuration) with regards to which pertinent elements need to be added to the web.config file.

### Second Modification: ###

The second modification will be to modify the `<casClientConfig>` xml element in your web.config file.  Specifically we will be changing the **proxyTicketManager** *(if you use that)* and the **serviceTicketManager** XML attribute values.

Set the **serviceTicketManager** attribute value to: `DotNetCasClient.State.MemcachedServiceTicketManager, DotNetCasClient.Memcached`

Set the **proxyTicketManager** attribute value to: `DotNetCasClient.State.MemcachedProxyTicketManager, DotNetCasClient.Memcached`

Also, don't forget to wire-up the rest of the [.NET Cas Client configuration](https://github.com/apereo/dotnet-cas-client/wiki/Getting-Started#integration-instructions) too.

After all that configuration you should be good to go!