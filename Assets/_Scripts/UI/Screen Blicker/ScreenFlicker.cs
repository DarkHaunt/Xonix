using UnityEngine;



namespace Xonix.UI
{
    [RequireComponent(typeof(Animator))]
    public class ScreenFlicker : MonoBehaviour
    {
        private const string FlickTriggerName = "Flick";


        private Animator _animator;



        private void Awake()
        {
            _animator = GetComponent<Animator>();

            XonixGame.OnLevelLosen += () => _animator.SetTrigger(FlickTriggerName);
        }
    }
}
