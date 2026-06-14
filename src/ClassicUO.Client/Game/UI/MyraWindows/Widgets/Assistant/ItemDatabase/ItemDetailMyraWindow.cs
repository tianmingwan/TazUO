#nullable enable
using System;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Game.Managers.Structs;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.Utility;
using ClassicUO.Utility.Logging;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.ItemDatabase;

public class ItemDetailMyraWindow : MyraControl
{
    private readonly ItemInfo _item;
    private static ItemDetailLanguage Lang => Language.Instance.Assistant.ItemDetail;
    private static UiCommonsLanguage Common => Language.Instance.UiCommons;

    public ItemDetailMyraWindow(ItemInfo item) : base(string.Format(Lang.WindowTitle, item.Name))
    {
        _item = item;

        var layout = new VerticalStackPanel { Spacing = 8 };
        layout.Widgets.Add(BuildGraphicSection());
        layout.Widgets.Add(BuildBasicInfoSection());
        layout.Widgets.Add(BuildLocationSection());
        layout.Widgets.Add(BuildPropertiesSection());
        layout.Widgets.Add(BuildActionsSection());

        SetRootContent(new ScrollViewer { MaxHeight = 600, Content = layout });
        CenterInViewPort();
        UIManager.Add(this);
        BringOnTop();
    }

    private Widget BuildGraphicSection()
    {
        var lang = Lang;
        var row = new HorizontalStackPanel { Spacing = 8 };

        if (_item.Graphic > 0)
            row.Widgets.Add(new MyraArtTexture(_item.Graphic, 64)
                { Tooltip = $"Graphic: {_item.Graphic} (0x{_item.Graphic:X4})" });

        var infoCol = new VerticalStackPanel { Spacing = 2 };
        infoCol.Widgets.Add(new MyraLabel(string.Format(lang.GraphicId, _item.Graphic, _item.Graphic), MyraLabel.TextStyle.P));
        infoCol.Widgets.Add(_item.Hue > 0
            ? new MyraLabel(string.Format(lang.Hue, _item.Hue, _item.Hue), MyraLabel.TextStyle.P)
            : new MyraLabel(lang.HueDefault, MyraLabel.TextStyle.P));
        row.Widgets.Add(infoCol);
        return row;
    }

    private Widget BuildBasicInfoSection()
    {
        var lang = Lang;
        var common = Common;
        var panel = new VerticalStackPanel { Spacing = 2 };
        panel.Widgets.Add(new MyraLabel(lang.BasicInformation, MyraLabel.TextStyle.H3));

        if (_item.CustomName.NotNullNotEmpty())
            panel.Widgets.Add(new MyraLabel(string.Format(lang.CustomName, _item.CustomName), MyraLabel.TextStyle.P));

        panel.Widgets.Add(new MyraLabel(string.Format(lang.Name, _item.Name, _item.Serial), MyraLabel.TextStyle.P));
        panel.Widgets.Add(new MyraLabel(string.Format(lang.Layer, _item.Layer, (int)_item.Layer), MyraLabel.TextStyle.P));

        TimeSpan timeAgo = DateTime.Now - _item.UpdatedTime;
        string timeText = timeAgo.TotalDays >= 1    ? string.Format(common.DaysAgo, timeAgo.Days)
            : timeAgo.TotalHours >= 1               ? string.Format(common.HoursAgo, timeAgo.Hours)
            : timeAgo.TotalMinutes >= 1             ? string.Format(common.MinutesAgo, (int)timeAgo.TotalMinutes)
            : common.JustNow;
        panel.Widgets.Add(new MyraLabel(string.Format(lang.LastSeen, timeText), MyraLabel.TextStyle.P));

        string charServer = _item.CharacterName;
        if (!string.IsNullOrEmpty(_item.ServerName))
            charServer += string.Format($" ({lang.Server})", _item.ServerName);
        panel.Widgets.Add(new MyraLabel(string.Format(lang.Character, charServer), MyraLabel.TextStyle.P));

        return panel;
    }

