using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameController3 : MonoBehaviour
{
    public TextMeshProUGUI scrambledText;
    public TMP_InputField answerInput;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public GameObject winPanel;

    public AudioSource audioSource; // новый AudioSource для звуков
    public AudioClip correctSound;  // звук правильного ответа
    public AudioClip wrongSound;    // звук неправильного ответа

    private List<WordData> words;
    private int currentWordIndex = 0;
    private int score = 0;
    private int requiredScore = 7;
    private string currentWord;
    private bool isAnswering = false;

    void Start()
    {
        LoadWords();
        DisplayWord();
    }

    void LoadWords()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("level3_questions");
        words = new List<WordData>(JsonUtility.FromJson<WordList>("{\"words\":" + jsonFile.text + "}").words);
    }

    void DisplayWord()
    {
        if (currentWordIndex >= words.Count)
        {
            EndLevel();
            return;
        }

        isAnswering = false;
        feedbackText.text = "";
        currentWord = words[currentWordIndex].word;
        scrambledText.text = ShuffleWord(currentWord);
        answerInput.text = "";
        scoreText.text = "Score: " + score;
    }

    public void SubmitAnswer()
    {
        if (isAnswering) return;
        isAnswering = true;

        string playerAnswer = answerInput.text.ToUpper().Trim();

        if (playerAnswer == currentWord)
        {
            score++;
            PlaySound(correctSound);
        }
        else
        {
            PlaySound(wrongSound);
        }

        feedbackText.text = currentWord;

        StartCoroutine(NextWordAfterDelay(1.5f));
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator NextWordAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentWordIndex++;
        DisplayWord();
    }

    string ShuffleWord(string word)
    {
        return new string(word.ToCharArray().OrderBy(c => Random.value).ToArray());
    }

    void EndLevel()
    {
        winPanel.SetActive(true);
        string result = score >= requiredScore ? "Level Passed!" : "Try Again.";
        winPanel.GetComponentInChildren<TextMeshProUGUI>().text = result + "\nScore: " + score;

        int previousHighScore = PlayerPrefs.GetInt("Level3_HighScore", 0);
        if (score > previousHighScore)
        {
            PlayerPrefs.SetInt("Level3_HighScore", score);
            PlayerPrefs.Save();
        }

        StartCoroutine(ReturnToMainMenuAfterDelay(3f));
    }

    IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }
}
