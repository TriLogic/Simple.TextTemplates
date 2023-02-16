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

Console.WriteLine("done.");