    private Widget BuildLocationSection()
    {
        var lang = Lang;
        var panel = new VerticalStackPanel { Spacing = 2 };
        panel.Widgets.Add(new MyraLabel(lang.Location, MyraLabel.TextStyle.H3));

        if (_item.OnGround)
        {
            panel.Widgets.Add(new MyraLabel(string.Format(lang.OnGroundAt, _item.X, _item.Y), MyraLabel.TextStyle.P));
        }
        else
        {
            panel.Widgets.Add(new MyraLabel(lang.InContainer, MyraLabel.TextStyle.P));
            if (_item.Container != 0)
            {
                panel.Widgets.Add(new MyraLabel(string.Format(lang.Container, _item.Container), MyraLabel.TextStyle.P));

                Item? containerItem = Client.Game.UO?.World?.Items?.Get(_item.Container);
                if (containerItem != null &&
                    containerItem.RootContainer != 0 &&
                    containerItem.RootContainer != _item.Container)
                    panel.Widgets.Add(new MyraLabel(string.Format(lang.RootContainer, containerItem.RootContainer), MyraLabel.TextStyle.P));
            }
        }

        return panel;
    }

    private Widget BuildPropertiesSection()
    {
        var lang = Lang;
        var panel = new VerticalStackPanel { Spacing = 2 };
        panel.Widgets.Add(new MyraLabel(lang.Properties, MyraLabel.TextStyle.H3));

        if (!string.IsNullOrEmpty(_item.Properties))
        {
            foreach (string prop in _item.Properties.Split('|'))
                if (!string.IsNullOrWhiteSpace(prop))
                    panel.Widgets.Add(new MyraLabel(string.Concat("\u2022 ", prop.Trim()), MyraLabel.TextStyle.P));
        }
        else
        {
            panel.Widgets.Add(new MyraLabel(lang.NoPropertiesAvailable, MyraLabel.TextStyle.P));
        }

        return panel;
    }

    private Widget BuildActionsSection()
    {
        var lang = Lang;
        var panel = new VerticalStackPanel { Spacing = 4 };
        panel.Widgets.Add(new MyraLabel(lang.Actions, MyraLabel.TextStyle.H3));

        var row1 = new HorizontalStackPanel { Spacing = 4 };

        Item? worldItem = World.Instance?.Items?.Get(_item.Serial);
        if (worldItem != null && !worldItem.IsDestroyed)
        {
            row1.Widgets.Add(new MyraButton(lang.UseItem, () =>
                GameActions.DoubleClick(World.Instance, _item.Serial))
            { Tooltip = lang.UseItemTooltip });
        }

        uint backpackSerial = Client.Game.UO?.World?.Player?.Backpack?.Serial ?? 0;
        if (_item.Container != backpackSerial)
        {
            row1.Widgets.Add(new MyraButton(lang.TakeItem, MoveToBackpack)
                { Tooltip = lang.TakeItemTooltip });
        }

        row1.Widgets.Add(new MyraButton(lang.TryToLocate, TryToLocate)
            { Tooltip = lang.TryToLocateTooltip });

        row1.Widgets.Add(new MyraButton(lang.SetCustomName, () =>
        {
            var nameBox = new MyraInputBox { Text = _item.CustomName, Width = 220 };
            new MyraDialog(lang.SetCustomNameTitle, nameBox, ok =>
            {
                if (!ok) return;
                _item.CustomName = nameBox.Text ?? "";
                Item? wi = World.Instance?.Items?.Get(_item.Serial);
                if (wi != null)
                {
                    wi.CustomName = _item.CustomName;
                    ItemDatabaseManager.Instance.AddOrUpdateItem(wi, World.Instance);
                }
            });
        }));

        panel.Widgets.Add(row1);

        var row2 = new HorizontalStackPanel { Spacing = 4 };

        if (!_item.OnGround && _item.Container != 0)
        {
            row2.Widgets.Add(new MyraButton(lang.ViewContainer, () =>
                OpenContainerDetail(_item.Container))
            { Tooltip = lang.ViewContainerTooltip });

            Item? cont = Client.Game.UO?.World?.Items?.Get(_item.Container);
            if (cont != null &&
                cont.RootContainer != 0 &&
                cont.RootContainer != _item.Container)
            {
                row2.Widgets.Add(new MyraButton(lang.ViewRootContainer, () =>
                    OpenContainerDetail(cont.RootContainer))
                { Tooltip = lang.ViewRootContainerTooltip });
            }
        }

        row2.Widgets.Add(new MyraButton(lang.Close, () => _disposeRequested = true));
        panel.Widgets.Add(row2);

        return panel;
    }

