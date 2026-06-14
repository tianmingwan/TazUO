#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.Network;
using ClassicUO.Utility;
using Myra.Graphics2D.UI;

namespace ClassicUO.Game.UI.MyraWindows.Widgets.Assistant.Skills;

public class SkillsTabContent : VerticalStackPanel
{
    private Action? _resort;
    private Action? _rebuild;
    private MyraLabel? _totalLabel;
    private Skill[]? _skills;

    public SkillsTabContent()
    {
        var lang = Language.Instance.Assistant.Skills;

        Skill[]? skills = World.Instance?.Player?.Skills;
        if (skills == null)
            return;

        Spacing = MyraStyle.STANDARD_SPACING;

        _skills = skills;

        PlayerMobile player = World.Instance!.Player!;
        int count = skills.Length;
        int[] sortedIndices = new int[count];
        for (int i = 0; i < count; i++) sortedIndices[i] = i;

        int sortColIndex = 1;
        bool sortAscending = true;
        bool showGroups = false;

        var gridPanel = new VerticalStackPanel();

        void SortSkills() =>
            Array.Sort(sortedIndices, (a, b) =>
            {
                if (a >= skills.Length || b >= skills.Length) return 0;
                Skill? sa = skills[a];
                Skill? sb = skills[b];
                if (sa == null || sb == null) return 0;
                int cmp = sortColIndex switch
                {
                    1 => string.Compare(sa.Name, sb.Name, StringComparison.OrdinalIgnoreCase),
                    2 => sa.Value.CompareTo(sb.Value),
                    3 => sa.Base.CompareTo(sb.Base),
                    4 => sa.Cap.CompareTo(sb.Cap),
                    5 => (sa.Base - sa.BaseAtLogin).CompareTo(sb.Base - sb.BaseAtLogin),
                    6 => ((byte)sa.Lock).CompareTo((byte)sb.Lock),
                    _ => 0
                };
                return sortAscending ? cmp : -cmp;
            });

        void AddSkillRow(Skill? skill, int row, MyraGrid grid)
        {
            if (skill == null) return;

            if (skill.IsClickable)
            {
                int capturedIdx = skill.Index;
                grid.AddWidget(
                    new MyraButton(lang.ColUse, () => GameActions.UseSkill(capturedIdx))
                        { Tooltip = string.Format(lang.UseSkillTooltip, skill.Name) },
                    row, 0);
            }

            var name = new MyraLabel(skill.Name, MyraLabel.TextStyle.P);
            if (skill.IsClickable)
            {
                name.TouchDoubleClick += (_, _) => UIManager.Add(new SkillButtonGump(World.Instance, skill,
                    Input.Mouse.Position.X, Input.Mouse.Position.Y));
                name.Tooltip = string.Format(lang.DoubleClickSkillTooltip, skill.Name);
            }
            grid.AddWidget(name, row, 1);
            grid.AddWidget(new MyraLabel(skill.Value.ToString("F1"), MyraLabel.TextStyle.P), row, 2);
            grid.AddWidget(new MyraLabel(skill.Base.ToString("F1"), MyraLabel.TextStyle.P), row, 3);
            grid.AddWidget(new MyraLabel(skill.Cap.ToString("F1"), MyraLabel.TextStyle.P), row, 4);

            float delta = skill.Base - skill.BaseAtLogin;
            string deltaStr;
            if (delta > 0f)       deltaStr = $"+{delta:F1}";
            else if (delta < 0f)  deltaStr = $"{delta:F1}";
            else                  deltaStr = "0.0";
            grid.AddWidget(new MyraLabel(deltaStr, MyraLabel.TextStyle.P), row, 5);

            var lockWrapper = new HorizontalStackPanel();
            void BuildLockBtn()
            {
                lockWrapper.Widgets.Clear();
                int capturedSkillIdx = skill.Index;

                var btn = new MyraButton("", () =>
                {
                    byte nextLock = (byte)(((byte)skill.Lock + 1) % 3);
                    GameActions.ChangeSkillLockStatus((ushort)capturedSkillIdx, nextLock);
                    skill.Lock = (Lock)nextLock;
                    AsyncNetClient.Socket.Send_SkillsRequest(player.Serial);
                    BuildLockBtn();
                });
                btn.Tooltip = string.Format(lang.LockTooltip, skill.Lock);
                lockWrapper.Widgets.Add(MyraStyle.ApplySkillButtonStyle(btn, skill.Lock));
            }
            BuildLockBtn();
            grid.AddWidget(lockWrapper, row, 6);
        }

        void BuildGroupedRows(MyraGrid mainGrid)
        {
            List<SkillsGroup>? groups = World.Instance?.SkillsGroupManager?.Groups;
            if (groups == null) return;

            int skillCount = skills.Length;
            int dataRow = 1;

            foreach (SkillsGroup group in groups)
            {
                var groupIndices = new List<int>(group.Count);
                float groupTotal = 0f;

                for (int gi = 0; gi < group.Count; gi++)
                {
                    byte skillIdx = group.GetSkill(gi);
                    if (skillIdx == 0xFF || skillIdx >= skillCount) continue;
                    Skill? skill = skills[skillIdx];
                    if (skill == null) continue;
                    groupIndices.Add(skillIdx);
                    groupTotal += skill.Base;
                }

                groupIndices.Sort((a, b) =>
                {
                    Skill? sa = skills[a];
                    Skill? sb = skills[b];
                    if (sa == null || sb == null) return 0;
                    int cmp = sortColIndex switch
                    {
                        1 => string.Compare(sa.Name, sb.Name, StringComparison.OrdinalIgnoreCase),
                        2 => sa.Value.CompareTo(sb.Value),
                        3 => sa.Base.CompareTo(sb.Base),
                        4 => sa.Cap.CompareTo(sb.Cap),
                        5 => (sa.Base - sa.BaseAtLogin).CompareTo(sb.Base - sb.BaseAtLogin),
                        6 => ((byte)sa.Lock).CompareTo((byte)sb.Lock),
                        _ => 0
                    };
                    return sortAscending ? cmp : -cmp;
                });

                var groupHeader = new MyraLabel(string.Format("\u2500\u2500 {0} ({1:F1}) \u2500\u2500", group.Name, groupTotal), MyraLabel.TextStyle.H3);
                mainGrid.AddWidget(groupHeader, dataRow, 0);
                Grid.SetColumnSpan(groupHeader, 7);
                dataRow++;

                foreach (int idx in groupIndices)
                {
                    AddSkillRow(skills[idx], dataRow, mainGrid);
                    dataRow++;
                }
            }
        }

        void BuildGrid()
        {
            gridPanel.Widgets.Clear();

            var grid = new MyraGrid();
            grid.AddColumn();
            grid.AddColumn(new Proportion(ProportionType.Fill));
            grid.AddColumn(null, 5);
            MyraStyle.ApplyStandardGridStyling(grid);

            grid.AddWidget(new MyraLabel(lang.ColUse, MyraLabel.TextStyle.TableHeader), 0, 0);

            void AddSortHeader(string name, int col, int gridCol)
            {
                string indicator = sortColIndex == col ? (sortAscending ? " \u2191" : " \u2193") : "";
                grid.AddWidget(new MyraButton(name + indicator, () =>
                {
                    if (sortColIndex == col) sortAscending = !sortAscending;
                    else { sortColIndex = col; sortAscending = true; }
                    SortSkills();
                    BuildGrid();
                }), 0, gridCol);
            }

            AddSortHeader(lang.ColName,  1, 1);
            AddSortHeader(lang.ColValue, 2, 2);
            AddSortHeader(lang.ColBase,  3, 3);
            AddSortHeader(lang.ColCap,   4, 4);
            AddSortHeader(lang.ColDelta,   5, 5);
            AddSortHeader(lang.ColLock,  6, 6);

            if (showGroups)
                BuildGroupedRows(grid);
            else
                for (int i = 0; i < sortedIndices.Length; i++)
                    AddSkillRow(skills[sortedIndices[i]], i + 1, grid);

            gridPanel.Widgets.Add(grid);
        }

        var toolbar = new HorizontalStackPanel { Spacing = 4 };

        toolbar.Widgets.Add(new MyraButton(lang.AllUp, () =>
        {
            for (int i = 0; i < skills.Length; i++)
                if(skills[i].Lock != Lock.Up)
                    GameActions.ChangeSkillLockStatus((ushort)i, (byte)Lock.Up);
            AsyncNetClient.Socket.Send_SkillsRequest(player.Serial);
        }));

        toolbar.Widgets.Add(new MyraButton(lang.AllDown, () =>
        {
            for (int i = 0; i < skills.Length; i++)
                if(skills[i].Lock != Lock.Down)
                    GameActions.ChangeSkillLockStatus((ushort)i, (byte)Lock.Down);
            AsyncNetClient.Socket.Send_SkillsRequest(player.Serial);
        }));

        toolbar.Widgets.Add(new MyraButton(lang.AllLock, () =>
        {
            for (int i = 0; i < skills.Length; i++)
                if(skills[i].Lock != Lock.Locked)
                    GameActions.ChangeSkillLockStatus((ushort)i, (byte)Lock.Locked);
            AsyncNetClient.Socket.Send_SkillsRequest(player.Serial);
        }));

        toolbar.Widgets.Add(new MyraLabel("|", MyraLabel.TextStyle.P));

        toolbar.Widgets.Add(new MyraButton(lang.ResetDelta, () =>
        {
            for (int i = 0; i < skills.Length; i++)
                if (skills[i] != null) skills[i].BaseAtLogin = skills[i].Base;
            BuildGrid();
        }) { Tooltip = lang.ResetDeltaTooltip });

        toolbar.Widgets.Add(new MyraButton(lang.CopyAll, () =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name\tValue\tBase\tCap\t+/-\tLock");
            for (int i = 0; i < sortedIndices.Length; i++)
            {
                int idx = sortedIndices[i];
                if (idx >= skills.Length) continue;
                Skill? skill = skills[idx];
                if (skill == null) continue;
                float d = skill.Base - skill.BaseAtLogin;
                string lockStr = skill.Lock switch
                {
                    Lock.Up     => "Up",
                    Lock.Down   => "Down",
                    Lock.Locked => "Locked",
                    _           => "?"
                };
                sb.AppendLine($"{skill.Name}\t{skill.Value:F1}\t{skill.Base:F1}\t{skill.Cap:F1}\t{d:F1}\t{lockStr}");
            }
            sb.ToString().CopyToClipboard();
            GameActions.Print(lang.SkillsCopiedToClipboard, Constants.HUE_SUCCESS);
        }) { Tooltip = lang.CopyAllTooltip });

