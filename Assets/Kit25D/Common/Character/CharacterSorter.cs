using UnityEngine;

namespace Kit25D
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class CharacterSorter : MonoBehaviour
    {
        CharacterMotor motor;
        SpriteRenderer shadowRenderer;

        public float depthOverride = 0f;

        void Start()
        {
            motor = GetComponent<CharacterMotor>();
            shadowRenderer = motor.ShadowTransform.GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, depthOverride == 0f ? transform.position.y : depthOverride);

            if (motor.onRoof)
            {
                shadowRenderer.sortingOrder = 0;
                shadowRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                if (!motor.onPlatform)
                    motor.shadowFix.SetActive(true);
            }
            else
            {
                shadowRenderer.sortingOrder = -1;
                shadowRenderer.maskInteraction = SpriteMaskInteraction.None;
                motor.shadowFix.SetActive(false);
            }

            transform.position = pos;
        }
    }
}