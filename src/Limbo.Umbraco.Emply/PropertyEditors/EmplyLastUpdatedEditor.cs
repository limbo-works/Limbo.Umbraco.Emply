using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.String)]
public class EmplyLastUpdatedEditor : DataEditor {

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Emply.LastUpdated";

    #endregion

    #region Constructors

    public EmplyLastUpdatedEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}