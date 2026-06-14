#nullable enable
using ClassicUO.Configuration;
using ClassicUO.Game.Managers;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Game.UI.MyraWindows.Widgets;
using ClassicUO.LegionScripting;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows;

public class ScriptErrorWindow : MyraControl
{
    private static int _id = 1;

    public ScriptErrorWindow(ScriptErrorDetails errorDetails) : base(string.Format(Language.Instance.Scripting.ScriptErrorTitle, _id++))
    {
        Build(errorDetails);
        _rootWindow.UpdateArrange();
        CenterInViewPort();
        UIManager.Add(this);
        BringOnTop();
    }

    private void Build(ScriptErrorDetails errorDetails)
    {
        var root = new VerticalStackPanel { Spacing = MyraStyle.STANDARD_SPACING };

        root.Widgets.Add(new MyraLabel(Language.Instance.Scripting.ScriptErrorHeader, MyraLabel.TextStyle.P));

        // Clickable red error message
        var errorLabel = new MyraLabel(errorDetails.ErrorMsg, MyraLabel.TextStyle.P)
        {
            TextColor = Color.Red,
            Tooltip = Language.Instance.Scripting.ClickToCopyToClipboard
        };
        errorLabel.TouchDown += (_, _) =>
        {
            SDL3.SDL.SDL_SetClipboardText(errorDetails.ErrorMsg);
            GameActions.Print(Language.Instance.Scripting.CopiedErrorToClipboard, Constants.HUE_SUCCESS);
        };
        root.Widgets.Add(errorLabel);

        // Locations in reverse order (innermost first)
        for (int i = errorDetails.Locations.Count - 1; i >= 0; i--)
        {
            ScriptErrorLocation loc = errorDetails.Locations[i];

            root.Widgets.Add(new MyraLabel(string.Format(Language.Instance.Scripting.FileLineFormat, loc.FileName, loc.LineNumber), MyraLabel.TextStyle.P));

            if (!string.IsNullOrEmpty(loc.LineContent))
            {
                root.Widgets.Add(new MyraInputBox
                {
                    Text = loc.LineContent,
                    Multiline = true,
                    Width = 480,
                    Height = 80,
                    Enabled = false
                });
            }
        }

        var btnRow = new HorizontalStackPanel { Spacing = 4 };
        btnRow.Widgets.Add(new MyraButton(Language.Instance.Scripting.Edit_, () => new ScriptEditorWindow(errorDetails.Script)));
        btnRow.Widgets.Add(new MyraButton(Language.Instance.Scripting.EditExternally, () =>
            ClassicUO.Utility.FileSystemHelper.OpenFileWithDefaultApp(errorDetails.Script.FullPath)));
        root.Widgets.Add(btnRow);

        SetRootContent(root);
    }
}
