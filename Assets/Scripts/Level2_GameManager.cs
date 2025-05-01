using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController2 : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public Button trueButton;
    public Button falseButton;
    public TextMeshProUGUI scoreText;
    public GameObject winPanel;

    public Image questionImage;
    public AudioSource audioSource;
    public VideoPlayer videoPlayer;
    public RawImage videoScreen;

    public AudioClip correctSound; // 🎵 звук правильного ответа
    public AudioClip wrongSound;   // 🎵 звук неправильного ответа

    private List<QuestionData2> questions;
    private int currentQuestionIndex = 0;
    private int score = 0;
    private int requiredScore = 3;
    private bool isAnswering = false;

    void Start()
    {
        LoadQuestions();
        DisplayQuestion();
    }

    void LoadQuestions()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("level2_questions");
        questions = new List<QuestionData2>(JsonUtility.FromJson<QuestionList2>("{\"questions\":" + jsonFile.text + "}").questions);
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndLevel();
            return;
        }

        isAnswering = false;

        QuestionData2 q = questions[currentQuestionIndex];
        questionText.text = q.question;

        trueButton.interactable = true;
        falseButton.interactable = true;
        ResetButtonColors();

        // Сброс медиа
        questionImage.enabled = false;
        videoScreen.enabled = false;
        videoPlayer.Stop();
        audioSource.Stop();

        // Показать картинку
        if (!string.IsNullOrEmpty(q.image))
        {
            questionImage.enabled = true;
            Sprite img = Resources.Load<Sprite>(q.image);
            questionImage.sprite = img;
        }

        // Проиграть аудио
        if (!string.IsNullOrEmpty(q.audio))
        {
            AudioClip clip = Resources.Load<AudioClip>(q.audio);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        // Проиграть видео
        if (!string.IsNullOrEmpty(q.video))
        {
            VideoClip vid = Resources.Load<VideoClip>(q.video);
            if (vid != null)
            {
                videoScreen.enabled = true;
                videoPlayer.clip = vid;
                videoPlayer.Play();
            }
        }

        scoreText.text = "Score: " + score;
    }

    public void OnTrueSelected()
    {
        OnAnswerSelected(true, trueButton);
    }

    public void OnFalseSelected()
    {
        OnAnswerSelected(false, falseButton);
    }

    void OnAnswerSelected(bool selectedAnswer, Button selectedButton)
    {
        if (isAnswering) return;
        isAnswering = true;

        QuestionData2 q = questions[currentQuestionIndex];

        trueButton.interactable = false;
        falseButton.interactable = false;

        if (selectedAnswer == q.correctAnswer)
        {
            score++;
            selectedButton.GetComponent<Image>().color = Color.green;
            PlaySound(correctSound); // ▶ правильный звук
        }
        else
        {
            selectedButton.GetComponent<Image>().color = Color.red;
            PlaySound(wrongSound);   // ▶ неправильный звук

            // Подсветить правильную кнопку зелёным
            if (q.correctAnswer)
                trueButton.GetComponent<Image>().color = Color.green;
            else
                falseButton.GetComponent<Image>().color = Color.green;
        }

        StartCoroutine(NextQuestionAfterDelay(1.5f));
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentQuestionIndex++;
        DisplayQuestion();
    }

    void ResetButtonColors()
    {
        trueButton.GetComponent<Image>().color = Color.yellow;
        falseButton.GetComponent<Image>().color = Color.cyan;
    }

    void EndLevel()
    {
        winPanel.SetActive(true);
        string result = score >= requiredScore ? "Level Passed!" : "Try Again.";
        winPanel.GetComponentInChildren<TextMeshProUGUI>().text = result + "\nScore: " + score;

        int previousHighScore = PlayerPrefs.GetInt("Level2_HighScore", 0);
        if (score > previousHighScore)
        {
            PlayerPrefs.SetInt("Level2_HighScore", score);
            PlayerPrefs.Save();
        }

        StartCoroutine(ReturnToMainMenuAfterDelay(3f));
    }

    IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
