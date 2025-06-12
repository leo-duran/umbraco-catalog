using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Catalog.Plugin.Composers
{
    public class SuggestionsPropertyEditorComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register our custom property editor schema
            builder.DataEditors().Add<SuggestionsPropertyEditor>();
        }
    }

    [DataEditor("Catalog.Suggestions")]
    public class SuggestionsPropertyEditor : DataEditor
    {
        public SuggestionsPropertyEditor(IDataValueEditorFactory dataValueEditorFactory)
            : base(dataValueEditorFactory)
        {
        }
    }
} 
