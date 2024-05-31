using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace elhodel.EasyEditorSettings
{

    public class ScriptableSingletonProvider : SettingsProvider
    {

        private List<PropertyField> _propertyFields;
        private SerializedObject _serializedObject;
        private Action _saveAction;
        public ScriptableSingletonProvider(SerializedObject serializedObject, Action saveAction, string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
            _saveAction = saveAction;
            _serializedObject = serializedObject;
            GetSearchKeywordsFromSerializedObject(serializedObject);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            _propertyFields = new List<PropertyField>();

            var settings = _serializedObject;

            // rootElement is a VisualElement. If you add any children to it, the OnGUI function
            // isn't called because the SettingsProvider uses the UIElements drawing framework.
            //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/settings_ui.uss");
            //rootElement.styleSheets.Add(styleSheet);
            var title = new Label()
            {
                text = label
            };
            title.AddToClassList("title");
            rootElement.Add(title);

            var properties = new VisualElement()
            {
                style =
                    {
                        flexDirection = FlexDirection.Column
                    }
            };
            properties.AddToClassList("property-list");
            rootElement.Add(properties);

            var iterator = settings.GetIterator();
            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                PropertyField field = new PropertyField(iterator);
                field.RegisterValueChangeCallback(OnAnyValueChanged);
                properties.Add(field);
                _propertyFields.Add(field);

            }

            rootElement.Bind(settings);
        }

        private void OnAnyValueChanged(SerializedPropertyChangeEvent evt)
        {
            _saveAction();
        }

        public override void OnDeactivate()
        {
            if (_propertyFields == null || _propertyFields.Count == 0)
            {
                return;
            }
            foreach (var propertyField in _propertyFields)
            {
                propertyField.UnregisterCallback<SerializedPropertyChangeEvent>(OnAnyValueChanged);
            }
        }




    }

}