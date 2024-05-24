using System;
using UnityEditor;

namespace elhodel.EasyEditorSettings
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EasyEditorSettingsAttribute : Attribute
    {
        private string _savePath;
        private string _menuPath;
        private SettingsScope _scope;

        public EasyEditorSettingsAttribute(SettingsScope scope,  string savePath, string menuPath)
        {
            _savePath = savePath;
            _menuPath = menuPath;
            _scope = scope;
        }
    }

}