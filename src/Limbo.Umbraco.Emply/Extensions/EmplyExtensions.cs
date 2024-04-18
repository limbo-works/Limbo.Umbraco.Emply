using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Limbo.Integrations.Emply.Models.Jobs;
using Limbo.Integrations.Emply.Models.Postings;
using Microsoft.Extensions.Configuration;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Essentials.Time;

namespace Limbo.Umbraco.Emply.Extensions;

public static class EmplyExtensions {

    public static void Add(this List<KeyValuePair<string, IEnumerable<object?>>> list, string key, long value) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(new KeyValuePair<string, IEnumerable<object?>>(key, new object[] { value }));
    }

    public static void Add(this List<KeyValuePair<string, IEnumerable<object?>>> list, string key, string value) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(new KeyValuePair<string, IEnumerable<object?>>(key, new object[] { value }));
    }

    public static void Add(this List<KeyValuePair<string, IEnumerable<object?>>> list, string key, DateTimeOffset value) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(key, value.ToString("yyyyMMddHHmmss000", CultureInfo.InvariantCulture));
    }

    public static void Add(this List<KeyValuePair<string, IEnumerable<object?>>> list, string key, EssentialsTime value) {
        // TODO: consider moving to the "Skybrud.Essentials.Umbraco" package
        list.Add(key, value.DateTimeOffset.ToString("yyyyMMddHHmmss000", CultureInfo.InvariantCulture));
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

    public static bool GetBoolean(this IConfiguration configuration, string key) {
        // TODO: consider moving to the "Skybrud.Essentials.AspNetCore" package
        return (configuration.GetSection(key)?.Value).ToBoolean();
    }

    public static bool GetBoolean(this IConfiguration configuration, string key, bool fallback) {
        // TODO: consider moving to the "Skybrud.Essentials.AspNetCore" package
        return (configuration.GetSection(key)?.Value).ToBoolean(fallback);
    }

    public static bool? GetBooleanOrNull(this IConfiguration configuration, string key) {
        // TODO: consider moving to the "Skybrud.Essentials.AspNetCore" package
        return (configuration.GetSection(key)?.Value).ToBooleanOrNull();
    }

}