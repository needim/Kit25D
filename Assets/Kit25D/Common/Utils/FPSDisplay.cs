using UnityEngine;

namespace Kit25D
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class FPSDisplay : MonoBehaviour
    {
        public Color color = new Color(1f, 1f, 1f, 1.0f);
        float deltaTime = 0.0f;

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(5, 5, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 50;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = color;
            float fps = 1.0f / deltaTime;
            string text = Mathf.RoundToInt(fps).ToString();
            GUI.Label(rect, text, style);
        }
    }
}