using System.Runtime.CompilerServices;
using System.Text;

namespace Simple.TextTemplates.Extensions
{
    public static class StringBuilderTemplateExt
    {
        public static ITextSource GetTextSource(this StringBuilder me)
        {
            return new StringBuilderTextSource(me);
        }

        public static StringBuilder ReplaceTags(this StringBuilder me, TagLookup lookup, TagStyle style)
        {
            StringBuilder target = new StringBuilder();
            me.CompileTemplate(style)
                .ReplaceTags(target, lookup);
            return target;
        }
        public static TextTemplate CompileTemplate(this StringBuilder me, TagStyle style)
        {
            return TextTemplate.Compile(me.GetTextSource(), style);
        }
    }
}
