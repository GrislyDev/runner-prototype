using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{
    public TMP_Text _rankText;
    public TMP_Text _usernameText;
    public TMP_Text _scoreText;

    public void NewScoreElement (int rank, string username, int score)
    {
        _rankText.text = rank.ToString();
        _usernameText.text = username;
        _scoreText.text = score.ToString();
    }
}