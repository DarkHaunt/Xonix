using UnityEngine;
using Xonix.LevelHandling;



namespace Xonix.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Canvas))]
    public class ScreenFlicker : MonoBehaviour
    {
        private const string FlickTriggerName = "Flick";


        private Canvas _flickCanvas;
        private Animator _animator;



        public void FlickScreen()
        {
            _flickCanvas.gameObject.SetActive(true);

            _animator.SetTrigger(FlickTriggerName);
        }

        // Used in animator event
        private void OffCanvas() => _flickCanvas.gameObject.SetActive(false);



        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _flickCanvas = GetComponent<Canvas>();

            LevelHandler.OnLevelLosen += FlickScreen;

            _flickCanvas.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            LevelHandler.OnLevelLosen -= FlickScreen;
        }
    }
}
