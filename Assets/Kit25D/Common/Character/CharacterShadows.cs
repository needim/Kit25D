using UnityEngine;

namespace Kit25D
{
    public class CharacterShadows
    {
        CharacterMotor motor;

        public CharacterShadows(CharacterMotor motor)
        {
            this.motor = motor;
        }

        public void SetShadowHeightModifier(float h)
        {
            Vector3 modifier = motor.ShadowTransform.position;
            modifier.y += h;
            motor.ShadowTransform.position = modifier;
        }

        public void HandleShadows()
        {
            if (motor.isFreezed())
                return;

            if (!motor.onGround)
            {
                Vector3 scale = new Vector3(motor.defaultShadowScaleX, motor.defaultShadowScaleX, 1);
                scale.x = Mathf.Lerp(motor.ShadowTransform.localScale.x, Mathf.Clamp(-motor.velocity.z, -.3f, motor.defaultShadowScaleX), Time.deltaTime * .5f);
                motor.ShadowTransform.localScale = scale;
            }
            else
            {
                if (motor.ShadowTransform.localScale.x != motor.defaultShadowScaleX)
                {
                    Vector3 scale = new Vector3(motor.defaultShadowScaleX, motor.defaultShadowScaleX, 1);
                    scale.x = Mathf.Lerp(motor.ShadowTransform.localScale.x, motor.defaultShadowScaleX, Time.deltaTime * 15f);
                    motor.ShadowTransform.localScale = scale;
                }
            }

            if (motor.onRoof && !motor.onGround)
            {
                Vector3 scale = new Vector3(motor.defaultShadowScaleX, motor.defaultShadowScaleX, 1);
                scale.x = Mathf.Lerp(motor.shadowFix.transform.localScale.x, Mathf.Clamp(-motor.velocity.z, -.3f, motor.defaultShadowScaleX), Time.deltaTime * .5f);
                motor.shadowFix.transform.localScale = scale;
            }
            else if (motor.onRoof && motor.onGround)
            {
                if (motor.shadowFix.transform.localScale.x != motor.defaultShadowScaleX)
                {
                    Vector3 scale = new Vector3(motor.defaultShadowScaleX, motor.defaultShadowScaleX, 1);
                    scale.x = Mathf.Lerp(motor.shadowFix.transform.localScale.x, motor.defaultShadowScaleX, Time.deltaTime * 15f);
                    motor.shadowFix.transform.localScale = scale;
                }
            }
        }
    }
}