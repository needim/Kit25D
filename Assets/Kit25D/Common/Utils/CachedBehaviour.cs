using UnityEngine;
using System;

namespace Kit25D
{
    public class CachedBehaviour : MonoBehaviour
    {
        [HideInInspector, NonSerialized]
        private Collider2D __collider2D;

        /// <summary>
        /// Gets the Collider2D attached to the object.
        /// </summary>
        public Collider2D _collider2D { get { return __collider2D ? __collider2D : (__collider2D = GetComponent<Collider2D>()); } }

        [HideInInspector, NonSerialized]
        private Rigidbody2D __rigidbody2D;

        /// <summary>
        /// Gets the Rigidbody2D attached to the object.
        /// </summary>
        public Rigidbody2D _rb { get { return __rigidbody2D ? __rigidbody2D : (__rigidbody2D = GetComponent<Rigidbody2D>()); } }

        [HideInInspector, NonSerialized]
        private Transform __transform;

        /// <summary>
        /// Gets the Transform attached to the object.
        /// </summary>
        public Transform _transform { get { return __transform ? __transform : (__transform = GetComponent<Transform>()); } }
    }
}