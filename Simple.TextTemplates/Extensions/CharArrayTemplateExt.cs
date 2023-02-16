using System.Text;

namespace Simple.TextTemplates.Extensions
{
    public static class CharArrayTemplateExt
    {
        public static ITextSource GetTextSource(this char[] me)
        {
            return new CharArrayTextSource(me);
        }
        public static StringBuilder ReplaceTags(this char[] me, TagLookup lookup)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(new CharArrayTextSource(me))
                .ReplaceTags(target, lookup);
            return target;
        }
        public static TextTemplate CompileTemplate(this char[] me)
        {
            return TextTemplate.Compile(me.GetTextSource());
        }
    }
}
