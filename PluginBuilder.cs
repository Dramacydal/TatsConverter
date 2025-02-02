using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace TatsConverter;

public class PluginBuilder
{
    private readonly SkyrimMod _mod;
    private readonly string _path;
    private readonly string _contextName;
    private readonly SkyrimRelease _release;

    public PluginBuilder(string contextName, string path, SkyrimRelease release = SkyrimRelease.SkyrimSE)
    {
        _contextName = contextName;
        _path = path;
        _release = release;
    }

    private string QuestEditorId => Helpers.NormalizeId($"STC_{_contextName}");
    
    public string ScriptName => QuestEditorId + "_Script";
    
    public void Save(IEnumerable<string> tattooSections)
    {
        var mod = new SkyrimMod(ModKey.FromNameAndExtension(_path), _release);
        mod.ModHeader.Author = "Dramacydal";
        mod.ModHeader.Description = "Contains RaceMenu Overlays created from tattoos: " + string.Join(", ", tattooSections);
        
        var quest = mod.Quests.AddNew();
        quest.Name = QuestEditorId;
        quest.EditorID = QuestEditorId;
        quest.Flags = Quest.Flag.StartGameEnabled | Quest.Flag.RunOnce | (Quest.Flag)16;

        var alias = new QuestAlias();
        alias.Name = "Player";
        alias.ForcedReference = new FormLinkNullable<IPlacedGetter>(new FormKey(mod.ModKey, 0x14));
        
        quest.Aliases.Add(alias);
        
        var adapter = new QuestAdapter();
        var adapterAlias = new QuestFragmentAlias();
        adapterAlias.Property = new()
        {
            Alias = 0,
            Object = new FormLink<ISkyrimMajorRecordGetter>(quest)
        };
        adapterAlias.Scripts.Add(new ScriptEntry()
        {
            Flags = ScriptEntry.Flag.Local,
            Name = "RaceMenuLoad",
        });
        
        adapter.Aliases.Add(adapterAlias);
        adapter.Scripts.Add(new ScriptEntry()
        {
            Flags = ScriptEntry.Flag.Local,
            Name = ScriptName,
        });
        
        quest.VirtualMachineAdapter = adapter;
        
        mod.WriteToBinary(_path);
    }
}