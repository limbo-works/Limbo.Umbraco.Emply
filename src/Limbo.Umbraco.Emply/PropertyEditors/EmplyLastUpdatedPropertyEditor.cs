using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.String)]
public class EmplyLastUpdatedPropertyEditor : DataEditor {

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Emply.LastUpdated";

    public const string EditorUiAlias = "Limbo.Umbraco.Emply.PropertyEditorUi.LastUpdated";

    #endregion

    #region Constructors

    public EmplyLastUpdatedPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}