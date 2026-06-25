using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace MiraAPI.Patches;

[HarmonyPatch(typeof(IntroCutscene))]
public static class IntroCutscenePatches
{
    /*
    [HarmonyPostfix]
    [HarmonyPatch(nameof(IntroCutscene.BeginImpostor))]
    public static void BeginImpostorPatch(IntroCutscene __instance)
    {
        if (CustomGameModeManager.ActiveMode != null && CustomGameModeManager.ActiveMode.ShowCustomRoleScreen())
        {
            var mode = CustomGameModeManager.ActiveMode;
            __instance.TeamTitle.text = $"<size=70%>{mode.Name}</size>\n<size=20%>{mode.Description}</size>";
        }
    }*/

    [HarmonyPostfix]
    [HarmonyPatch(nameof(IntroCutscene.CoBegin))]
    public static void IntroBeginPatch(IntroCutscene __instance)
    {
        var @event = new IntroBeginEvent(__instance);
        MiraEventManager.InvokeEvent(@event);
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowRole))]
    public static class IntroCutsceneShowRolePatch
    {
        public static void Prefix(IntroCutscene __instance)
        {
            Info("IntroCutscene ShowRole reached");
        
            var @event = new IntroRoleRevealEvent(__instance);
            MiraEventManager.InvokeEvent(@event);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(IntroCutscene.BeginImpostor))]
    [HarmonyPatch(nameof(IntroCutscene.BeginCrewmate))]
    public static bool BeginPrefix(IntroCutscene __instance, [HarmonyArgument(0)] ref List<PlayerControl> yourTeam)
    {
        return PlayerControl.LocalPlayer.Data.Role is not ICustomRole customRole || customRole.SetupIntroTeam(__instance, ref yourTeam);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(IntroCutscene.BeginImpostor))]
    [HarmonyPatch(nameof(IntroCutscene.BeginCrewmate))]
    public static void BeginPostfix(IntroCutscene __instance)
    {
        if (PlayerControl.LocalPlayer.Data.Role is not ICustomRole customRole)
        {
            return;
        }

        if (customRole.IntroConfiguration is { } introConfig)
        {
            __instance.BackgroundBar.material.SetColor(ShaderID.Color, introConfig.IntroTeamColor);
            __instance.TeamTitle.color = introConfig.IntroTeamColor;
            __instance.TeamTitle.text = introConfig.IntroTeamTitle;
            __instance.ImpostorText.text = introConfig.IntroTeamDescription;
        }
    }

    [HarmonyPatch]
    public static class IntroCutsceneDestroyPatch
    {
        private static bool _usedFallback;

        public static MethodBase TargetMethod()
        {
            // For now, we just force the fallback
            
            _usedFallback = true;
            return AccessTools.Method(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin));
        }
        
        public static void Postfix(IntroCutscene __instance, ref IEnumerator __result)
        {
            if (!_usedFallback)
            {
                TriggerIntroEndEvents(__instance);
                return;
            }

            if (__result == null)
            {
                TriggerIntroEndEvents(__instance);
                return;
            }

            __result = Helpers.CreateWrapper(__result, () => TriggerIntroEndEvents(__instance));
        }
        
        private static void TriggerIntroEndEvents(IntroCutscene introCutscene)
        {
            Info("IntroCutscene ended");

            MiraEventManager.InvokeEvent(new IntroEndEvent(introCutscene));

            var @event = new BeforeRoundStartEvent(true);
            MiraEventManager.InvokeEvent(@event);

            if (@event.IsCancelled) return;
            MiraEventManager.InvokeEvent(new RoundStartEvent(true));
        }
    }
}
