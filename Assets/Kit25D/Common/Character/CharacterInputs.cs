using UnityEngine;

namespace Kit25D
{
    public class CharacterInputs
    {
        CharacterMotor motor;

        public CharacterInputs(CharacterMotor motor)
        {
            this.motor = motor;
        }

        public void HandleInputs()
        {
            if (motor.isKinematic())
                return;

            motor.directionalInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (Input.GetButtonDown("Jump"))
                OnJumpInputDown();

            if (Input.GetButtonUp("Jump"))
                OnJumpInputUp();

            if (Input.GetButtonDown("Fire1"))
            {
                // OnAttackInputDown();
            }
        }

        public void OnJumpInputDown()
        {
            if (motor.isFreezed())
                return;

            if (motor.onGround)
            {
                motor.velocity.z = motor.maxJumpVelocity;
                motor.onGround = false;
            }
        }

        void OnJumpInputUp()
        {
            if (motor.isFreezed())
                return;

            if (motor.velocity.z > motor.minJumpVelocity)
                motor.velocity.z = motor.minJumpVelocity;
        }
    }
}