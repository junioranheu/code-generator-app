using CodeGenerator.Enums;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

string className = "Pessoa";
string classDefinition = "Name string Age int City string AEA bool";

List<string> attributes = GenerateAttributes(classDefinition);
string content = GenerateModel(className, attributes);
GenerateFile(className, content, GetEnumDesc(ExtensionsEnum.cs));

Console.WriteLine($"File {className}.cs generated successfully!");
Console.ReadLine();