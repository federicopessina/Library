namespace Library.Console.Input;

public static class ConsoleInput
{

    public static async Task<int> GetIntAsync(string variableName)
    {
        do
        {
            System.Console.Write($"Insert {nameof(Int32)} {variableName}: ");

            string? @string = System.Console.ReadLine();

            if (string.IsNullOrEmpty(@string))
                continue;

            if (!int.TryParse(@string, out int result))
                continue;

            return await Task.FromResult(result);

        } while (true);
    }

    public static async Task<string> GetStringAsync(string variableName)
    {
        do
        {
            System.Console.Write($"Insert {nameof(String)} {variableName}: ");

            string? result = System.Console.ReadLine();
            if (string.IsNullOrEmpty(result))
                continue;

            return await Task.FromResult(result);

        } while (true);
    }

    public static async Task<int?> GetNullableIntAsync(string variableName)
    {
        do
        {
            System.Console.Write($"Insert {nameof(Int32)} {variableName}: ");

            string? @string = System.Console.ReadLine();

            if (@string is null || @string.Equals(string.Empty))
                return await Task.FromResult<int?>(null);

            if (int.TryParse(@string, out int result))
                return await Task.FromResult(result);

        } while (true);
    }

    public static async Task<string?> GetNullableStringAsync(string variableName)
    {
        do
        {
            System.Console.Write($"Insert {nameof(String)} {variableName}: ");

            string? @string = System.Console.ReadLine();

            if (@string is null || @string.Equals(string.Empty))
                return await Task.FromResult<string?>(null);

            return await Task.FromResult(@string);

        } while (true);
    }
}
