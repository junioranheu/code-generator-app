namespace CodeGenerator.Utils.Fixtures;

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
}