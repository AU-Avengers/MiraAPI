using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace MiraAPI.Colors;

/// <summary>
/// Used to register and track <see cref="CustomColor"/>s.
/// </summary>
public static class PaletteManager
{
    /// <summary>
    /// Gets all registered <see cref="CustomColor"/>s.
    /// </summary>
    public static CustomColor[] RegisteredColors => [.. CustomColors];

    internal static readonly List<CustomColor> CustomColors = [];

    internal static void RegisterAllColors()
    {
        var colors = CustomColors.Select(x => x.MainColor).ToArray();
        var shadowColors = CustomColors.Select(x => x.ShadowColor).ToArray();
        var stringNames = CustomColors.Select(x => x.Name).ToArray();

        // TODO: publicize mono gamelibs (these are all internal)
        /*Palette.PlayerColors = Palette.PlayerColors.ToArray().AddRangeToArray(colors);
        Palette.ShadowColors = Palette.ShadowColors.ToArray().AddRangeToArray(shadowColors);
        Palette.ColorNames = Palette.ColorNames.ToArray().AddRangeToArray(stringNames);

        Palette.TextColors = Palette.TextColors.ToArray().AddRangeToArray(colors);
        Palette.TextOutlineColors = Palette.TextOutlineColors.ToArray().AddRangeToArray(shadowColors);*/

        var plrCols = typeof(Palette).GetField("PlayerColors", BindingFlags.Public | BindingFlags.Static);
        if (plrCols != null)
        {
            plrCols.SetValue(
                null,
                Palette.PlayerColors
                    .Concat(colors)
                    .Distinct()
                    .ToArray());
        }

        var shadowCols = typeof(Palette).GetField("ShadowColors", BindingFlags.Public | BindingFlags.Static);
        if (shadowCols != null)
        {
            shadowCols.SetValue(
                null,
                Palette.ShadowColors
                    .Concat(shadowColors)
                    .Distinct()
                    .ToArray());
        }

        var names = typeof(Palette).GetField("ColorNames", BindingFlags.Public | BindingFlags.Static);
        if (names != null)
        {
            names.SetValue(
                null,
                Palette.ColorNames
                    .Concat(stringNames)
                    .Distinct()
                    .ToArray());
        }

        var textColor = typeof(Palette).GetField("TextColors", BindingFlags.Public | BindingFlags.Static);
        if (textColor != null)
        {
            textColor.SetValue(
                null,
                Palette.TextColors
                    .Concat(colors)
                    .Distinct()
                    .ToArray());
        }
        var outlineColor = typeof(Palette).GetField("TextOutlineColors", BindingFlags.Public | BindingFlags.Static);
        if (outlineColor != null)
        {
            outlineColor.SetValue(
                null,
                Palette.TextOutlineColors
                    .Concat(shadowColors)
                    .Distinct()
                    .ToArray());
        }
    }
}
