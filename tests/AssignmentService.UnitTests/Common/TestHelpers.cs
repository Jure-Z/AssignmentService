using System.Reflection;

namespace AssignmentService.UnitTests.Common;

public static class TestHelpers
{
    // Nastavi vrednost privatne lastnosti na entiteti za potrebe testiranja.
    public static void SetPrivateProperty<T>(T target, string propertyName, object? value)
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property == null)
            throw new ArgumentException($"Lastnost '{propertyName}' ne obstaja na tipu {typeof(T).Name}");

        property.SetValue(target, value);
    }
}