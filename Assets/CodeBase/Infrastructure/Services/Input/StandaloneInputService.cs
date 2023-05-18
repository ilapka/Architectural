using UnityEngine;

namespace Infrastructure.Services.Input
{
    public class StandaloneInputService : InputService
    {
        private const KeyCode AttackKey = KeyCode.L;

        public override Vector2 Axis
        {
            get
            {
                Vector2 axis = GetSimpleInputAxis();

                if (axis == Vector2.zero)
                    axis = GetUnityAxis();

                return axis;
            }
        }

        private static Vector2 GetUnityAxis() =>
            new Vector2(UnityEngine.Input.GetAxis(Horizontal), UnityEngine.Input.GetAxis(Vertical));

        public override bool IsAttackButtonUp()
        {
            bool isButtonUp = SimpleInput.GetButtonUp(Button) || UnityEngine.Input.GetKeyUp(AttackKey);

            return isButtonUp;
        }
    }
}