    private void MoveToBackpack()
    {
        try
        {
            World? world = Client.Game.UO?.World;
            PlayerMobile? player = world?.Player;
            if (player == null) return;

            Item? item = world?.Items?.Get(_item.Serial);
            if (item == null) { Log.Warn("Cannot move item: not found in world"); return; }

            Item? backpack = world?.Items?.Get(player.Backpack?.Serial ?? 0);
            if (backpack == null) { Log.Warn("Cannot move item: backpack not found"); return; }

            if (backpack.Serial == item.Container) { Log.Info("Item is already in backpack"); return; }

            ObjectActionQueue.Instance.Enqueue(
                new MoveRequest(item.Serial, backpack.Serial).ToObjectActionQueueItem(),
                ActionPriority.MoveItem);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to move item to backpack: {ex.Message}");
        }
    }

    private void TryToLocate()
    {
        try
        {
            World? world = Client.Game.UO?.World;
            if (world?.Player == null) return;

            if (_item.OnGround)
            {
                CreateQuestArrow(_item.X, _item.Y);
                return;
            }

            if (_item.Container == 0) return;

            Item? containerItem = world.Items?.Get(_item.Container);
            if (containerItem != null)
            {
                if (containerItem.RootContainer == world.Player.Serial)
                {
                    CreateQuestArrow(world.Player.X, world.Player.Y);
                }
                else
                {
                    Item? root = world.Items?.Get(containerItem.RootContainer);
                    if (root != null && root.OnGround)
                        CreateQuestArrow(root.X, root.Y);
                    else
                    {
                        Mobile? mob = world.Mobiles?.Get(containerItem.RootContainer);
                        if (mob != null)
                            CreateQuestArrow(mob.X, mob.Y);
                        else
                            SearchDatabaseForLocation(containerItem.RootContainer);
                    }
                }
            }
            else
            {
                SearchDatabaseForLocation(_item.Container);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to locate item: {ex.Message}");
        }
    }

    private void SearchDatabaseForLocation(uint containerSerial) =>
        ItemDatabaseManager.Instance.SearchItems(
            results =>
            {
                MainThreadQueue.InvokeOnMainThread(() =>
                {
                    if (results is { Count: > 0 })
                    {
                        ItemInfo ci = results[0];
                        if (ci.OnGround)
                            CreateQuestArrow(ci.X, ci.Y);
                        else
                        {
                            World? world = Client.Game.UO?.World;
                            if (world?.Player != null && ci.Container == world.Player.Serial)
                                CreateQuestArrow(world.Player.X, world.Player.Y);
                        }
                    }
                });
            },
            serial: containerSerial,
            limit: 1);

    private void CreateQuestArrow(int x, int y)
    {
        try
        {
            World? world = Client.Game.UO?.World;
            if (world == null) return;

            QuestArrowGump? existing = UIManager.GetGump<QuestArrowGump>(_item.Serial);
            existing?.Dispose();

            var arrow = new QuestArrowGump(world, _item.Serial, x, y)
                { CanCloseWithRightClick = true };
            UIManager.Add(arrow);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to create quest arrow: {ex.Message}");
        }
    }

    private void OpenContainerDetail(uint containerSerial) =>
        ItemDatabaseManager.Instance.SearchItems(
            results =>
            {
                MainThreadQueue.InvokeOnMainThread(() =>
                {
                    if (results is { Count: > 0 })
                        new ItemDetailMyraWindow(results[0]);
                    else
                        Log.Warn($"Container 0x{containerSerial:X8} not found in item database");
                });
            },
            serial: containerSerial,
            limit: 1);
}
