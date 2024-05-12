using System.Collections;
using UnityEngine;
using UnityEditor;
using test.something.Hello.Blah;



[EasyEditorSettings]
[FilePath("ProjectSettings/Tools/TestSettings.asset", FilePathAttribute.Location.ProjectFolder)]
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
