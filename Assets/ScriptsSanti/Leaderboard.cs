using System.Collections.Generic;
using Dan.Main;
using TMPro;
using UnityEngine;

namespace ScriptsSanti
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] string nameToAdd;
        [SerializeField] int scoreToAdd;

        [SerializeField] private List<TextMeshProUGUI> names;
        [SerializeField] private List<TextMeshProUGUI> scores;

        private readonly string _publicLeaderboardKey = "9aa45831f2d7b47f100c04b1147555116ce517cce44f50c1f56cd0a6839ac47a";

        private void GetLeaderboard()
        {
            LeaderboardCreator.GetLeaderboard(_publicLeaderboardKey, (msg) =>
            {
                int loopLength = (msg.Length < names.Count ? msg.Length : names.Count);
                for (int i = 0; i < loopLength; i++)
                {
                    names[i].text = msg[i].Username;
                    scores[i].text = msg[i].Score.ToString();
                }
            });
        }

        public void SetLeaderboardEntry(string username, int score)
        {
            username = username.ToUpper();
            LeaderboardCreator.UploadNewEntry(_publicLeaderboardKey, username, score, (_) => { GetLeaderboard(); });
            LeaderboardCreator.ResetPlayer();
        }

        public void SubmitScore()
        {
            SetLeaderboardEntry(nameToAdd, scoreToAdd);
        }

        void Start()
        {
            GetLeaderboard();
        }

        public int GetNumber10Score()
        {
            int.TryParse(scores[scores.Count - 1].text, out var numberTenScore);
            return numberTenScore;
        }
    }
}