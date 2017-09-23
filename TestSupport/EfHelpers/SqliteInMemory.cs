﻿// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace TestSupport.EfHelpers
{
    public static class SqliteInMemory
    {
        /// <summary>
        /// Created a Sqlite Options for in-memory database. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="throwOnClientServerWarning">Optional: if set to true then will throw exception if QueryClientEvaluationWarning is logged</param>
        /// <returns></returns>
        public static DbContextOptions<T> CreateOptions<T>(bool throwOnClientServerWarning = false) where T : DbContext
        {
            //Thanks to https://www.scottbrady91.com/Entity-Framework/Entity-Framework-Core-In-Memory-Testing
            var connectionStringBuilder =
                new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();                //see https://github.com/aspnet/EntityFramework/issues/6968

            // create in-memory context
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite(connection)
                .EnableSensitiveDataLogging();  //You get more information with this turned on.
            if (throwOnClientServerWarning)
            {
                //This will throw an exception of a QueryClientEvaluationWarning is logged
                builder.ConfigureWarnings(warnings =>
                    warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
            }

            return builder.Options;
        }

    }
}