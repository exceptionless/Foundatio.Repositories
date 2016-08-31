﻿using System;
using Nest;

namespace Foundatio.Repositories.Elasticsearch.Queries.Builders {
    public interface IChildQuery {
        ITypeQuery ChildQuery { get; set; }
    }

    public class ChildQueryBuilder : IElasticQueryBuilder {
        private readonly ElasticQueryBuilder _queryBuilder;

        public ChildQueryBuilder(ElasticQueryBuilder queryBuilder) {
            _queryBuilder = queryBuilder;
        }

        public void Build<T>(QueryBuilderContext<T> ctx) where T : class, new() {
            var childQuery = ctx.GetSourceAs<IChildQuery>();
            if (childQuery?.ChildQuery == null)
                return;

            if (String.IsNullOrEmpty(childQuery.ChildQuery.Type))
                throw new ArgumentException("Must specify a child type for child queries.");

            var childContext = new QueryBuilderContext<T>(childQuery.ChildQuery, ctx.Options);
            _queryBuilder.Build(childContext);

            if ((childContext.Query == null || childContext.Query.IsConditionless)
                && (childContext.Filter == null || childContext.Filter.IsConditionless))
                return;

            ctx.Filter &= new HasChildFilter {
                Query = childContext.Query,
                Filter = childContext.Filter,
                Type = childQuery.ChildQuery.Type
            };
        }
    }

    public static class ChildQueryExtensions {
        public static TQuery WithChildQuery<TQuery, TChildQuery>(this TQuery query, Func<TChildQuery, TChildQuery> childQueryFunc) where TQuery : IChildQuery where TChildQuery : class, ITypeQuery, new() {
            if (childQueryFunc == null)
                throw new ArgumentNullException(nameof(childQueryFunc));

            var childQuery = query.ChildQuery as TChildQuery ?? new TChildQuery();
            query.ChildQuery = childQueryFunc(childQuery);

            return query;
        }

        public static Query WithChildQuery<T>(this Query query, Func<T, T> childQueryFunc) where T : class, ITypeQuery, new() {
            if (childQueryFunc == null)
                throw new ArgumentNullException(nameof(childQueryFunc));

            var childQuery = query.ChildQuery as T ?? new T();
            query.ChildQuery = childQueryFunc(childQuery);

            return query;
        }

        public static ElasticQuery WithChildQuery<T>(this ElasticQuery query, Func<T, T> childQueryFunc) where T : class, ITypeQuery, new() {
            if (childQueryFunc == null)
                throw new ArgumentNullException(nameof(childQueryFunc));

            var childQuery = query.ChildQuery as T ?? new T();
            query.ChildQuery = childQueryFunc(childQuery);

            return query;
        }

        public static ElasticQuery WithChildQuery(this ElasticQuery query, Func<ChildQuery, ChildQuery> childQueryFunc) {
            if (childQueryFunc == null)
                throw new ArgumentNullException(nameof(childQueryFunc));

            var childQuery = query.ChildQuery as ChildQuery ?? new ChildQuery();
            query.ChildQuery = childQueryFunc(childQuery);

            return query;
        }
    }
}