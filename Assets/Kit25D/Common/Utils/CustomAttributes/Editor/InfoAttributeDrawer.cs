using UnityEditor;
using UnityEngine;

namespace Kit25D.CustomAttributes
{
    [CustomPropertyDrawer(typeof(InfoAttribute))]
    public class InfoAttributeDrawer : DecoratorDrawer
    {
        InfoAttribute infoAttribute
        {
            get { return ((InfoAttribute) attribute); }
        }

        GUIStyle style
        {
            get
            {
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.fontSize = 10;
                style.richText = true;
                style.wordWrap = true;

                return style;
            }
        }

        GUIContent content
        {
            get
            {
                return new GUIContent("↓ " + infoAttribute.message);
            }
        }

        float propHeight
        {
            get
            {
                return style.CalcHeight(content, EditorGUIUtility.currentViewWidth) + 12;
            }
        }

        public override float GetHeight()
        {
            return propHeight;
        }

        public override void OnGUI(Rect rect)
        {
            rect.y += 10;
            rect.height = propHeight;

            EditorGUI.LabelField(rect, content, style);
        }
    }
}