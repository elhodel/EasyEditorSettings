using UnityEngine;
using UnityEditor;
using test.something.Hello.Blah;


namespace elhodel.Tools
{

    [EasyEditorSettings("ProjectSettings/Tools/OtherSettings.asset", "Tools/Other Settings", FilePathAttribute.Location.ProjectFolder)]

    public partial class OtherToolSettings : ScriptableSingleton<OtherToolSettings>
    {
        [Header("Test")]
        [SerializeField]
        private Vector3 _value;

        [SerializeField]
        private TestClass _testValue;

        [SerializeField]
        private float _floatValue;
    }


}