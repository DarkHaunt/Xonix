using UnityEngine;



namespace Xonix
{
	public class ScoreCounter
	{
		private const string ScorePrefsKey = "Score";


		private int _score = 0;



		public int Score => _score;



		public ScoreCounter() { }



		public static int GetRecordScore()
		{
			if (!PlayerPrefs.HasKey(ScorePrefsKey))
				PlayerPrefs.SetInt(ScorePrefsKey, 0);

			return PlayerPrefs.GetInt(ScorePrefsKey);
		}

		public void TryToUpdateRecord()
		{
			var currenRecord = GetRecordScore();

			if (currenRecord <= _score)
				PlayerPrefs.SetInt(ScorePrefsKey, _score);
		}

		public void AddScore(int score)
		{
			_score += score;
		}
	} 
}
