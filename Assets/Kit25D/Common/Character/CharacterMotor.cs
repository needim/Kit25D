using System.Collections;
using System.Collections.Generic;
using Kit25D.CustomAttributes;
using UnityEngine;

namespace Kit25D
{
    [AddComponentMenu("Kit25D/CharacterMotor")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(CharacterSorter))]
    public class CharacterMotor : CachedBehaviour
    {
        public enum Brains { Player, AI }
        public enum FaceDirections { Left = -1, Right = 1 }
        public enum Statuses { Normal, Freezed, Kinematic, Dead }
        public enum Teams { Friendly, Enemy }

        public Brains brainType = Brains.AI;
        public FaceDirections faceDirection = FaceDirections.Right;
        public Teams team = Teams.Enemy;

        private List<Statuses> statuses = new List<Statuses>();

        [Header("GFX Refereneces")]
        [Info("This must be your characters graphical visuals with animations, sounds etc. While jumping this transform will go up!")]
        public Transform GFXTransform;

        [Info("This transform determines ground position and used by landing check.")]
        public Transform ShadowTransform;

        [Info("This needs to be duplicate of Shadow Transform but disabled. CharacterMotor will handle rest of it.")]
        public GameObject shadowFix;

        public float defaultShadowScaleX { get; protected set; }

        [Header("Movement")]
        public LayerMask obstacleLayerMask;
        public bool onGround = true;
        public bool onRoof = false;
        public bool onPlatform = false;
        [HideInInspector] public float roofHeight = 0f;
        public float speed = 5f;
        public float speedModifierY = .65f;
        public float currentSpeed = 5f;
        public float maxJumpHeight = 3f;
        public float minJumpHeight = 1f;
        public float timeToJumpApex = .4f;
        public float skinWidth = .015f;
        public float zHeight = 0f;
        public Vector2 directionalInput;
        public Vector3 velocity;
        public Bounds bounds { get; protected set; }
        public float maxJumpVelocity { get; protected set; }
        public float minJumpVelocity { get; protected set; }
        public float gravity { get; protected set; }

        [Header("Slope Movement")]
        [Info("While true we are trying to keep player on track.")]
        public bool slopeControl = true;
        public string slopeTagName = "LevelBounds";
        public float slopeRayLength = 10f;
        public float slopeAngle = 0f;

        [Header("Platforming")]
        [Info("It's used by movement controller, if player jump while under a platform.")]
        public Transform headPoint;
        public bool underPlatform = false;

        public Animator _animator { get; protected set; }
        public CharacterMovements Movement { get; protected set; }
        public CharacterShadows Shadows { get; protected set; }
        public CharacterInputs Inputs { get; protected set; }

        private SpriteRenderer[] GFXSpriteRenderers;

        private void Start()
        {
            Movement = new CharacterMovements(this);
            Shadows = new CharacterShadows(this);
            Inputs = new CharacterInputs(this);

            _animator = GFXTransform.GetComponentInChildren<Animator>();
            currentSpeed = speed;
            defaultShadowScaleX = ShadowTransform.transform.localScale.x;
            _collider2D.bounds.Expand(skinWidth);
            GFXSpriteRenderers = GFXTransform.GetComponentsInChildren<SpriteRenderer>();

            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        }

        void Update()
        {
            if (!isAI())
                Inputs.HandleInputs();

            Shadows.HandleShadows();
            HandleAnimations();
        }

        void FixedUpdate()
        {
            Movement.CalculateVelocity();
            Movement.ApplyMovement(velocity * Time.fixedDeltaTime, directionalInput);

            zHeight = GFXTransform.position.y - ShadowTransform.position.y;

            if (!onGround && zHeight <= 0f)
            {
                onGround = true;
                velocity.z = 0f;
                GFXTransform.localPosition = new Vector3(ShadowTransform.localPosition.x, ShadowTransform.localPosition.y, 0);
            }
        }

        void HandleAnimations()
        {
            if (isFreezed())
                return;

            _animator.SetBool("grounded", onGround);
            _animator.SetFloat("velocityX", Mathf.Abs(directionalInput.x) + Mathf.Abs(directionalInput.y));
            _animator.SetFloat("velocityY", velocity.z);
        }

        public void OnUnderPlatform()
        {
            if (GFXSpriteRenderers.Length > 0)
            {
                for (int i = 0; i < GFXSpriteRenderers.Length; i++)
                {
                    GFXSpriteRenderers[i].color = new Color32(180, 180, 180, 255);
                }
            }
        }

        public void OnExitUnderPlatform()
        {
            if (GFXSpriteRenderers.Length > 0)
            {
                for (int i = 0; i < GFXSpriteRenderers.Length; i++)
                {
                    GFXSpriteRenderers[i].color = Color.white;
                }
            }
        }

        public void SetFaceDirection(int dir)
        {
            _transform.localScale = new Vector3(dir, 1, 1);
        }

        public bool isKinematic()
        {
            return statuses.Contains(Statuses.Kinematic);
        }

        public bool isFreezed()
        {
            return statuses.Contains(Statuses.Freezed);
        }

        public void Freeze()
        {
            if (!isFreezed())
                statuses.Add(Statuses.Freezed);
        }

        public void UnFreeze()
        {
            if (isFreezed())
                statuses.Remove(Statuses.Freezed);
        }

        public bool isDead()
        {
            return statuses.Contains(Statuses.Dead);
        }

        public bool isAI()
        {
            return brainType == Brains.AI;
        }

        /// <summary> This validates Character component and warns you if something is wrong. (Called in the editor only). </summary>
        void OnValidate()
        {
            if (!_rb.isKinematic)
                Debug.LogWarning("Character's Rigidbody2D must be kinematic in order to work with Kit25D");
        }

        /// <summary> This tries to assing default values </summary>
        void Reset()
        {
            _rb.isKinematic = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}