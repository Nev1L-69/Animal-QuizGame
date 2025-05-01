[System.Serializable]
public class QuestionData
{
    public string question;
    public string[] options;
    public int correctIndex;
    public string image; // путь к изображению в Resources
}


[System.Serializable]
public class QuestionList
{
    public QuestionData[] questions;
}
