namespace TrackingService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.Metadata;
    using NHibernate.Stat;

    internal class SingleConnectionSessionFactory :
        ISessionFactory
    {
        private readonly ISessionFactory inner;

        private readonly IDbConnection liveConnection;

        public SingleConnectionSessionFactory(ISessionFactory inner, IDbConnection liveConnection)
        {
            this.inner = inner;
            this.liveConnection = liveConnection;
        }

        public void Dispose()
        {
            this.inner.Dispose();
        }

        public ISession OpenSession(IDbConnection conn)
        {
            return this.inner.OpenSession(this.liveConnection);
        }

        public ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            return this.inner.OpenSession(this.liveConnection, sessionLocalInterceptor);
        }

        public ISession OpenSession(IDbConnection conn, IInterceptor sessionLocalInterceptor)
        {
            return this.inner.OpenSession(this.liveConnection, sessionLocalInterceptor);
        }

        public ISession OpenSession()
        {
            return this.inner.OpenSession(this.liveConnection);
        }

        public IClassMetadata GetClassMetadata(Type persistentClass)
        {
            return this.inner.GetClassMetadata(persistentClass);
        }

        public IClassMetadata GetClassMetadata(string entityName)
        {
            return this.inner.GetClassMetadata(entityName);
        }

        public ICollectionMetadata GetCollectionMetadata(string roleName)
        {
            return this.inner.GetCollectionMetadata(roleName);
        }

        public IDictionary<string, IClassMetadata> GetAllClassMetadata()
        {
            return this.inner.GetAllClassMetadata();
        }

        public IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata()
        {
            return this.inner.GetAllCollectionMetadata();
        }

        public void Close()
        {
            this.inner.Close();
        }

        public void Evict(Type persistentClass)
        {
            this.inner.Evict(persistentClass);
        }

        public void Evict(Type persistentClass, object id)
        {
            this.inner.Evict(persistentClass, id);
        }

        public void EvictEntity(string entityName)
        {
            this.inner.EvictEntity(entityName);
        }

        public void EvictEntity(string entityName, object id)
        {
            this.inner.EvictEntity(entityName, id);
        }

        public void EvictCollection(string roleName)
        {
            this.inner.EvictCollection(roleName);
        }

        public void EvictCollection(string roleName, object id)
        {
            this.inner.EvictCollection(roleName, id);
        }

        public void EvictQueries()
        {
            this.inner.EvictQueries();
        }

        public void EvictQueries(string cacheRegion)
        {
            this.inner.EvictQueries(cacheRegion);
        }

        public IStatelessSession OpenStatelessSession()
        {
            return this.inner.OpenStatelessSession();
        }

        public IStatelessSession OpenStatelessSession(IDbConnection connection)
        {
            return this.inner.OpenStatelessSession(connection);
        }

        public FilterDefinition GetFilterDefinition(string filterName)
        {
            return this.inner.GetFilterDefinition(filterName);
        }

        public ISession GetCurrentSession()
        {
            return this.inner.GetCurrentSession();
        }

        public IStatistics Statistics
        {
            get { return this.inner.Statistics; }
        }

        public bool IsClosed
        {
            get { return this.inner.IsClosed; }
        }

        public ICollection<string> DefinedFilterNames
        {
            get { return this.inner.DefinedFilterNames; }
        }
    }
}