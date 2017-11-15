using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit25D
{
    public class CharacterMovements
    {
        private CharacterMotor motor;

        public CharacterMovements(CharacterMotor motor)
        {
            this.motor = motor;
        }

        public void CalculateVelocity()
        {
            if (motor.isFreezed())
                return;

            motor.velocity.x = motor.directionalInput.x * motor.currentSpeed;
            motor.velocity.y = motor.directionalInput.y * (motor.currentSpeed * motor.speedModifierY);

            if (!motor.onGround)
            {
                motor.velocity.z += motor.gravity * Time.fixedDeltaTime;

                if (motor.velocity.z < -20f)
                    motor.velocity.z = -20f;
            }
        }

        public void ApplyMovement(Vector3 moveAmount, Vector2 input)
        {
            if (motor.isFreezed())
                return;

            motor.faceDirection = moveAmount.x > 0 ? CharacterMotor.FaceDirections.Right : moveAmount.x < 0 ? CharacterMotor.FaceDirections.Left : motor.faceDirection;
            motor._transform.localScale = new Vector3((int) motor.faceDirection, 1, 1);

            if (motor.slopeControl && moveAmount.x != 0)
                SlopeControl(ref moveAmount);

            if (moveAmount.y == 0)
            {
                VerticalCollisions(ref moveAmount, 1);
                VerticalCollisions(ref moveAmount, -1);
            }

            if (moveAmount.y > 0)
                VerticalCollisions(ref moveAmount, 1);

            if (moveAmount.y < 0)
                VerticalCollisions(ref moveAmount, -1);

            HorizontalCollisions(ref moveAmount);

            if (!motor.onGround && motor.underPlatform && motor.velocity.z > 0f)
                JumpBlockControl(ref moveAmount);

            motor._transform.Translate(new Vector2(moveAmount.x, moveAmount.y));

            if (!motor.onGround)
                motor.GFXTransform.Translate(new Vector2(0, moveAmount.z));
        }

        void VerticalCollisions(ref Vector3 moveAmount, int directionY)
        {
            float rayLength = Mathf.Abs(moveAmount.y) + motor.skinWidth;

            List<Vector2> rays = new List<Vector2>();

            if (directionY == 1)
            {
                rays.Add(new Vector2(motor._collider2D.bounds.max.x, motor._collider2D.bounds.max.y));
                rays.Add(new Vector2((motor._collider2D.bounds.max.x + motor._collider2D.bounds.min.x) * .5f, motor._collider2D.bounds.max.y));
                rays.Add(new Vector2(motor._collider2D.bounds.min.x, motor._collider2D.bounds.max.y));
            }
            else
            {
                rays.Add(new Vector2(motor._collider2D.bounds.max.x, motor._collider2D.bounds.min.y));
                rays.Add(new Vector2((motor._collider2D.bounds.max.x + motor._collider2D.bounds.min.x) * .5f, motor._collider2D.bounds.min.y));
                rays.Add(new Vector2(motor._collider2D.bounds.min.x, motor._collider2D.bounds.min.y));
            }

            for (int i = 0; i < rays.Count; i++)
            {
                RaycastHit2D hit;

                if (directionY == 1)
                {
                    hit = Physics2D.Raycast(rays[i], Vector2.up, rayLength, motor.obstacleLayerMask);
                    Debug.DrawRay(rays[i], Vector2.up * directionY * rayLength, Color.yellow);
                }
                else
                {
                    hit = Physics2D.Raycast(rays[i], Vector2.down, rayLength, motor.obstacleLayerMask);
                    Debug.DrawRay(rays[i], Vector2.up * directionY * rayLength, Color.red);
                }

                if (hit)
                {
                    if (motor.isKinematic())
                        continue;

                    SceneryCollider sceneryCollider = hit.collider.transform.GetComponent<SceneryCollider>();

                    if (sceneryCollider != null)
                    {
                        if (sceneryCollider.hasRoof && !motor.onGround && motor.zHeight > sceneryCollider.height)
                            continue;

                        if (sceneryCollider.hasRoof && motor.onRoof)
                            continue;

                        if (sceneryCollider.isPlatform)
                            continue;
                    }

                    if (hit.collider.CompareTag("JumpBlocker") && motor.onGround)
                        continue;

                    moveAmount.y = (hit.distance - motor.skinWidth) * directionY;
                }
            }
        }

        void HorizontalCollisions(ref Vector3 moveAmount)
        {
            float rayLength = Mathf.Abs(moveAmount.x) + motor.skinWidth;
            float directionX = Mathf.Sign(moveAmount.x);

            List<Vector2> rays = new List<Vector2>();

            if (directionX == 1)
            {
                rays.Add(new Vector2(motor._collider2D.bounds.max.x, motor._collider2D.bounds.max.y));
                rays.Add(new Vector2(motor._collider2D.bounds.max.x, motor._collider2D.bounds.min.y));
            }
            else
            {
                rays.Add(new Vector2(motor._collider2D.bounds.min.x, motor._collider2D.bounds.max.y));
                rays.Add(new Vector2(motor._collider2D.bounds.min.x, motor._collider2D.bounds.min.y));
            }

            for (int i = 0; i < rays.Count; i++)
            {
                RaycastHit2D hit;

                if (directionX == 1)
                {
                    hit = Physics2D.Raycast(rays[i], Vector2.right * directionX, rayLength, motor.obstacleLayerMask);
                    Debug.DrawRay(rays[i], Vector2.right * directionX * rayLength, Color.white);
                }
                else
                {
                    hit = Physics2D.Raycast(rays[i], Vector2.right * directionX, rayLength, motor.obstacleLayerMask);
                    Debug.DrawRay(rays[i], Vector2.right * directionX * rayLength, Color.white);
                }

                if (hit)
                {
                    if (motor.isKinematic())
                        continue;

                    SceneryCollider sceneryCollider = hit.collider.transform.GetComponent<SceneryCollider>();

                    if (sceneryCollider != null)
                    {
                        if (sceneryCollider.hasRoof && !motor.onGround && motor.zHeight > sceneryCollider.height)
                            continue;

                        if (sceneryCollider.hasRoof && motor.onRoof)
                            continue;

                        if (sceneryCollider.isPlatform)
                            continue;
                    }

                    if (hit.collider.CompareTag("JumpBlocker") && motor.onGround)
                        continue;

                    moveAmount.x = (hit.distance - motor.skinWidth) * directionX;
                }
            }
        }

        void SlopeControl(ref Vector3 moveAmount)
        {
            float rayLength = motor.slopeRayLength + motor.skinWidth;

            Vector2 front = new Vector2(motor._collider2D.bounds.max.x, motor._collider2D.bounds.min.y);
            Vector2 back = new Vector2(motor._collider2D.bounds.min.x, motor._collider2D.bounds.min.y);

            if ((int) motor.faceDirection == -1)
            {
                back = new Vector2(motor._collider2D.bounds.max.x, motor._collider2D.bounds.min.y);
                front = new Vector2(motor._collider2D.bounds.min.x, motor._collider2D.bounds.min.y);
            }

            RaycastHit2D hitFront = Physics2D.Raycast(front, Vector2.down, rayLength, motor.obstacleLayerMask);
            Debug.DrawRay(front, Vector2.down * rayLength, Color.magenta);

            RaycastHit2D hitBack = Physics2D.Raycast(back, Vector2.down, rayLength, motor.obstacleLayerMask);
            Debug.DrawRay(back, Vector2.down * rayLength, Color.black);

            if (hitFront && hitBack)
            {
                if (hitFront.collider.CompareTag(motor.slopeTagName) && hitBack.collider.CompareTag(motor.slopeTagName))
                {
                    float angle = Vector2.Angle(hitFront.normal, Vector2.up);
                    motor.slopeAngle = hitFront.distance < hitBack.distance ? angle : -angle;

                    if (Mathf.Abs(motor.slopeAngle) < 0.3f)
                        return; // discard very low slopes

                    float moveDistance = Mathf.Abs(moveAmount.x);
                    float climbmoveAmountY = Mathf.Sin(motor.slopeAngle * Mathf.Deg2Rad) * moveDistance;

                    moveAmount.y += climbmoveAmountY;
                }
            }
        }

        void JumpBlockControl(ref Vector3 moveAmount)
        {
            float rayLength = .15f;
            Vector2 rayOrigin = new Vector2(motor.headPoint.position.x, motor.headPoint.position.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, motor.obstacleLayerMask);
            Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.magenta);

            if (hit)
            {
                if (hit.collider.CompareTag("JumpBlocker"))
                {
                    moveAmount.z = -0.1f;
                    motor.velocity.z = -0.1f;
                }
            }
        }
    }
}