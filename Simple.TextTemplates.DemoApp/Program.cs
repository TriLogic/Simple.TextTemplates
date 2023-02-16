using System.Text;
using Simple.TextTemplates;
using Simple.TextTemplates.Extensions;

const string TestTemplate = "Hello ${place}!";

// Simple single string template with replace.
Console.WriteLine(TestTemplate.ReplaceTags((string tag) => "World"));
Console.WriteLine();

// Compiled template for multiple use.
var template = TestTemplate.CompileTemplate();

List<string> places = new() { "Town", "City", "State", "Country", "World", "Solar System", "Galaxy" };
var target = new StringBuilder();

places.ForEach(place =>
{
    template.ReplaceTags(target, (string tag) => place);
    Console.WriteLine(target);
    target.Clear();
});
Console.WriteLine();

Console.WriteLine("done.");