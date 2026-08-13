using Limbo.Umbraco.Emply.Factories;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class EmplyJobDataPropertyEditor : DataEditor {

    private readonly EmplyJobDataPropertyIndexValueFactory _indexValueFactory;

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Emply.JobData";

    #endregion

    #region Constructors

    public EmplyJobDataPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, EmplyJobDataPropertyIndexValueFactory indexValueFactory) : base(dataValueEditorFactory) {
        _indexValueFactory = indexValueFactory;
    }

    #endregion

    #region Member methods

    public override IPropertyIndexValueFactory PropertyIndexValueFactory => _indexValueFactory;

    #endregion

}