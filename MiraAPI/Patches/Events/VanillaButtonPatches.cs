using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Usables;

namespace MiraAPI.Patches.Events;

/// <summary>
/// Patches to invoke <see cref="ActionButton"/> related <see cref="MiraEvent"/>s.
/// </summary>
[HarmonyPatch]
public static class VanillaButtonPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static bool DoClickPrefix(AbilityButton __instance)
    {
        // Invoke the generic button click event.
        PlayerControl playerTarget = null;
        Vent ventTarget = null;
        var role = PlayerControl.LocalPlayer.Data.Role;
        if (role.Role is RoleTypes.Tracker)
        {
            playerTarget = ((TrackerRole)role).currentTarget;
        }
        else if (role.Role is RoleTypes.GuardianAngel)
        {
            playerTarget = ((GuardianAngelRole)role).currentTarget;
        }
        else if (role.Role is RoleTypes.Engineer)
        {
            ventTarget = ((EngineerRole)role).currentTarget;
        }
        var genericEvent = new VanillaButtonClickEvent(__instance, playerTarget, ventTarget);
        MiraEventManager.InvokeEvent(genericEvent);
        if (genericEvent.IsCancelled)
        {
            MiraEventManager.InvokeEvent(new VanillaButtonCancelledEvent(__instance));
        }

        if (!genericEvent.IsCancelled)
        {
            return true;
        }

        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SecondaryAbilityButton), nameof(SecondaryAbilityButton.DoClick))]
    public static bool SecondaryDoClickPrefix(AbilityButton __instance)
    {
        // Invoke the generic button click event.
        PlayerControl playerTarget = null;
        var role = PlayerControl.LocalPlayer.Data.Role;
        if (role.Role is RoleTypes.Detective)
        {
            playerTarget = ((DetectiveRole)role).currentTarget;
        }
        var genericEvent = new VanillaButtonClickEvent(__instance, playerTarget);
        MiraEventManager.InvokeEvent(genericEvent);
        if (genericEvent.IsCancelled)
        {
            MiraEventManager.InvokeEvent(new VanillaButtonCancelledEvent(__instance));
        }

        if (!genericEvent.IsCancelled)
        {
            return true;
        }

        return false;
    }
}
