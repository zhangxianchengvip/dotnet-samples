using System;
using System.Collections.Generic;
using System.Threading;
using global::Yarp.ReverseProxy.Configuration;
using Microsoft.Extensions.Primitives;

namespace YaprService
{
    public sealed class InMemoryConfigProvider : IProxyConfigProvider
    {
        // Marked as volatile so that updates are atomic
        private volatile InMemoryConfig _config;

        public InMemoryConfigProvider(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
            : this(routes, clusters, Guid.NewGuid().ToString())
        { }

        public InMemoryConfigProvider(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters, string revisionId)
        {
            _config = new InMemoryConfig(routes, clusters, revisionId);
        }

        public IProxyConfig GetConfig() => _config;

        public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            var newConfig = new InMemoryConfig(routes, clusters);
            UpdateInternal(newConfig);
        }


        public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters, string revisionId)
        {
            var newConfig = new InMemoryConfig(routes, clusters, revisionId);
            UpdateInternal(newConfig);
        }

        private void UpdateInternal(InMemoryConfig newConfig)
        {
            var oldConfig = Interlocked.Exchange(ref _config, newConfig);
            oldConfig.SignalChange();
        }

        private class InMemoryConfig : IProxyConfig
        {

            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
                : this(routes, clusters, Guid.NewGuid().ToString())
            { }

            public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters, string revisionId)
            {
                RevisionId = revisionId ?? throw new ArgumentNullException(nameof(revisionId));
                Routes = routes;
                Clusters = clusters;
                ChangeToken = new CancellationChangeToken(_cts.Token);
            }

            /// <inheritdoc/>
            public string RevisionId { get; }


            public IReadOnlyList<RouteConfig> Routes { get; }

            public IReadOnlyList<ClusterConfig> Clusters { get; }

            public IChangeToken ChangeToken { get; }

            internal void SignalChange()
            {
                _cts.Cancel();
            }
        }
    }
}
