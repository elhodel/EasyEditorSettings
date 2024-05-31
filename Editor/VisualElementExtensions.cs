using UnityEngine.UIElements;

namespace elhodel.EasyEditorSettings
{
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