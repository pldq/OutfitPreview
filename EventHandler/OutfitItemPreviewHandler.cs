using Microsoft.Xna.Framework;
using OutfitPreview.UIElements;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace OutfitPreview.EventHandler;

public class OutfitItemPreviewHandler
{
    private readonly IModHelper _helper;
    

    private OutfitFarmerPreviewPanel? _outfitFarmerPreviewPanel;

    public OutfitItemPreviewHandler(IModHelper helper)
    {
        _helper = helper;

        helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        switch (e.NewMenu)
        {
            case GameMenu gameMenu:
                if (gameMenu.GetCurrentPage() is InventoryPage)
                {
                    return;
                }

                break;
            case ItemGrabMenu:
            case TailoringMenu:
            case DyeMenu:
            case ShopMenu:
                return;
        }

        _outfitFarmerPreviewPanel?.Dispose();
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        var hoveredItem = CastItem2Outfit(GetHoveredItemFromActiveMenu());
        if (hoveredItem == null) return;
        _outfitFarmerPreviewPanel ??= new OutfitFarmerPreviewPanel();
        var position = new Vector2(
            Game1.getMouseX() + 32,
            Game1.getMouseY() + 32 - _outfitFarmerPreviewPanel.PanelHeight - 8
        );

        _outfitFarmerPreviewPanel.Draw(e.SpriteBatch, hoveredItem, position);
    }

    private static Item? CastItem2Outfit(Item? item)
    {
        return item switch
        {
            Clothing or Hat or Boots => item,
            _ => null
        };
    }

    private Item? GetHoveredItemFromActiveMenu()
    {
        switch (Game1.activeClickableMenu)
        {
            case GameMenu gameMenu:
            {
                var currentPage = gameMenu.GetCurrentPage();
                if (currentPage is InventoryPage inventory)
                {
                    return inventory.hoveredItem;
                }

                break;
            }
            case ItemGrabMenu itemGrabMenu:
                return itemGrabMenu.hoveredItem;
            // case TailoringMenu tailoringMenu:
                // return GetHoveredItemFromHasIngredientMenu(tailoringMenu);
            // case DyeMenu dyeMenu:
                // return GetHoveredItemFromHasIngredientMenu(dyeMenu);
            case ShopMenu { hoveredItem: Item hoveredItem }:
                return hoveredItem;
        }

        return null;
    }

    // private Item? GetHoveredItemFromHasIngredientMenu(IClickableMenu menu)
    // {
    //     try
    //     {
    //         var rightIngredientField = _helper.Reflection.GetField<Item>(menu, "_rightIngredient", required: false);
    //         Item? rightItem = rightIngredientField?.GetValue();
    //         if (rightItem is Clothing clothing)
    //             return clothing;
    //
    //         var leftIngredientField = _helper.Reflection.GetField<Item>(menu, "_leftIngredient", required: false);
    //         Item? leftItem = leftIngredientField?.GetValue();
    //         if (leftItem is Clothing leftClothing)
    //             return leftClothing;
    //     }
    //     catch
    //     {
    //         // ignored
    //     }
    //
    //     return null;
    // }
}