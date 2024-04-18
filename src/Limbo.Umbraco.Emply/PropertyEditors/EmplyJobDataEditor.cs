using Limbo.Umbraco.Emply.Factories;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Emply.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
public class EmplyJobDataEditor : DataEditor {

    private readonly EmplyJobDataPropertyIndexValueFactory _indexValueFactory;

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Emply.JobData";

    internal const string EditorName = "Limbo Emply Job Data";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Emply/Views/JobData.html";

    internal const string EditorIcon = "icon-limbo-emply color-limbo";

    #endregion

    #region Constructors

    public EmplyJobDataEditor(IDataValueEditorFactory dataValueEditorFactory, EmplyJobDataPropertyIndexValueFactory indexValueFactory) : base(dataValueEditorFactory) {
        _indexValueFactory = indexValueFactory;
    }

    #endregion

    #region Member methods

    public override IPropertyIndexValueFactory PropertyIndexValueFactory => _indexValueFactory;

    #endregion

}