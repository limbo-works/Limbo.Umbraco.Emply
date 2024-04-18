using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Integer, Group = "Limbo", Icon = EditorIcon)]
public class EmplyJobIdEditor : DataEditor {

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Emply.JobId";

    internal const string EditorName = "Limbo Emply Job ID";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Emply/Views/JobId.html";

    internal const string EditorIcon = "icon-limbo-emply color-limbo";

    #endregion

    #region Constructors

    public EmplyJobIdEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}