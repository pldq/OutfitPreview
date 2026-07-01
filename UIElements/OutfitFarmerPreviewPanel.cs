using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace OutfitPreview.UIElements;

public class OutfitFarmerPreviewPanel : IDisposable
{
    public int PanelWidth => GetBackground().Width;
    public int PanelHeight = GetBackground().Height;

    private Farmer? _farmer;

    private readonly Mutex _mutex = new();

    public OutfitFarmerPreviewPanel()
    {
        _farmer ??= Game1.player.CreateFakeEventFarmer();
    }

    public void Dispose()
    {
        _farmer = null;
    }

    public void Draw(SpriteBatch b, Item hoveredItem, Vector2 position)
    {
        if (_mutex.WaitOne())
        {
            try
            {
                _farmer ??= Game1.player.CreateFakeEventFarmer();
                switch (hoveredItem)
                {
                    case Clothing clothing:
                        switch (clothing.clothesType.Value)
                        {
                            case Clothing.ClothesType.SHIRT:
                                _farmer.shirtItem.Set(clothing);
                                _farmer.changeShirt("-1");
                                break;
                            case Clothing.ClothesType.PANTS:
                                _farmer.pantsItem.Set(clothing);
                                _farmer.changePantStyle("-1");
                                _farmer.pantsColor.Set(clothing.clothesColor.Value);
                                break;
                        }

                        _farmer.UpdateClothing();
                        break;
                    case Hat hat:
                        _farmer.hat.Set(hat);
                        break;
                    case Boots boots:
                        _farmer.boots.Set(boots);
                        break;
                }

                DrawFarmerPreview(b, position);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }
    }

    private void DrawFarmerPreview(SpriteBatch b, Vector2 position)
    {
        Rectangle safeArea = Utility.getSafeArea();
        if (position.X + PanelWidth > safeArea.Right)
            position.X = safeArea.Right - PanelWidth;
        if (position.X < safeArea.Left)
            position.X = safeArea.Left;
        if (position.Y + PanelHeight > safeArea.Bottom)
            position.Y = safeArea.Bottom - PanelHeight;
        if (position.Y < safeArea.Top)
            position.Y = safeArea.Top;
        b.End();
        b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
        FarmerRenderer.isDrawingForUI = true;
        var bgTexture = GetBackground();
        b.Draw(bgTexture, position, Color.White);
        _farmer?.FarmerRenderer.draw(
            b,
            new FarmerSprite.AnimationFrame(0, 0, false, false),
            0,
            new Rectangle(0, 0, 16, 32),
            new Vector2(
                position.X + 32,
                position.Y + 32
            ),
            Vector2.Zero,
            0.8f,
            _farmer.FacingDirection,
            Color.White,
            0.0f,
            1.0f,
            _farmer
        );
        FarmerRenderer.isDrawingForUI = false;
        DrawFarmerOutfitTooltip(b, position);
    }

    private void DrawFarmerOutfitTooltip(SpriteBatch b, Vector2 previewPos)
    {
        const int textPaddingWidth = 12 + 8 + 16;
        int windowWidth = 0, windowHeight = 20 + 16;
        const int rowHeight = 30;

        var hatName = _farmer?.hat?.Value?.DisplayName;
        if (hatName != null)
        {
            windowWidth = Math.Max(windowWidth, textPaddingWidth + (int)Game1.smallFont.MeasureString(hatName).X);
            windowHeight += rowHeight;
        }

        var shirtName = _farmer?.shirtItem?.Value?.DisplayName;
        if (shirtName != null)
        {
            windowWidth = Math.Max(windowWidth, textPaddingWidth + (int)Game1.smallFont.MeasureString(shirtName).X);
            windowHeight += rowHeight;
        }

        var pantsName = _farmer?.pantsItem?.Value?.DisplayName;
        if (pantsName != null)
        {
            windowWidth = Math.Max(windowWidth, textPaddingWidth + (int)Game1.smallFont.MeasureString(pantsName).X);
            windowHeight += rowHeight;
        }

        var bootsName = _farmer?.boots?.Value?.DisplayName;
        if (bootsName != null)
        {
            windowWidth = Math.Max(windowWidth, textPaddingWidth + (int)Game1.smallFont.MeasureString(bootsName).X);
            windowHeight += rowHeight;
        }

        var textOffset = new Vector2(4, (rowHeight - 18) / 2 - 6);

        if (hatName != null || shirtName != null || pantsName != null || bootsName != null)
        {
            var windowPos = previewPos + new Vector2(-windowWidth - 8, 0);
            IClickableMenu.drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                (int)windowPos.X,
                (int)windowPos.Y,
                windowWidth,
                windowHeight,
                Color.White
            );
            var drawPosition = windowPos + new Vector2(12, 20);
            if (hatName != null)
            {
                DrawSmallTextWithShadow(b, hatName, drawPosition + textOffset);
                drawPosition.Y += rowHeight;
            }

            if (shirtName != null)
            {
                DrawSmallTextWithShadow(b, shirtName, drawPosition + textOffset);
                drawPosition.Y += rowHeight;
            }

            if (pantsName != null)
            {
                DrawSmallTextWithShadow(b, pantsName, drawPosition + textOffset);
                drawPosition.Y += rowHeight;
            }

            if (bootsName != null)
            {
                DrawSmallTextWithShadow(b, bootsName, drawPosition + textOffset);
            }
        }
    }

    private void DrawSmallTextWithShadow(SpriteBatch b, string text, Vector2 position)
    {
        b.DrawString(Game1.smallFont, text, position + new Vector2(2, 2), Game1.textShadowColor);
        b.DrawString(Game1.smallFont, text, position, Game1.textColor);
    }

    private static Texture2D GetBackground()
    {
        return Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg;
    }
}