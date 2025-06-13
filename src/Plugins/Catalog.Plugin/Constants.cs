namespace Catalog.Plugin
{
    public class Constants
    {
        public const string ApiName = "catalogplugin";

        public static class DataTypes
        {
            // Built-in data type IDs
            public static readonly Guid Textbox = new("0cc0eba1-9960-42c9-bf9b-60e150b429ae");
            public static readonly Guid TextArea = new("c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3");
            public static readonly Guid RichTextEditor = new("ca90c950-0aff-4e72-b976-a30b1ac57dad");
        }
    }
}
