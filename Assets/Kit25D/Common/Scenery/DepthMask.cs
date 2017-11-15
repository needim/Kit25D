using UnityEngine;

namespace Kit25D
{
    [DisallowMultipleComponent]
    public class DepthMask : MonoBehaviour
    {
        private float _z;

        void Start()
        {
            _z = transform.parent.position.z;
        }

#if UNITY_EDITOR
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        void Reset()
        {
            Collider2D _c = GetComponent<Collider2D>();

            if (_c == null)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Choose a component", "You are missing one of the required componets. Please choose one to add.", "BoxCollider2D", "PolygonCollider2D"))
                    _c = gameObject.AddComponent<BoxCollider2D>();
                else
                    _c = gameObject.AddComponent<PolygonCollider2D>();
            }

            _c.isTrigger = true;
        }
#endif

        void OnTriggerStay2D(Collider2D coll)
        {
            if (coll.CompareTag("Player"))
            {
                CharacterMotor motor = coll.gameObject.GetComponent<CharacterMotor>();
                CharacterSorter charSorter = coll.gameObject.GetComponent<CharacterSorter>();

                if (!motor.onRoof)
                    charSorter.depthOverride = _z + 0.15f;
                else
                    charSorter.depthOverride = 0f;
            }
        }

        void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.CompareTag("Player"))
            {
                CharacterSorter charSorter = coll.gameObject.GetComponent<CharacterSorter>();
                charSorter.depthOverride = 0f;
            }
        }
    }
}