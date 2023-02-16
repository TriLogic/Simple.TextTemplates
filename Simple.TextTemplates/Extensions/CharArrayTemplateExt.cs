using System.Text;

namespace Simple.TextTemplates.Extensions
{
    public static class CharArrayTemplateExt
    {
        public static StringBuilder ReplaceTags(this char[] me, TagLookup lookup)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(new CharArrayTextSource(me))
                .ReplaceTags(target, lookup);
            return target;
        }
    }
}
