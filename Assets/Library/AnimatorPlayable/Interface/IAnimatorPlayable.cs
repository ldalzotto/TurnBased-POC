using System;

namespace _AnimatorPlayable._Interface
{
    public interface IAnimatorPlayable
    {
        void PlayAnimation(int layerID, IAnimationInput animationInput, Action OnAnimationEnd = null, Func<float> InputWeightProvider = null);
        void Stop();
        void Play();
        void DestroyLayer(int layerID);
        void StopLayer(int animationLayer);
    }
}
