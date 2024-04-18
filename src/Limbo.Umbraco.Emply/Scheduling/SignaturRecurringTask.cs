using System.Text;
using System.Threading.Tasks;
using Limbo.Umbraco.Emply.Models.Import;
using Limbo.Umbraco.Emply.Models.Settings;
using Limbo.Umbraco.Emply.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Time;
using Skybrud.Essentials.Umbraco.Scheduling;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace Limbo.Umbraco.Emply.Scheduling;

public class EmplyRecurringTask : RecurringHostedServiceBase {

    private readonly EmplySettings _settings;
    private readonly EmplyJobsService _emplyJobsService;
    private readonly TaskHelper _taskHelper;

    public EmplyRecurringTask(ILogger<EmplyRecurringTask> logger, IOptions<EmplySettings> settings, EmplyJobsService emplyJobsService, TaskHelper taskHelper) : base(logger, settings.Value.Scheduling.Interval, settings.Value.Scheduling.Delay) {
        _settings = settings.Value;
        _emplyJobsService = emplyJobsService;
        _taskHelper = taskHelper;
    }

    public override Task PerformExecuteAsync(object? state) {

        // Don't do anything if the site is not running.
        if (_taskHelper.RuntimeLevel != RuntimeLevel.Run) return Task.CompletedTask;

        // TODO: verify that the task should actually run on this server/application

        StringBuilder sb = new();
        sb.AppendLine(EssentialsTime.Now.Iso8601);

        foreach (EmplySourceSettings source in _settings.Sources) {

            // Write a bit to the log
            sb.AppendLine($"> Starting import for customer '{source.CustomerName}'...");

            // Run a new import
            EmplyImportResult result = _emplyJobsService.Import(source, true);

            // Save the result to the disk
            if (_settings.LogResults) _emplyJobsService.WriteToLog(result);

            // Write a bit to the log
            sb.AppendLine($"> Import finished with status {result.Status}.");
            _taskHelper.AppendToLog(this, sb);

        }

        // Make sure we save that the job has run
        _taskHelper.SetLastRunTime(this);

        return Task.CompletedTask;

    }

}