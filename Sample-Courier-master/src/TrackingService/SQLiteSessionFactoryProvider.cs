namespace TrackingService
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using MassTransit.NHibernateIntegration;
    using NHibernate;
    using NHibernate.Cache;
    using NHibernate.Cfg;
    using NHibernate.Cfg.Loquacious;
    using NHibernate.Dialect;
    using NHibernate.Tool.hbm2ddl;


    /// <summary>
    /// Creates a session factory that works with SQLite, by default in memory, for testing purposes
    /// </summary>
    internal class SqLiteSessionFactoryProvider :
        NHibernateSessionFactoryProvider,
        IDisposable
    {
        private const string InMemoryConnectionString = "Data Source=:memory:;Version=3;New=True;Pooling=True;Max Pool Size=1;";

        private bool disposed;

        private ISessionFactory innerSessionFactory;

        private SQLiteConnection openConnection;

        private SingleConnectionSessionFactory sessionFactory;

        public SqLiteSessionFactoryProvider(string connectionString, params Type[] mappedTypes)
            : base(mappedTypes, x => Integrate(x, connectionString, false))
        {
        }

        public SqLiteSessionFactoryProvider(params Type[] mappedTypes)
            : this(false, mappedTypes)
        {
        }

        public SqLiteSessionFactoryProvider(bool logToConsole, params Type[] mappedTypes)
            : base(mappedTypes, x => Integrate(x, null, logToConsole))
        {
            this.Configuration.SetProperty(NHibernate.Cfg.Environment.UseSecondLevelCache, "true");
            this.Configuration.SetProperty(NHibernate.Cfg.Environment.UseQueryCache, "true");
            this.Configuration.SetProperty(NHibernate.Cfg.Environment.CacheProvider,
                typeof(HashtableCacheProvider).AssemblyQualifiedName);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SqLiteSessionFactoryProvider()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {
                if (this.openConnection != null)
                {
                    this.openConnection.Close();
                    this.openConnection.Dispose();
                }
            }

            this.disposed = true;
        }

        public override ISessionFactory GetSessionFactory()
        {
            var connectionString = this.Configuration.Properties[NHibernate.Cfg.Environment.ConnectionString];
            this.openConnection = new SQLiteConnection(connectionString);
            this.openConnection.Open();

            BuildSchema(this.Configuration, this.openConnection);

            this.innerSessionFactory = base.GetSessionFactory();
            this.innerSessionFactory.OpenSession(this.openConnection);

            this.sessionFactory = new SingleConnectionSessionFactory(this.innerSessionFactory, this.openConnection);

            return this.sessionFactory;
        }

        private static void BuildSchema(Configuration config, IDbConnection connection)
        {
            new SchemaExport(config).Execute(true, true, false, connection, null);
        }

        private static void Integrate(IDbIntegrationConfigurationProperties db, string connectionString, bool logToConsole)
        {
            db.Dialect<SQLiteDialect>(); //This is a custom dialect

            db.ConnectionString = connectionString ?? InMemoryConnectionString;
            db.BatchSize = 100;
            db.IsolationLevel = IsolationLevel.Serializable;
            db.LogSqlInConsole = logToConsole;
            db.LogFormattedSql = logToConsole;
            db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;

            // Do not use this property with real DB as it will modify schema
            db.SchemaAction = SchemaAutoAction.Update;

            //Disable comments until this issue is resolved
            // https://groups.google.com/forum/?fromgroups=#!topic/nhusers/xJ675yG2uhY
            //properties.AutoCommentSql = true;
        }
    }
}