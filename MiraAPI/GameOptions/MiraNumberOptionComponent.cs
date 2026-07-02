using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace MiraAPI.GameOptions;

/// <summary>
/// Component added onto options generated via MiraAPI. This allows developers to hook patches onto options in general, without using a list.
/// </summary>
public class MiraNumberOptionComponent : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the <see cref="ModdedNumberOption"/> associated with the object.
    /// </summary>
    public ModdedNumberOption NumberOption { get; set; }

    /// <summary>
    /// Gets or sets the default increment.
    /// </summary>
    public float DefaultIncrement { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether holding shift will split the increment in half.
    /// </summary>
    public bool ShiftIncrementToggle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating what Zero displays.
    /// </summary>
    public string ZeroValue { get; set; } = "#";

    /// <summary>
    /// Gets or sets a value indicating what Negative One displays.
    /// </summary>
    public string NegativeValue { get; set; } = "#";
}
