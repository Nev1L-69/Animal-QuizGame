using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI level1HighScoreText;
    public TextMeshProUGUI level2HighScoreText;
    public TextMeshProUGUI level3HighScoreText;

    void Start()
    {
        int highScore1 = PlayerPrefs.GetInt("Level1_HighScore", 0);
        int highScore2 = PlayerPrefs.GetInt("Level2_HighScore", 0);
        int highScore3 = PlayerPrefs.GetInt("Level3_HighScore", 0);
        level1HighScoreText.text = "Best Score (Level 1): " + highScore1;
        level2HighScoreText.text = "Best Score (Level 2): " + highScore2;
        level3HighScoreText.text = "Best Score (Level 3): " + highScore3;
    }

    // Кнопки переходов
    public void StartLevel1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1_MultipleChoice");
    }

    public void StartLevel2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level2_TrueFalse");
    }

    public void StartLevel3()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level3_WordPuzzle");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
