using UnityEditor;
using UnityEngine.UIElements;

namespace elhodel.EasyEditorSettings
{
    public static class UiUtils
    {
        public static Image GetLogIconImage(string iconName)
        {
            var texture = EditorGUIUtility.IconContent(iconName).image;

            var image = new Image { image = texture };
            image.style.width = EditorGUIUtility.singleLineHeight;
            return image;
        }
    }

    public static class VisualElementExtensions
    {
      
        public static void SetPadding(this IStyle style, StyleLength padding)
        {
            style.paddingBottom = padding;
            style.paddingLeft = padding;
            style.paddingRight = padding;
            style.paddingTop = padding;
        }
    }

}