using Newtonsoft.Json;

#pragma warning disable 1591

namespace Limbo.Umbraco.Emply.Models.Import;

public class EmplyImportResult : ImportTask {

    [JsonProperty("type", Order = -999)]
    public static string Type => "Job";

    public EmplyImportResult() { }

    public EmplyImportResult(string name) {
        Name = name;
    }

}