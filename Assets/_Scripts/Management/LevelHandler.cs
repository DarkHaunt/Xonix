using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Xonix.Audio;



namespace Xonix.LevelHandling
{
    public class LevelHandler : MonoBehaviour
    {
        private const string LevelUpSoundPath = "Audio/Game/LevelPassSound";

        private const float TimerTickDurationInSeconds = 60f; // Timer counts minutes
        private const float GameEndTimeTicksCount = 90; // One and a half hour of max game time
        private const float TargetSeaFieldCorruptionPercent = 0.5f; // A percent of corrupted sea field, when level will be completed

        public static event Action OnLevelCompleted;
        public static event Action OnLevelLosen;


        private Timer _levelEndTimer;
        private float _timeTicksLeft = GameEndTimeTicksCount;

        private int _levelNumber = 1;


        public int CurrentLevel => _levelNumber;
        public Timer LevelEndTimer => _levelEndTimer;
        public float TimeLeft => _timeTicksLeft;



        public async Task InitAsync()
        {
            var levelUpSoundLoadingTask = Addressables.LoadAssetAsync<AudioClip>(LevelUpSoundPath).Task;

            await levelUpSoundLoadingTask;

            OnLevelCompleted += () => SoundManager.PlayClip(levelUpSoundLoadingTask.Result);

            _levelEndTimer = new Timer(TimerTickDurationInSeconds, GameEndTimeTicksCount);
            _levelEndTimer.OnTickPassed += () => _timeTicksLeft--;


#pragma warning disable CS4014 // “ак как этот вызов не ожидаетс€, выполнение существующего метода продолжаетс€ до тех пор, пока вызов не будет завершен
            _levelEndTimer.Start();
#pragma warning restore CS4014 // “ак как этот вызов не ожидаетс€, выполнение существующего метода продолжаетс€ до тех пор, пока вызов не будет завершен
        }

        private void CompleteLevel()
        {
            _levelNumber++;

            OnLevelCompleted?.Invoke();
        }

        public void LoseLevel()
        {
            OnLevelLosen?.Invoke();
        }

        public void CheckForLevelComplete(float currentSeaCorruptionPercent)
        {
            if (currentSeaCorruptionPercent >= TargetSeaFieldCorruptionPercent)
                CompleteLevel();
        }
    }
}
