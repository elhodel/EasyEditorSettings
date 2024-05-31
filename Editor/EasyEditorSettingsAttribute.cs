using System;
using UnityEditor;


[AttributeUsage(AttributeTargets.Class)]
public class EasyEditorSettingsAttribute : Attribute
{
    public string SavePath;
    public string MenuPath;
    public FilePathAttribute.Location Location;

    public EasyEditorSettingsAttribute(string savePath, string menuPath, FilePathAttribute.Location location)
    {
        SavePath = savePath;
        MenuPath = menuPath;
        Location = location;
    }
}
