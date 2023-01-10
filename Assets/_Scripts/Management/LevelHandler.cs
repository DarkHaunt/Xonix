using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Xonix.Audio;



namespace Xonix.LevelHandling
{
    /// <summary>
    /// Processes all level-based logic
    /// </summary>
    public class LevelHandler : MonoBehaviour
    {
        private const string LevelUpSoundPath = "Audio/Game/LevelPassSound";

        private const float TimerTickDurationInSeconds = 60f; // Timer counts minutes
        private const float GameEndTimeTicksCount = 90; // One and a half hour of max game time
        private const float TargetSeaFieldCorruptionPercent = 0.50f; // A percent of corrupted sea field, when level will be completed

        public static event Action OnLevelCompleted;
        public static event Action OnLevelLosen;


        private Timer _levelEndTimer;

        private int _levelNumber = 1;


        public int CurrentLevel => _levelNumber;
        public Timer LevelEndTimer => _levelEndTimer;
        public float TimeLeft => _levelEndTimer.TicksLeft;



        public async Task InitAsync()
        {
            var levelUpSoundLoadingTask = Addressables.LoadAssetAsync<AudioClip>(LevelUpSoundPath).Task;

            await levelUpSoundLoadingTask;

            OnLevelCompleted += () => AudioManager2D.PlaySound(levelUpSoundLoadingTask.Result);

            _levelEndTimer = new Timer(TimerTickDurationInSeconds, GameEndTimeTicksCount);


#pragma warning disable CS4014 // ��� ��� ���� ����� �� ���������, ���������� ������������� ������ ������������ �� ��� ���, ���� ����� �� ����� ��������
            _levelEndTimer.Start();
#pragma warning restore CS4014 // ��� ��� ���� ����� �� ���������, ���������� ������������� ������ ������������ �� ��� ���, ���� ����� �� ����� ��������
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

        private void CompleteLevel()
        {
            _levelNumber++;

            OnLevelCompleted?.Invoke();
        }
    }
}
