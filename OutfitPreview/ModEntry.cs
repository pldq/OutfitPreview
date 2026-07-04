using OutfitPreview.EventHandler;
using StardewModdingAPI;

namespace OutfitPreview;

public class ModEntry : Mod
{
    private OutfitItemPreviewHandler? _outfitItemPreviewHandler;
    
    public override void Entry(IModHelper helper)
    {
        _outfitItemPreviewHandler = new OutfitItemPreviewHandler(helper, Monitor);
        Monitor.Log("DressUpPreview Mod initialized.", LogLevel.Info);
    }
}