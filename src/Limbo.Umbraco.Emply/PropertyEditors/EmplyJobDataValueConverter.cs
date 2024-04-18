using System;
using Limbo.Integrations.Emply.Models.Postings;
using Limbo.Umbraco.Emply.Factories;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

public class EmplyJobDataValueConverter : PropertyValueConverterBase {

    private readonly EmplyModelFactory _modelFactory;

    public EmplyJobDataValueConverter(EmplyModelFactory modelFactory) {
        _modelFactory = modelFactory;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == EmplyJobDataEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source is not string json || !json.StartsWith("_{") ? null : JsonUtils.ParseJsonObject(json[1..]);
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        if (inter is not JObject json) return null;
        return _modelFactory.ConvertJobData(owner, propertyType, EmplyPosting.Parse(json));
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return _modelFactory.GetJobDataValueType(propertyType);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}