using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Dan.Main;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "b4b5364321b1d6af6c914a51c2d352f3bb04ca52ef1c2193625ecc6a0f01cb79";

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, (msg) =>
        {
            for (int i = 0; i < names.Count; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        });
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, (_) =>
        {
            GetLeaderboard();
        });
    }
}
