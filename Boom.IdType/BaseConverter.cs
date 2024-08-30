namespace Boom.IdType;


/// <summary>
/// Base 31 is used in order to make the IDs case ignorant
/// </summary>
internal static class BaseConverter
{
    private static readonly char[] BaseChars = "0123456789abcdefghjklmnpqrstvwxyz".ToCharArray();
    private static readonly int Base = BaseChars.Length; // Matches the number of characters in the allowed characters array below

    public static string ToBase(this long input)
    {
        if (input < 0) throw new ArgumentOutOfRangeException(nameof(input), input, "input cannot be negative");

        if (input == 0) return BaseChars[0].ToString();

        var stack = new Stack<char>();
        while (input != 0)
        {
            stack.Push(BaseChars[input % Base]);
            input /= Base;
        }

        return new string(stack.ToArray());
    }
}