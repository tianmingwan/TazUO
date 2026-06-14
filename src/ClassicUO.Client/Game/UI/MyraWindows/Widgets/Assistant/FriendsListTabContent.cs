#nullable enable
using System.Collections.Generic;
using ClassicUO.Configuration;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant;

public static class FriendsListTabContent
{
    public static Widget Build()
    {
        var lang = Language.Instance.Assistant.FriendsList;

        var friendsListPanel = new VerticalStackPanel { Spacing = 4 };

        void BuildFriendsList()
        {
            friendsListPanel.Widgets.Clear();

            List<FriendEntry> friends = FriendsListManager.Instance.GetFriends();

            if (friends.Count == 0)
            {
                friendsListPanel.Widgets.Add(new MyraLabel(lang.NoFriendsAddedYet, MyraLabel.TextStyle.P));
                return;
            }

            friendsListPanel.Widgets.Add(new MyraLabel(lang.CurrentFriends, MyraLabel.TextStyle.H2));

            var grid = new MyraGrid();
            grid.SetupWithHeaders(
                GridColumnInfo.Numeric(lang.ColSerial),
                GridColumnInfo.Fill(lang.ColName, 2),
                GridColumnInfo.Auto(lang.ColDateAdded),
                GridColumnInfo.Auto("")
            );

            int row = 1;
            for (int i = friends.Count - 1; i >= 0; i--)
            {
                FriendEntry f = friends[i];

                grid.AddWidget(new MyraLabel(f.Serial != 0 ? f.Serial.ToString() : lang.NA, MyraLabel.TextStyle.P, MyraLabel.AlignMode.Right), row, 0);
                grid.AddWidget(new MyraLabel(f.Name ?? lang.Unknown, MyraLabel.TextStyle.P), row, 1);
                grid.AddWidget(new MyraLabel(f.DateAdded.ToString("yyyy-MM-dd"), MyraLabel.TextStyle.P), row, 2);
                grid.AddWidget(MyraStyle.ApplyButtonDangerStyle(new MyraButton(Language.Instance.UiCommons.Remove, () =>
                {
                    bool removed = f.Serial != 0
                        ? FriendsListManager.Instance.RemoveFriend(f.Serial)
                        : FriendsListManager.Instance.RemoveFriend(f.Name);

                    if (removed)
                    {
                        GameActions.Print(World.Instance, string.Format(lang.RemovedFromFriendsList, f.Name));
                        BuildFriendsList();
                    }
                })), row, 3);

                row++;
            }

            friendsListPanel.Widgets.Add(grid);
        }

        BuildFriendsList();

        var root = new VerticalStackPanel { Spacing = 6 };
        root.Widgets.Add(new MyraLabel(lang.ManageFriendsList, MyraLabel.TextStyle.H3));
        root.Widgets.Add(new MyraButton(lang.AddByTarget, () =>
        {
            GameActions.Print(World.Instance, lang.TargetPlayerToAdd);
            World.Instance.TargetManager.SetTargeting(targeted =>
            {
                if (targeted is Mobile mobile)
                {
                    if (FriendsListManager.Instance.AddFriend(mobile))
                    {
                        GameActions.Print(World.Instance, string.Format(lang.AddedToFriendsList, mobile.Name));
                        BuildFriendsList();
                    }
                    else
                    {
                        GameActions.Print(World.Instance, string.Format(lang.CouldNotAddAlreadyInList, mobile.Name));
                    }
                }
                else
                {
                    GameActions.Print(World.Instance, lang.InvalidTargetMustBePlayer);
                }
            });
        }));
        root.Widgets.Add(new ScrollViewer { Height = 300, Content = friendsListPanel });

        return root;
    }
}
