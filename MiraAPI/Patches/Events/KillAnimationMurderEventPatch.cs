using System.Reflection;
using System.Collections;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Utilities;
using UnityEngine;

namespace MiraAPI.Patches.Events;

/// <summary>
/// Patch to invoke <see cref="AfterMurderEvent"/> after a kill animation is performed.
/// </summary>
[HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
public static class KillAnimationMurderEventPatch
{
    public static void Postfix(KillAnimation __instance, ref IEnumerator __result, PlayerControl source, PlayerControl target)
    {
        __result = Helpers.CreateWrapper(__result, () => 
        {
            var deadBody = Helpers.GetBodyById(target.PlayerId);
            var afterMurderEvent = new AfterMurderEvent(source, target, deadBody);
        
            MiraEventManager.InvokeEvent(afterMurderEvent);
        });
    }
}