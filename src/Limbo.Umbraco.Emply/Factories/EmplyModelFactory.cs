using System;
using Limbo.Integrations.Emply.Models.Postings;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.Emply.Factories;

public class EmplyModelFactory {

    public virtual Type GetJobDataValueType(IPublishedPropertyType propertyType) {
        return typeof(EmplyPosting);
    }

    public virtual object ConvertJobData(IPublishedElement owner, IPublishedPropertyType propertyType, EmplyPosting item) {
        return item;
    }

}