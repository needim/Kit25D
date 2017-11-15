using Kit25D.CustomAttributes;
using UnityEngine;

namespace Kit25D
{
    [RequireComponent(typeof(SpriteSorter))]
    [DisallowMultipleComponent]
    public class SceneryCollider : MonoBehaviour
    {
        public bool hasRoof = false;
        [Info("It's for floating platforms. Has a little different behaviour then hasRoof.")]
        public bool isPlatform = false;
        public float height { get; private set; }
        private SpriteSorter spriteSorter;

#if UNITY_EDITOR
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        void Reset()
        {
            Collider2D _c = GetComponent<Collider2D>();

            if (_c == null)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Choose a component", "You are missing one of the required componets. Please choose one to add.", "CapsuleCollider2D", "PolygonCollider2D"))
                    _c = gameObject.AddComponent<CapsuleCollider2D>();
                else
                    _c = gameObject.AddComponent<PolygonCollider2D>();
            }

            _c.isTrigger = true;
        }
#endif

        void Start()
        {
            if (transform.parent)
                spriteSorter = transform.parent.GetComponent<SpriteSorter>();

            if (!spriteSorter)
                spriteSorter = transform.GetComponent<SpriteSorter>();

            height = spriteSorter.height + Mathf.Abs(spriteSorter.groundOffset);

            if (isPlatform)
                hasRoof = true;
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.CompareTag("Player"))
            {
                CharacterMotor motor = coll.gameObject.GetComponent<CharacterMotor>();

                if (!motor.onGround && motor.zHeight > height)
                {
                    motor.Shadows.SetShadowHeightModifier(height);
                    motor.roofHeight = height;
                    motor.onRoof = true;
                    motor.onPlatform = isPlatform;
                }
            }
        }

        void OnTriggerStay2D(Collider2D coll)
        {
            if (isPlatform && coll.CompareTag("Player"))
            {
                CharacterMotor motor = coll.gameObject.GetComponent<CharacterMotor>();

                if (motor.onGround && !motor.onRoof && !motor.underPlatform)
                {
                    motor.underPlatform = true;
                    motor.OnUnderPlatform();
                }

            }
        }

        void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.CompareTag("Player"))
            {
                CharacterMotor motor = coll.gameObject.GetComponent<CharacterMotor>();

                if (motor.onRoof)
                {
                    motor.Shadows.SetShadowHeightModifier(-height);
                    motor.onRoof = false;
                    motor.roofHeight = 0f;
                    motor.onGround = false;
                }

                motor.underPlatform = false;
                motor.OnExitUnderPlatform();
            }
        }
    }
}