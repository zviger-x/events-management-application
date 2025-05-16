using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Extensions
{
    public static class AggregateFluentExtensions
    {
        public static IAggregateFluent<TDocument> Lookup<
            TDocument, TForeignDocument, TDocumentField, TForeignDocumentField, TDocumentFieldResult>(
            this IAggregateFluent<TDocument> aggregateFluent,
            string foreignCollectionName,
            Expression<Func<TDocument, TDocumentField>> localField,
            Expression<Func<TForeignDocument, TForeignDocumentField>> foreignField,
            Expression<Func<TDocument, TDocumentFieldResult>> @as)
        {
            return aggregateFluent.Lookup<TForeignDocument, TDocument>(
                foreignCollectionName: foreignCollectionName,
                localField: new ExpressionFieldDefinition<TDocument, TDocumentField>(localField),
                foreignField: new ExpressionFieldDefinition<TForeignDocument, TForeignDocumentField>(foreignField),
                @as: new ExpressionFieldDefinition<TDocument, TDocumentFieldResult>(@as));
        }
    }
}
