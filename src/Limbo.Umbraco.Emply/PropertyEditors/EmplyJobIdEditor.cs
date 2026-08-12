using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.Integer)]
public class EmplyJobIdEditor : DataEditor {

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Emply.JobId";

    #endregion

    #region Constructors

    public EmplyJobIdEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}