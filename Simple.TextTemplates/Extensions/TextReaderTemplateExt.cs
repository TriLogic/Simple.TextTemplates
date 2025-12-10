using System.Text;
using System.IO;

namespace Simple.TextTemplates.Extensions
{
    public static class TextReaderTemplateExt
    {
        public static ITextSource GetTextSource(this TextReader me)
        {
            return new StringTextSource(me.ReadToEnd());
        }
        public static StringBuilder ReplaceTags(this TextReader me, TagLookup lookup, TagStyle style)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(me.GetTextSource(), style)
                .ReplaceTags(target, lookup);
            return target;
        }
        public static TextTemplate CompileTemplate(this TextReader me, TagStyle style)
        {
            return TextTemplate.Compile(me.GetTextSource(), style);
        }
    }
}
