using System.Text;

namespace Simple.TextTemplates.Extensions
{
    public static class StringBuilderTemplateExt
    {
        public static StringBuilder ReplaceTags(this StringBuilder me, TagLookup lookup)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(new StringBuilderTextSource(me))
                .ReplaceTags(target, lookup);
            return target;
        }
    }
}
