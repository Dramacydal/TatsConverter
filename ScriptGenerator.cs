using System.Text.RegularExpressions;

namespace TatsConverter;

public static class ScriptGenerator
{
    private static string _template =
"""
Scriptname <script_name> extends RaceMenuBase

Event OnWarpaintRequest()
    
<war_paint>
    
EndEvent

Event OnBodyPaintRequest()
    
<body_paint>
     
EndEvent

Event OnHandPaintRequest()
    
<hands_paint>
    
EndEvent

Event OnFeetPaintRequest()
    
<feet_paint>
    
EndEvent

Event OnFacePaintRequest()
    
<face_paint>
    
EndEvent
""";

    public static string Generate(string scriptName, string contextName, string textureRelativePath, IEnumerable<TattooData> tattooData)
    {
        var content = _template;

        content = content.Replace("<war_paint>", "");
        content = content.Replace("<script_name>", scriptName);

        foreach (var part in new List<string> { "body", "face", "hands", "feet" })
        {
            content = content.Replace($"<{part}_paint>",
                string.Join("\r\n",
                    tattooData.Where(t => t.Area.ToLower() == part)
                        .Select(t => "    " + GenerateLine(contextName, textureRelativePath, t))));
        }

        return content;
    }

    private static string EscapeString(string s)
    {
        return Regex.Replace(s, "([\\\\\"])", "\\$1");
    }

    private static string GenerateLine(string contextName, string textureRelativePath, TattooData data)
    {
        string function;
        switch (data.Area.ToLower())
        {
            case "body":
                function = "AddBodyPaint";
                break;
            case "face":
                function = "AddFacePaint";
                break;
            case "hands":
                function = "AddHandPaint";
                break;
            case "feet":
                function = "AddFeetPaint";
                break;
            default:
                throw new Exception($"Unknown tattoo section: {data.Area}");

        }

        var paintName = $"{data.Section} - {data.Name}";
        if (!string.IsNullOrEmpty(contextName))
            paintName = $"{contextName} - {paintName}";
        
        return GenerateLine(function, paintName, Path.Combine(textureRelativePath, data.Texture));
    }

    private static string GenerateLine(string function, string name, string texturePath)
    {
        return $"{function}(\"" + EscapeString(name) + "\", \"" + EscapeString(texturePath) + "\")";
    }
}