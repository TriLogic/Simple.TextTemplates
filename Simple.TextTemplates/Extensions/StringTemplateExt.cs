using System;
using System.Text;

namespace Simple.TextTemplates.Extensions
{
    public static class StringTemplateExt
    {
        public static ITextSource GetTextSource(this string me)
        {
            return new StringTextSource(me);
        }
        public static StringBuilder ReplaceTags(this string me, TagLookup lookup, TagStyle style)
        {
            StringBuilder target = new StringBuilder();
            me.CompileTemplate(style)
                .ReplaceTags(target, lookup);
            return target;
        }

        public static TextTemplate CompileTemplate(this string me, TagStyle style) 
        {
            return TextTemplate.Compile(me.GetTextSource(), style);
        }
    }
}
