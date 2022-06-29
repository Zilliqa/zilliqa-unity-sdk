using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerTextColors
{
    public const string defaultLowHex = "#E94F64";
    public const string defaultHighHex = "#52D273";
    public static Color GetColor(Color lowColor, Color highColor, float lowValue, float highValue, float value)
    {
        var hlDiff = highValue - lowValue;

        if (hlDiff == 0)
            return highColor;



        var hvDiff = highValue - value;
        var ratio = 1 - (hvDiff / hlDiff);

        return Color.Lerp(lowColor, highColor, ratio);
    }

    public static Color GetColor(float value, float lowValue, float highValue, string lowColorHex = defaultLowHex, string highColorHex = defaultHighHex, bool higherIsBetter = true)
    {
        Color lowColor, highColor;

        var hlDiff = highValue - lowValue;

        if (hlDiff == 0)
        {
            ColorUtility.TryParseHtmlString(highColorHex, out highColor);
            return highColor;
        }

        var hvDiff = highValue - value;

        var ratio = higherIsBetter ? hvDiff / hlDiff : 1 - (hvDiff / hlDiff);

       
        if (!ColorUtility.TryParseHtmlString(lowColorHex, out lowColor) ||
            !ColorUtility.TryParseHtmlString(highColorHex, out highColor))
            return Color.white;

        return Color.Lerp(lowColor, highColor, ratio);
    }

    public static string GetColorHex(float value, float lowValue, float highValue, string lowColorHex = defaultLowHex, string highColorHex = defaultHighHex, bool higherIsBetter = true)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(GetColor(value, lowValue, highValue, lowColorHex, highColorHex, higherIsBetter));
    }

    public static string GetRichText(float value, float lowValue, float highValue, Color lowColor, Color highColor, bool higherIsBetter)
    {
        var hlDiff = highValue - lowValue;

        if (hlDiff == 0)
            return "<color=#" + highColor + ">" + value + "</color>";

        var hvDiff = highValue - value;

        var ratio = higherIsBetter ? hvDiff / hlDiff : 1 - (hvDiff / hlDiff);

        var color = ColorUtility.ToHtmlStringRGBA(Color.Lerp(lowColor, highColor, ratio));

        return "<color=#" + color + ">" + value + "</color>";

    }

    public static string GetRichText(float value, float lowValue, float highValue, string lowColorHex = defaultLowHex, string highColorHex = defaultHighHex, bool higherIsBetter = true)
    {
        Color lowColor, highColor;

        var hlDiff = highValue - lowValue;

        if (hlDiff == 0)
        {
            ColorUtility.TryParseHtmlString(highColorHex, out highColor);
            return "<color=#" + highColor + ">" + value + "</color>";
        }
        

        var hvDiff = highValue - value;

        var ratio = higherIsBetter ? hvDiff / hlDiff : 1 - (hvDiff / hlDiff);

       
        if (!ColorUtility.TryParseHtmlString(lowColorHex, out lowColor) ||
            !ColorUtility.TryParseHtmlString(highColorHex, out highColor))
            return "" + value;

        var color = ColorUtility.ToHtmlStringRGBA(Color.Lerp(lowColor, highColor, ratio));

        return "<color=" + color + ">" + value + "</color>";

    }

    public static string SetColor(string str, string colorHex) => "<color=" + colorHex + ">" + str + "</color>";
}
