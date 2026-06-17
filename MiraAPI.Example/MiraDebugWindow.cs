using System;
using Reactor.Utilities.ImGui;
using UnityEngine;

namespace MiraAPI.Example;

public class MiraDebugWindow : MonoBehaviour
{
    public DragWindow DebuggingWindow { get; } = new(
        new Rect(10, 10, 0, 0),
        "MIRA API DEBUGGING",
        () =>
        {
        })
    {
        Enabled = true,
    };

    public void OnGUI()
    {
        DebuggingWindow.OnGUI();
    }
}
