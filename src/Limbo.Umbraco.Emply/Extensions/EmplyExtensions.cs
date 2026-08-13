using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Limbo.Integrations.Emply.Models.Jobs;
using Limbo.Integrations.Emply.Models.Postings;
using Skybrud.Essentials.Time;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.Extensions;

public static class EmplyExtensions {

    internal static void Add(this List<IndexValue> list, string key, long value, string? culture = null) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(new IndexValue { FieldName = key, Values = [value], Culture = culture });
    }

    internal static void Add(this List<IndexValue> list, string key, string value, string? culture = null) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(new IndexValue { FieldName = key, Values = [value], Culture = culture });
    }

    internal static void Add(this List<IndexValue> list, string key, DateTimeOffset value, string? culture = null) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(new IndexValue { FieldName = key, Values = [value.ToString("yyyyMMddHHmmss000", CultureInfo.InvariantCulture)], Culture = culture });
    }

    internal static void Add(this List<IndexValue> list, string key, EssentialsTime value, string? culture = null) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(new IndexValue { FieldName = key, Values = [value.DateTimeOffset.ToString("yyyyMMddHHmmss000", CultureInfo.InvariantCulture)], Culture = culture });
    }

    public static bool TryGetData(this EmplyPosting posting, Func<EmplyJobData, bool> predicate, [NotNullWhen(true)] out EmplyJobData? result) {
        // TODO: consider moving to the "Limbo.Integrations.Emply" package
        result = posting.Data.FirstOrDefault(predicate);
        return result != null;
    }

    public static bool TryGetData<T>(this EmplyPosting posting, Func<T, bool> predicate, [NotNullWhen(true)] out T? result) where T : EmplyJobData {
        // TODO: consider moving to the "Limbo.Integrations.Emply" package
        result = posting.Data.OfType<T>().FirstOrDefault(predicate);
        return result != null;
    }

}