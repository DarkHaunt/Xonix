using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Xonix.UI;



namespace Xonix.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance;

        // To avoid animation blicking for small scenes
        private const int MinimalLoadingScreenAnimationPlayingTimeMiliseconds = 1000;
        // List of general used scenes
        private const string MainMenuScenePath = "Scenes/MainMenu";


        [SerializeField] private ScreenEffectHandler _transitionHandler;

        private CancellationTokenSource _sceneLoadingCancellationSource;



        /// <summary>
        /// Load main menu scene, unload other scenes
        /// </summary>
        public static async void LoadMainMenu() => await LoadSceneWithTransition(MainMenuScenePath, LoadSceneMode.Single);

        /// <summary>
        /// Uses setted animation hide loading process from user
        /// </summary>
        /// <param name="sceneKey">Addressables loading key-string</param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        public static async Task LoadSceneWithTransition(string sceneKey, LoadSceneMode loadMode)
        {
            await _instance._transitionHandler.PlayFadeOutAnimation();

            var loadSceneProcess = Addressables.LoadSceneAsync(sceneKey, loadMode, true);

            var loadingSceneTask = Task.Run(async () => await loadSceneProcess.Task, _instance._sceneLoadingCancellationSource.Token);

            var longTimeSceneLoadingImitationTask = Task.Delay(MinimalLoadingScreenAnimationPlayingTimeMiliseconds, _instance._sceneLoadingCancellationSource.Token);
            var animationTask = _instance._transitionHandler.PlayLoadingAnimation(longTimeSceneLoadingImitationTask);

            var loadingAnimationTask = Task.WhenAny(longTimeSceneLoadingImitationTask, animationTask);

            await Task.WhenAll(loadingAnimationTask, loadingSceneTask);

            await _instance._transitionHandler.PlayFadeInAnimation();
        }

        /// <summary>
        /// Loads scene without animation
        /// </summary>
        /// <param name="sceneKey">Addressables loading key-string</param>
        /// <param name="loadMode"></param>
        public static async void LoadSceneImmediately(string sceneKey, LoadSceneMode loadMode)
        {
            await Addressables.LoadSceneAsync(sceneKey, loadMode, true).Task;
        }

        private void Init()
        {
            DontDestroyOnLoad(gameObject);

            _sceneLoadingCancellationSource = new CancellationTokenSource();

            Application.quitting += _sceneLoadingCancellationSource.Cancel;

            LoadSceneImmediately(MainMenuScenePath, LoadSceneMode.Single);
        }



        private void Awake()
        {
            #region [Singleton]

            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            #endregion

            Init();
        }

    }
}
