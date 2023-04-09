using Heretic.InteractiveFiction.GamePlay;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal class ResourceProvider : IResourceProvider
{
    public IDictionary<string, IEnumerable<string>> GetItemsFromResources()
    {
        return ((IResourceProvider)this).ReadEntriesFromResources(Items.ResourceManager);
    }

    public IDictionary<string, IEnumerable<string>> GetCharactersFromResources()
    {
        return ((IResourceProvider)this).ReadEntriesFromResources(Characters.ResourceManager);
    }

    public IDictionary<string, IEnumerable<string>> GetLocationsFromResources()
    {
        return ((IResourceProvider)this).ReadEntriesFromResources(Locations.ResourceManager);
    }
}