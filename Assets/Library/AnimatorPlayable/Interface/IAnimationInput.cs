namespace _AnimatorPlayable._Interface
{
    public enum AnimationInputType
    {
        BLENDED,
        SEQUENCED,
        TWODBLENDED
    }

    public interface IAnimationInput
    {
        AnimationInputType AnimationInputType { get; }
    }
}