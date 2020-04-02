﻿using System;
using System.Data.Common;
using System.Reflection;
using Hangfire.Storage;

namespace Hangfire.Tags.PostgreSql
{
    public class PostgreSqlTagsMonitoringApi
    {
        private readonly IMonitoringApi _monitoringApi;

        private static Type _type;

        private static MethodInfo _useConnection;

        public PostgreSqlTagsMonitoringApi(IMonitoringApi monitoringApi)
        {
            if (monitoringApi.GetType().Name != "PostgreSqlMonitoringApi")
            {
                throw new ArgumentException("The monitor API is not implemented using PostgreSql", nameof(monitoringApi));
            }

            _monitoringApi = monitoringApi;

            if (_useConnection == null)
                _useConnection = monitoringApi.GetType().GetTypeInfo().GetMethod(nameof(UseConnection),
                    BindingFlags.NonPublic | BindingFlags.Instance);

            if (_useConnection == null)
                throw new ArgumentException("The function UseConnection cannot be found.");
        }

        public T UseConnection<T>(Func<DbConnection, T> action)
        {
            var method = _useConnection.MakeGenericMethod(typeof(T));
            return (T)method.Invoke(_monitoringApi, new object[] { action });
        }
    }
}