        toolbar.Widgets.Add(new MyraLabel("|", MyraLabel.TextStyle.P));

        toolbar.Widgets.Add(MyraCheckButton.CreateWithCallback(false, b =>
        {
            showGroups = b;
            BuildGrid();
        }, lang.ShowGroups));

        toolbar.Widgets.Add(new MyraLabel("|", MyraLabel.TextStyle.P));

        _resort = SortSkills;
        _rebuild = BuildGrid;

        SortSkills();
        BuildGrid();

        float baseSum = 0f, capSum = 0f;
        for (int i = 0; i < skills.Length; i++)
            if (skills[i] != null) { baseSum += skills[i].Base; capSum += skills[i].Cap; }
        _totalLabel = new MyraLabel(string.Format(lang.Total, baseSum, capSum), MyraLabel.TextStyle.P);
        toolbar.Widgets.Add(_totalLabel);

        Widgets.Add(toolbar);
        Widgets.Add(new ScrollViewer { MaxHeight = 500, Content = gridPanel });
    }

    public void UpdateSkills()
    {
        var lang = Language.Instance.Assistant.Skills;

        _resort?.Invoke();
        _rebuild?.Invoke();

        if (_totalLabel != null && _skills != null)
        {
            float baseSum = 0f, capSum = 0f;
            for (int i = 0; i < _skills.Length; i++)
                if (_skills[i] != null) { baseSum += _skills[i].Base; capSum += _skills[i].Cap; }
            _totalLabel.Text = string.Format(lang.Total, baseSum, capSum);
        }
    }
}
