using System.Collections;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Utilities;

namespace MiraAPI.Patches.Events;

[HarmonyPatch(typeof(TutorialManager), nameof(TutorialManager.RunTutorial))]
public static class FreeplayRoundStartPatch
{
    public static void Postfix(ref IEnumerator __result)
    {
        __result = Helpers.CreateWrapper(__result, () => MiraEventManager.InvokeEvent(new RoundStartEvent(true)));
    }
}