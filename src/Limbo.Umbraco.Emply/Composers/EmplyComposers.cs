using System;
using Limbo.Umbraco.Emply.Extensions;
using Limbo.Umbraco.Emply.Factories;
using Limbo.Umbraco.Emply.Models.Settings;
using Limbo.Umbraco.Emply.Scheduling;
using Limbo.Umbraco.Emply.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Strings;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Essentials.Time.Iso8601;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.Emply.Composers;

/// <inheritdoc />
public class EmplyComposer : IComposer {

    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder) {

        builder.Services.AddSingleton<EmplyJobsService>();
        builder.Services.AddSingleton<EmplyJobDataPropertyIndexValueFactory>();
        builder.Services.AddOptions<EmplySettings>().Configure<IConfiguration>(ConfigureEmply);

        builder.ManifestFilters().Append<EmplyManifestFilter>();

        if (builder.Config.GetBoolean("Limbo:Emply:Scheduling:Enabled", true)) {
            builder.Services.AddHostedService<EmplyRecurringTask>();
        }

    }

    private static void ConfigureEmply(EmplySettings settings, IConfiguration configuration) {

        IConfigurationSection section = configuration.GetSection("Limbo:Emply");

        settings.ImportUserId = StringUtils.ParseInt32(section.GetSection("ImportUserId")?.Value, settings.ImportUserId);
        settings.LogResults = StringUtils.ParseBoolean(section.GetSection("LogResults")?.Value, settings.LogResults);

        ParseFeeds(section, settings);
        ParseScheduling(section, settings);

    }

    private static void ParseFeeds(IConfiguration section, EmplySettings settings) {

        IConfigurationSection? feeds = section.GetSection("Feeds");
        if (feeds == null) return;

        foreach (IConfigurationSection child in feeds.GetChildren()) {

            // Read from properties from their respective child sections
            string? customerName = child.GetSection("CustomerName")?.Value;
            string? apiKey = child.GetSection("ApiKey")?.Value;
            Guid? parentContentKey = child.GetSection("ParentContentKey")?.Value.ToGuid();
            string? contentTypeAlias = child.GetSection("ContentTypeAlias")?.Value;

            // Validate required properties
            if (string.IsNullOrWhiteSpace(customerName)) throw new Exception("Feed does not specify a customer name.");
            if (parentContentKey is null || parentContentKey == Guid.Empty) throw new Exception("Feed does not specify a valid 'ParentContentKey' value.");
            if (string.IsNullOrWhiteSpace(contentTypeAlias)) throw new Exception("Feed does not specify a 'ContentTypeAlias' value.");

            // Append the item to the list
            settings.Sources.Add(new EmplySourceSettings(customerName, apiKey, parentContentKey.Value, contentTypeAlias));

        }

    }

    private static void ParseScheduling(IConfiguration section, EmplySettings settings) {

        IConfigurationSection? scheduling = section.GetSection("Scheduling");
        if (scheduling == null) return;

        settings.Scheduling.Enabled = (section.GetSection("Enabled")?.Value).ToBoolean(true);

        string? delay = scheduling.GetSection("Delay")?.Value;
        string? interval = scheduling.GetSection("Interval")?.Value;

        if (int.TryParse(delay, out int delayMinutes)) {
            settings.Scheduling.Delay = TimeSpan.FromMinutes(delayMinutes);
        } else if (Iso8601Utils.TryParseDuration(delay, out TimeSpan delayTimeSpan)) {
            settings.Scheduling.Delay = delayTimeSpan;
        }

        if (int.TryParse(interval, out int internalMinutes)) {
            settings.Scheduling.Interval = TimeSpan.FromMinutes(internalMinutes);
        } else if (Iso8601Utils.TryParseDuration(interval, out TimeSpan internalTimeSpan)) {
            settings.Scheduling.Interval = internalTimeSpan;
        }

    }

}