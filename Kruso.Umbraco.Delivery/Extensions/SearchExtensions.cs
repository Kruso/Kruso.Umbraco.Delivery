using Examine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class SearchExtensions
    {
        public static IBooleanOperation FilterByDocumentTypes(this IQuery query, params string[] documentTypes)
        {
            return query.GroupedOr(new[] { "__NodeTypeAlias" }, documentTypes);
        }

        public static IBooleanOperation FilterByRoles(this IQuery query, string[] roles)
        {
            return query.GroupedOr(new[] { "Roles" }, roles);
        }

        public static IBooleanOperation SearchByLanguageFields(this IQuery query, string culture, string freeText, params string[] fields)
        {
            var languageFields = fields.Select(x => $"{x}_{culture.ToLower()}");
            return query.ManagedQuery(freeText, languageFields.ToArray());
        }

        public static IBooleanOperation FieldNot(this IQuery query, string field, string value)
        {
            return query.GroupedNot(new[] { field }, value);
        }

        public static IBooleanOperation FieldByLanguage(this IQuery query, string field, string culture, string value)
        {
            return query.Field($"{field}_{culture}", value);
        }

        public static IBooleanOperation MatchByLanguage(this IQuery query, string culture, string freeText, params string[] fields)
        {
            var languageFields = fields.Select(x => $"{x}_{culture}");
            return query.ManagedQuery(freeText, languageFields.ToArray());
        }
    }
}
