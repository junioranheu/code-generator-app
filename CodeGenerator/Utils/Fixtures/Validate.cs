namespace CodeGenerator.Console.Utils.Fixtures;

public static partial class Validate
{
    /// <summary>
    /// Validates whether the parameters of an entity constructor are valid.
    /// Example usage: ValidateEntityParams(nameof(User), [name, role], nameof(name), nameof(role));
    /// </summary>
    public static void ValidateEntityParams(string entityName, object[] values, params string[] paramNames)
    {
        if (values?.Length != paramNames?.Length)
        {
            throw new Exception($"Internal error. The number of properties of the entity '{entityName}' differs at the time of validation.");
        }

        if (values?.Length < 1 || paramNames?.Length < 1)
        {
            throw new Exception($"Internal error. There is a broken validation in the entity '{entityName}'.");
        }

        for (int i = 0; i < values?.Length; i++)
        {
            var item = values[i];
            string paramName = paramNames![i];
            bool shouldThrowException = false;

            if (item is string str && (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str)))
            {
                shouldThrowException = true;
            }
            else if (item is int intValue && intValue < 1)
            {
                shouldThrowException = true;
            }
            else if (item is double doubleValue && doubleValue < 1.0)
            {
                shouldThrowException = true;
            }
            else if (item is float floatValue && floatValue < 1.0)
            {
                shouldThrowException = true;
            }
            else if (item is Guid guidValue && guidValue == Guid.Empty)
            {
                shouldThrowException = true;
            }
            else if (item is DateTime dateTimeValue && dateTimeValue == DateTime.MinValue)
            {
                shouldThrowException = true;
            }
            else if (item == null)
            {
                shouldThrowException = true;
            }

            if (shouldThrowException)
            {
                throw new ArgumentException($"Internal error. The property '{paramName}' is invalid in the entity '{entityName}'.");
            }
        }
    }

    /// <summary>
    /// Validates that the specified DbContext name does not conflict with
    /// common .NET or Entity Framework types and APIs.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the name is null, empty, or consists only of whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified name is reserved or may cause conflicts
    /// with .NET or Entity Framework APIs.
    /// </exception>
    public static void ValidateDbContextName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome do DbContext não pode ser nulo, vazio ou conter apenas espaços em branco.", nameof(name));
        }

        HashSet<string> forbiddenNames =
        [
            with(StringComparer.OrdinalIgnoreCase),
            "Context",
            "DbContext",
            "Database",
            "Model",
            "Configuration",
            "Options",
            "Connection",
            "Transaction",
            "Entity",
            "Migration",
            "Command",
            "DataContext"
        ];

        if (forbiddenNames.Contains(name))
        {
            throw new InvalidOperationException($"O nome '{name}' não pode ser utilizado para um DbContext, pois pode entrar em conflito com APIs do .NET ou do Entity Framework.");
        }
    }
}