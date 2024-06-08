using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace elhodel.EasyEditorSettings
{

    public class ScriptableSingletonSettingsProvider : SettingsProvider
    {
        private const string _styleSheetAssetGuid = "5bdbd6e786df7164d8dba40b87172110";


        private List<PropertyField> _propertyFields;
        private SerializedObject _serializedObject;
        private Action _saveAction;

        Dictionary<string, MethodInfo> _contextMenuItems = new Dictionary<string, MethodInfo>();
        public ScriptableSingletonSettingsProvider(SerializedObject serializedObject, Action saveAction, string path, SettingsScope scopes) : base(path, scopes, null)
        {
            _saveAction = saveAction;
            _serializedObject = serializedObject;
            this.keywords = GetSearchKeywordsFromSerializedObject(serializedObject);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(_styleSheetAssetGuid));

            if (stylesheet != null)
            {
                rootElement.styleSheets.Add(stylesheet);
            }
            rootElement.AddToClassList("settings-provider");

            _propertyFields = new List<PropertyField>();

            var settings = _serializedObject;

            var titleContainer = new VisualElement();
            titleContainer.AddToClassList("title-container");
            // rootElement is a VisualElement. If you add any children to it, the OnGUI function
            // isn't called because the SettingsProvider uses the UIElements drawing framework.
            //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/settings_ui.uss");
            //rootElement.styleSheets.Add(styleSheet);
            var title = new Label()
            {
                text = label
            };
            title.AddToClassList("title");
            titleContainer.Add(title);

            Button contextMenuButton = new Button(OpenContextMenu);
            contextMenuButton.RemoveFromClassList("unity-button");
            contextMenuButton.AddToClassList("context-menu");
            contextMenuButton.style.maxWidth = 18;

            contextMenuButton.Add(UiUtils.GetLogIconImage("_Menu"));

            titleContainer.Add(contextMenuButton);

            rootElement.Add(titleContainer);
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

            _contextMenuItems = GetContextMenuEntries(_serializedObject.targetObject.GetType());

            if(_contextMenuItems.Count == 0)
            {
                contextMenuButton.style.display = DisplayStyle.None;
            }
        }

        private static Dictionary<string, MethodInfo> GetContextMenuEntries(Type type)
        {
            var contextMenuEntries = new Dictionary<string, MethodInfo>();

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


            foreach (var method in methods)
            {
                object[] attributes = method.GetCustomAttributes(typeof(ContextMenu), false);

                if (attributes.Length == 0)
                {
                    continue;
                }

                ContextMenu[] contextMenus = attributes.OfType<ContextMenu>().ToArray();

                foreach (var contextMenu in contextMenus)
                {
                    if (!contextMenu.validate)
                    {
                        contextMenuEntries.Add(contextMenu.menuItem, method);

                    }
                }
            }

            return contextMenuEntries;
        }

        private void OpenContextMenu()
        {
            // create the menu and add items to it
            GenericMenu menu = new GenericMenu();

            foreach (var menuItem in _contextMenuItems)
            {
                menu.AddItem(new GUIContent(menuItem.Key), false, () => menuItem.Value.Invoke(_serializedObject.targetObject, null));

            }
            // display the menu
            menu.ShowAsContext();
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