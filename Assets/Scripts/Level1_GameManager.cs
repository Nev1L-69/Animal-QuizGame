using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController1 : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI scoreText;
    public GameObject winPanel;
    public Image questionImage; // дл€ отображени€ картинки вопроса

    private List<QuestionData> questions;
    private int currentQuestionIndex = 0;
    private int score = 0;
    private int requiredScore = 7; // минимум 7 из 10
    private bool isAnswering = false;

    void Start()
    {
        LoadQuestions();
        DisplayQuestion();
    }

    void LoadQuestions()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("level1_questions");
        questions = new List<QuestionData>(
            JsonUtility.FromJson<QuestionList>("{\"questions\":" + jsonFile.text + "}").questions
        );
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndLevel();
            return;
        }

        isAnswering = false;

        QuestionData q = questions[currentQuestionIndex];
        questionText.text = q.question;

        // ѕоказать изображение, если указано
        if (!string.IsNullOrEmpty(q.image))
        {
            questionImage.enabled = true;
            Sprite img = Resources.Load<Sprite>(q.image);
            questionImage.sprite = img;
        }
        else
        {
            questionImage.enabled = false;
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].interactable = true;
            optionButtons[i].GetComponent<Image>().color = Color.white;
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.options[i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        scoreText.text = "Score: " + score;
    }

    void OnAnswerSelected(int selectedIndex)
    {
        if (isAnswering) return;
        isAnswering = true;

        QuestionData q = questions[currentQuestionIndex];

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].interactable = false;
        }

        if (selectedIndex == q.correctIndex)
        {
            score++;
            optionButtons[selectedIndex].GetComponent<Image>().color = Color.green;
        }
        else
        {
            optionButtons[selectedIndex].GetComponent<Image>().color = Color.red;
            optionButtons[q.correctIndex].GetComponent<Image>().color = Color.green;
        }

        StartCoroutine(NextQuestionAfterDelay(1.5f));
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentQuestionIndex++;
        DisplayQuestion();
    }

    void EndLevel()
    {
        winPanel.SetActive(true);
        string result = score >= requiredScore ? "Level Passed!" : "Try Again.";
        winPanel.GetComponentInChildren<TextMeshProUGUI>().text = result + "\nScore: " + score;

        // —охран€ем максимальный счЄт дл€ уровн€ 1
        int previousHighScore = PlayerPrefs.GetInt("Level1_HighScore", 0);
        if (score > previousHighScore)
        {
            PlayerPrefs.SetInt("Level1_HighScore", score);
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
