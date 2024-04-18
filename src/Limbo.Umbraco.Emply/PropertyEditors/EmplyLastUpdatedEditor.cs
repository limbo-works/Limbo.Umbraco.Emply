using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.String, Group = "Limbo", Icon = EditorIcon)]
public class EmplyLastUpdatedEditor : DataEditor {

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Emply.LastUpdated";

    internal const string EditorName = "Limbo Emply Last Updated";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Emply/Views/Timestamp.html";

    internal const string EditorIcon = "icon-limbo-emply color-limbo";

    #endregion

    #region Constructors

    public EmplyLastUpdatedEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}