﻿using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Enums;

#pragma warning disable 1591

namespace Limbo.Umbraco.Emply.Models.Import;

[JsonConverter(typeof(EnumStringConverter))]
public enum ImportAction {
    None,
    NotModified,
    Added,
    Updated,
    Deleted,
    Rejected
}