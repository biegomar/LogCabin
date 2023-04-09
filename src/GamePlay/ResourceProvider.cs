using System.Collections;
using System.Globalization;
using System.Resources;
using Heretic.InteractiveFiction.GamePlay;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal class ResourceProvider : IResourceProvider
{
    public IDictionary<string, IEnumerable<string>> GetItemsFromResources()
    {
        var result = new Dictionary<string, IEnumerable<string>>();
        
        var resourceSet = Items.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        if (resourceSet != null)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var inputList = entry.Value?.ToString()?.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (inputList?.Any() == true)
                {
                    var normalizedList = this.NormalizeResourceList(inputList);
                    result.Add(entry.Key.ToString()!, normalizedList);
                }
            }
        }

        return result;
    }
    
    public IDictionary<string, IEnumerable<string>> GetCharactersFromResources()
    {
        var result = new Dictionary<string, IEnumerable<string>>();

        var resourceSet = Characters.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        if (resourceSet != null)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var inputList = entry.Value?.ToString()?.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (inputList?.Any() == true)
                {
                    var normalizedList = this.NormalizeResourceList(inputList);
                    result.Add(entry.Key.ToString()!, normalizedList);
                }
            }
        }

        return result;
    }
    
    public IDictionary<string, IEnumerable<string>> GetLocationsFromResources()
    {
        var result = new Dictionary<string, IEnumerable<string>>();

        var resourceSet = Locations.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        if (resourceSet != null)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var inputList = entry.Value?.ToString()?.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (inputList?.Any() == true)
                {
                    var normalizedList = this.NormalizeResourceList(inputList);
                    result.Add(entry.Key.ToString()!, normalizedList);
                }
            }
        }

        return result;
    }
    
    private IEnumerable<string> NormalizeResourceList(IEnumerable<string> inputList)
    {
        var result = new List<string>();
        foreach (var item in inputList)
        {
            result.Add(item);
            var trimmedItem = string.Concat(item.Where(c => !char.IsWhiteSpace(c)));
            if (item != trimmedItem)
            {
                result.Add(trimmedItem);
            }
        }

        return result;
    }
}