using System.Text;
using System.IO;

namespace Simple.TextTemplates.Extensions
{
    public static class StreamReaderTemplateExt
    {
        public static ITextSource GetTextSource(this StreamReader me)
        {
            return new StringTextSource(me.ReadToEnd());
        }
        public static StringBuilder ReplaceTags(this StreamReader me, TagLookup lookup, TagStyle style)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(new StringTextSource(me.ReadToEnd()), style)
                .ReplaceTags(target, lookup);
            return target;
        }
        public static TextTemplate CompileTemplate(this StreamReader me, TagStyle style)
        {
            return TextTemplate.Compile(me.GetTextSource(), style);
        }
    }
}
