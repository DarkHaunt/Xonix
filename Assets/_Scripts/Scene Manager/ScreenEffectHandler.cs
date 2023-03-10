using System.Threading.Tasks;
using System.Threading;
using UnityEngine;



namespace Xonix.UI
{
    /// <summary>
    /// Executes screen transitions
    /// </summary>
    public class ScreenEffectHandler : MonoBehaviour
    {
        private const string FadeOutClipID = "---Fade Out Clip---";
        private const string FadeInClipID = "---Fade In Clip---";
        private const string LoadingScreenClipID = "---Loading Screen Clip---";


        [Header("--- Animations ---")]
        [SerializeField] private AnimationClip _screenFadeOutAnimation;
        [SerializeField] private AnimationClip _screenFadeInAnimation;
        [SerializeField] private AnimationClip _loadingScreenAnimation;

        private Animation _animation;
        private CancellationTokenSource _transitionCancellationSource;



        private bool TaskWasCancelled
        {
            get
            {
                var wasCancelled = _transitionCancellationSource.IsCancellationRequested;

                if (wasCancelled)
                {
                    _transitionCancellationSource.Dispose();
                    _transitionCancellationSource = new CancellationTokenSource();
                }

                return wasCancelled;
            }
        }



        public async Task PlayFadeInAnimation()
        {
            await PlayAnimationClip(FadeInClipID);
        }

        public async Task PlayFadeOutAnimation()
        {
            await PlayAnimationClip(FadeOutClipID);
        }

        public async Task PlayLoadingAnimation(Task sceneLoadingTask)
        {
            var loadingEndTask = sceneLoadingTask.ContinueWith(_ => _transitionCancellationSource.Cancel());

            var animation = PlayAnimationClip(LoadingScreenClipID);

            await Task.WhenAny(animation, loadingEndTask);
        }

        private async Task PlayAnimationClip(string clipID)
        {
            if (TaskWasCancelled)
                await Task.FromCanceled(_transitionCancellationSource.Token);

            if (_animation.isPlaying)
            {
#if UNITY_EDITOR

                print($"Animation component in {gameObject.name} is playing clip, so you can't play another until clip will end"); 
#endif
                await Task.FromCanceled(CancellationToken.None);
            }

            gameObject.SetActive(true);

            _animation.Play(clipID);

            await WaitUntilPlayingEnds();

            if (Application.isPlaying)
                gameObject.SetActive(false);
        }

        private async Task WaitUntilPlayingEnds()
        {
            while (!TaskWasCancelled && _animation.isPlaying)
                await Task.Yield();
        }

        private void Init()
        {
            DontDestroyOnLoad(gameObject);

            _animation = gameObject.AddComponent<Animation>();
            _animation.playAutomatically = false;

            InitClip(_screenFadeOutAnimation, FadeOutClipID);
            InitClip(_screenFadeInAnimation, FadeInClipID);
            InitClip(_loadingScreenAnimation, LoadingScreenClipID);

            _loadingScreenAnimation.wrapMode = WrapMode.Loop;

            _transitionCancellationSource = new CancellationTokenSource();

            Application.quitting += () => _transitionCancellationSource.Cancel();

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets clip to work with Animator component
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="clipID"></param>
        private void InitClip(AnimationClip clip, string clipID)
        {
            clip.legacy = true;
            _animation.AddClip(clip, clipID);
        }



        private void Awake()
        {
            Init();
        }
    }
}
