using UnityEngine;

namespace Kit25D
{
    [ExecuteInEditMode]
    public class SpriteSorter : MonoBehaviour
    {
        public float groundOffset = 0f;
        public float height = 0f;

        float m_spriteLowerBound;
        float m_spriteHalfWidth;

#if UNITY_EDITOR
        SpriteRenderer sr;
        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + groundOffset);
            }
        }

        void OnDrawGizmos()
        {
            Vector3 floorHeightPos = new Vector3(sr.bounds.min.x, sr.bounds.min.y + height, transform.position.z);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(floorHeightPos, floorHeightPos + new Vector3(sr.bounds.size.x, 0, 0));

            if (groundOffset != 0f)
            {
                Vector3 groundPos = new Vector3(sr.bounds.min.x, sr.bounds.min.y + groundOffset, transform.position.z);
                Gizmos.color = Color.black;
                Gizmos.DrawLine(groundPos, groundPos + new Vector3(sr.bounds.size.x, 0, 0));
            }
        }
#endif
    }
}