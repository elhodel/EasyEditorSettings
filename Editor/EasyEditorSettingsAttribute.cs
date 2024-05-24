using System;
using UnityEditor;

namespace elhodel.EasyEditorSettings
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EasyEditorSettingsAttribute : Attribute
    {
        private string _savePath;
        private string _menuPath;
        private FilePathAttribute.Location _location;

        public EasyEditorSettingsAttribute(string savePath, string menuPath, FilePathAttribute.Location location)
        {
            _savePath = savePath;
            _menuPath = menuPath;
            _location = location;
        }
    }

}