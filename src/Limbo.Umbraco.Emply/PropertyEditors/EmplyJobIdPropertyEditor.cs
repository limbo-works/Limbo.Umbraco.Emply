using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.Integer)]
public class EmplyJobIdPropertyEditor : DataEditor {

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Emply.JobId";

    #endregion

    #region Constructors

    public EmplyJobIdPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}