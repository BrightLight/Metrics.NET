﻿using System;
using System.Collections.Generic;
using System.Net;
using Metrics.MetricData;
using Metrics.Reporters;
using Metrics.Visualization;

namespace Metrics.Reports
{
    public sealed class MetricsEndpointReports : Utils.IHideObjectMembers
    {
        private readonly MetricsDataProvider metricsDataProvider;
        private readonly Func<HealthStatus> healthStatus;

        private readonly List<MetricsEndpoint> endpoints = new List<MetricsEndpoint>();

        public IEnumerable<MetricsEndpoint> Endpoints => this.endpoints;

        public MetricsEndpointReports(MetricsDataProvider metricsDataProvider, Func<HealthStatus> healthStatus)
        {
            this.metricsDataProvider = metricsDataProvider;
            this.healthStatus = healthStatus;
            RegisterDefaultEndpoints();
        }

        /// <summary>
        /// Register a report at the specified endpoint.
        /// </summary>
        /// <param name="endpoint">Endpoint where the report will be accessible. E.g. "/text" </param>
        /// <param name="responseFactory">Produces the response. Will be called each time the endpoint is accessed.</param>
        /// <returns>Chain-able configuration object.</returns>
        public MetricsEndpointReports WithEndpointReport(string endpoint, Func<MetricsData, Func<HealthStatus>, MetricsEndpointRequest, MetricsEndpointResponse> responseFactory)
        {
            var metricsEndpoint = new MetricsEndpoint(endpoint, r => responseFactory(this.metricsDataProvider.CurrentMetricsData, this.healthStatus, r));
            this.endpoints.Add(metricsEndpoint);
            return this;
        }

        private void RegisterDefaultEndpoints()
        {
            this
                .WithTextReport("/text")
                .WithJsonHealthReport("/health")
                .WithJsonHealthReport("/v1/health")
                .WithJsonV1Report("/v1/json")
                .WithJsonV2Report("/v2/json")
                .WithJsonReport("/json")
                .WithPing();
        }
    }
}