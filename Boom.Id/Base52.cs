namespace Boom.Id;

internal static class Base52
{
    private const int BASE = 52; // Matches the number of characters in the allowed characters array below
    private static readonly char[] BaseChars = "0123456789BCDFGHJKLMNPQRSTVWXYZbcdfghjklmnpqrstvwxyz".ToCharArray();

    public static string ToBase52(this long input)
    {
        if (input < 0) throw new ArgumentOutOfRangeException(nameof(input), input, "input cannot be negative");

        if (input == 0) return BaseChars[0].ToString();

        var stack = new Stack<char>();
        while (input != 0)
        {
            stack.Push(BaseChars[input % BASE]);
            input /= BASE;
        }

        return new string(stack.ToArray());
    }
}