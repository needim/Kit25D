using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kit25D.CustomAttributes
{
    public class InfoAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public string message;
        public MessageType type;

        public InfoAttribute(string message)
        {
            this.message = message;
            this.type = UnityEditor.MessageType.None;
        }
#else
		public InfoAttribute(string message)
		{

		}
#endif
    }
}