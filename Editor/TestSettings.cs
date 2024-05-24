using System.Collections;
using UnityEngine;
using UnityEditor;
using test.something.Hello.Blah;



[EasyEditorSettings("ProjectSettings/Tools/TestSettings.asset", "Tools/Test Settings", FilePathAttribute.Location.ProjectFolder)]
public partial class TestSettings : ScriptableSingleton<TestSettings>
{
   
    [Header("Test")]
    [SerializeField]
    private Vector3 _value;

    [SerializeField]
    private TestClass _testValue;

    [SerializeField]
    private float _floatValue;

}
