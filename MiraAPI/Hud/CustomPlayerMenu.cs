using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AmongUs.GameOptions;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace MiraAPI.Hud;

/// <summary>
/// Custom Player Menu using the ShapeshifterPanel as a base.
/// </summary>
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Unity Convention")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity Convention")]
public class CustomPlayerMenu : Minigame
{
    public ShapeshifterPanel panelPrefab;
    public float xStart = -0.8f;
    public float yStart = 2.15f;
    public float xOffset = 1.95f;
    public float yOffset = -0.65f;
    public UiElement backButton;
    public UiElement defaultButtonSelected;
    public List<ShapeshifterPanel> potentialVictims;

    /// <summary>
    /// Creates a CustomPlayerMenu.
    /// </summary>
    /// <returns>New CustomPlayerMenu object.</returns>
    public static CustomPlayerMenu Create()
    {
        var shapeShifterRole = RoleManager.Instance.GetRole(RoleTypes.Shapeshifter);

        var ogMenu = (shapeShifterRole as ShapeshifterRole)!.ShapeshifterMenu;
        var newMenu = Instantiate(ogMenu);
        var customMenu = newMenu.gameObject.AddComponent<CustomPlayerMenu>();

        customMenu.panelPrefab = newMenu.PanelPrefab;
        customMenu.xStart = newMenu.XStart;
        customMenu.yStart = newMenu.YStart;
        customMenu.xOffset = newMenu.XOffset;
        customMenu.yOffset = newMenu.YOffset;
        customMenu.backButton = newMenu.BackButton;
        var back = customMenu.backButton.GetComponent<PassiveButton>();
        back.OnClick.RemoveAllListeners();
        back.OnClick.AddListener((UnityAction)(() =>
        {
            Instance.Close();
        }));

        customMenu.CloseSound = newMenu.CloseSound;
        // TODO: publicize mono gamelibs
        //customMenu.logger = newMenu.logger;
        customMenu.OpenSound = newMenu.OpenSound;

        newMenu.DestroyImmediate();

        customMenu.transform.SetParent(Camera.main!.transform, false);
        customMenu.transform.localPosition = new Vector3(0f, 0f, -50f);
        return customMenu;
    }

    private void OnDisable()
    {
        ControllerManager.Instance.CloseOverlayMenu(name);
    }

    /// <inheritdoc />
    public override void Begin(PlayerTask task)
    {
        throw new NotImplementedException("Use the other Begin method.");
    }

    /// <summary>
    /// Begins/opens the custom player menu.
    /// </summary>
    /// <param name="playerMatch">Function to determine if player should show in the custom menu.</param>
    /// <param name="onClick">Onclick action for player.</param>
    public void Begin(Func<PlayerControl, bool> playerMatch, Action<PlayerControl?> onClick)
    {
        MinigameStubs.Begin(this, null);

        var back = backButton.GetComponent<PassiveButton>();
        back.OnClick.AddListener((UnityAction)(() =>
        {
            onClick(null);
        }));

        DestroyableSingleton<DebugAnalytics>.Instance.Analytics.MinigameOpened(PlayerControl.LocalPlayer.Data, TaskType);
        var list = PlayerControl.AllPlayerControls.Where(playerMatch).ToList();
        potentialVictims = [];
        var list2 = new List<UiElement>();

        for (var i = 0; i < list.Count; i++)
        {
            var player = list[i];
            var num = i % 3;
            var num2 = i / 3;
            var flag = PlayerControl.LocalPlayer.Data.Role.NameColor == player.Data.Role.NameColor;
            var shapeshifterPanel = Instantiate(panelPrefab, transform);
            shapeshifterPanel.transform.localPosition = new Vector3(xStart + num * xOffset, yStart + num2 * yOffset, -1f);
            shapeshifterPanel.SetPlayer(i, player.Data, () => { onClick(player); });
            shapeshifterPanel.NameText.color = flag ? player.Data.Role.NameColor : Color.white;
            potentialVictims.Add(shapeshifterPanel);
            list2.Add(shapeshifterPanel.Button);
        }
        ControllerManager.Instance.OpenOverlayMenu(name, backButton, defaultButtonSelected, list2);
    }
}
