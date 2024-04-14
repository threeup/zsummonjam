using System;

public static class ZapoEnumUtils
{
    public static T[] GetAllEnumValues<T>() where T : struct, IComparable, IFormattable, IConvertible
    {
        return (T[])Enum.GetValues(typeof(T));
    }

    public static string GetEnumName<T>(T enumValue) where T : struct, IComparable, IFormattable, IConvertible
    {
        return Enum.GetName(typeof(T), enumValue);
    }

    public static T GetEnumByName<T>(string name) where T : struct, IComparable, IFormattable, IConvertible
    {
        return (T)Enum.Parse(typeof(T), name, true);
    }

}