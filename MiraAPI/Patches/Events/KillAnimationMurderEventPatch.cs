using System.Reflection;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Utilities;
using UnityEngine;

namespace MiraAPI.Patches.Events;

/// <summary>
/// Patch to invoke AfterMurderEvent after a kill animation is performed.
/// </summary>
[HarmonyPatch]
public static class KillAnimationMurderEventPatch
{
    public static MethodBase TargetMethod()
    {
        return Helpers.GetStateMachineMoveNext<KillAnimation>(nameof(KillAnimation.CoPerformKill))!;
    }

    public static void Postfix(KillAnimation __instance)
    {
        var wrapper = new StateMachineWrapper<KillAnimation>(__instance);
        if (wrapper.GetState() != 1)
        {
            return;
        }

        var source = wrapper.GetParameter<PlayerControl>("source");
        var target = wrapper.GetParameter<PlayerControl>("target");

        var afterMurderEvent = new AfterMurderEvent(source, target, Helpers.GetBodyById(target.PlayerId));
        MiraEventManager.InvokeEvent(afterMurderEvent);
    }
}
