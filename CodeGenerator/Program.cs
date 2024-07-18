using CodeGenerator.Enums;
using static CodeGenerator.Utils.Fixtures.Generate;
using static CodeGenerator.Utils.Fixtures.Get;

string className = "Pessoa";
string classProps = "Name string Age int City string AEA bool";

List<string> props = GenerateModelProps(classProps);
string content = GenerateModel(className, props);
GenerateFile(fileName: className, content, GetEnumDesc(ExtensionsEnum.cs));

Console.ReadLine();