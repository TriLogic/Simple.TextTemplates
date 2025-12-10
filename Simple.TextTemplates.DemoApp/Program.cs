using System.Text;
using Simple.TextTemplates;
using Simple.TextTemplates.Extensions;

const string StringTemplate = "Hello $${ ${place} $}!";
const string HandlebarTemplate = "Hello \\{ {{place}} \\}!";

// Simple single string template with replace.
Console.WriteLine(StringTemplate.ReplaceTags((string tag) => "World", TagStyle.StringBraces));
Console.WriteLine();

// Compiled template for multiple use.
var template = StringTemplate.CompileTemplate(TagStyle.StringBraces);

List<string> places = new() { "Town", "City", "State", "Country", "World", "Solar System", "Galaxy" };
var target = new StringBuilder();

// Using the template multiple times.
places.ForEach(place =>
{
    target.Clear();
    template.ReplaceTags(target, (string tag) => place);
    Console.WriteLine(target);
});
Console.WriteLine();

// Using the template in parallel
Parallel.ForEach(places, place =>
{
    var result = template.ReplaceTags((string tag) => place);
    Console.WriteLine(result);
});
Console.WriteLine();

template = HandlebarTemplate.CompileTemplate(TagStyle.Handlebars);
places.ForEach(place =>
{
    target.Clear();
    template.ReplaceTags(target, (string tag) => place);
    Console.WriteLine(target);
});

Console.WriteLine("done.");