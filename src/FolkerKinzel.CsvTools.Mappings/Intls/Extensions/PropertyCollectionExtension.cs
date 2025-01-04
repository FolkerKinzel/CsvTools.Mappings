using System.Collections.ObjectModel;

namespace FolkerKinzel.CsvTools.Mappings.Intls.Extensions;

#if NETSTANDARD2_0 || NET462
internal static class PropertyCollectionExtension
{
    internal static bool TryGetValue(
        this KeyedCollection<string, DynamicProperty> kColl, string key, [NotNullWhen(true)] out DynamicProperty? value)
    {
        Debug.Assert(kColl != null);
        Debug.Assert(key != null);

        if (kColl.Contains(key))
        {
            value = kColl[key];
            return true;
        }

        value = null;
        return false;
    }
}
#endif