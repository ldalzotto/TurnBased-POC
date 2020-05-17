using _AnimatorPlayable;
using _AnimatorPlayable._Interface;

namespace UnityEngine.Rendering
{
    public class TreeAnimationTest : MonoBehaviour
    {
        private AnimatorPlayable AnimatorPlayableObject;
        public Vector2 Input;
        public IAnimationPlayableDefinition TwoDBlendTreePlayableDefinition;

        private void Start()
        {
            this.AnimatorPlayableObject = AnimatorPlayable.alloc("Test", this.GetComponent<Animator>());
            this.AnimatorPlayableObject.PlayAnimation(0, TwoDBlendTreePlayableDefinition.GetAnimationInput(), null, null);
        }

        private void Update()
        {
            this.AnimatorPlayableObject.SetTwoDInputWeight(0, this.Input);
            this.AnimatorPlayableObject.Tick(Time.deltaTime);
        }

    }
}