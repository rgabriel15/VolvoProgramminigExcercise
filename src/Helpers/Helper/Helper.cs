namespace Helper;
public static class Helper
{
    #region Methods
    /// <summary>
    /// Generates a random string using the System.Random class.
    /// </summary>
    /// <param name="length">String length.</param>
    /// <param name="charDictionary">Set of characters used to generate the random string.</param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "<Pending>")]
    public static string GetRandomString(ushort length
        , string charDictionary = "123456789ABCDEFGHJKLMNPRTUVWXY")
    {
        if (length < 2)
        {
            var paramName = nameof(length);
            throw new ArgumentException($"[{paramName}] must be greater than 1.", paramName);
        }

        if (charDictionary.Length < 2)
        {
            var paramName = nameof(charDictionary);
            throw new ArgumentException($"[{paramName}] length must be greater than 1.", paramName);
        }

        var random = new Random();
        var randomString = new string(Enumerable.Repeat(charDictionary, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
        return randomString;
    }
    #endregion
}
