using System.Collections;
using HarmonyLib;
using InnerNet;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.LocalSettings;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.Patches;

/// <summary>
/// Sync all settings to the player when they join the game.
/// </summary>
[HarmonyPatch(typeof(AmongUsClient))]
internal static class AmongUsClientSyncPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CreatePlayer))]
    public static void CreatePlayerPatch(ClientData clientData)
    {
        var joinEvent = new PlayerJoinEvent(clientData);
        MiraEventManager.InvokeEvent(joinEvent);

        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }

        if (clientData.Id == AmongUsClient.Instance.HostId)
        {
            return;
        }

        ModdedOptionsManager.SyncAllOptions(clientData.Id);
        CustomRoleManager.SyncAllRoleSettings(clientData.Id);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    public static void PlayerLeftPatch(ClientData data, DisconnectReasons reason)
    {
        var leftEvent = new PlayerLeaveEvent(data, reason);
        MiraEventManager.InvokeEvent(leftEvent);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
    public static void PlayerAwake(AmongUsClient __instance)
    {
        __instance.StartCoroutine(SetFps());
        // TODO: Fix addressable loader!
        // AddressablesLoader.LoadAll();
    }

    private static IEnumerator SetFps()
    {
        Application.targetFrameRate = (int)LocalSettingsTabSingleton<MiraApiSettings>.Instance.SetFpsSlider.Value;
        yield return new WaitForSeconds(1f);

        Application.targetFrameRate = (int)LocalSettingsTabSingleton<MiraApiSettings>.Instance.SetFpsSlider.Value;
    }
}
