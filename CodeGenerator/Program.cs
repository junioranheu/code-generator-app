using CodeGenerator.Enums;
using CodeGenerator.Models;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

List<Class> classes =
[
    new() { Name = "User", Props = "UserId Guid Name string Age int City string aeaaaaaa bool" },
    new() { Name = "Log", Props = "LogId Guid Desc string Date DateTime" },
];

foreach (var item in classes)
{
    List<string> props = GenerateModelProps(item.Props);
    string content = GenerateModel(item.Name, props);
    GenerateFile(fileName: item.Name, content, GetEnumDesc(ExtensionsEnum.cs));
}

Console.WriteLine("Press any key to exit the program");
Console.ReadLine();