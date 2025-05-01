[System.Serializable]
public class QuestionData2
{
    public string question;
    public bool correctAnswer;
    public string image;
    public string audio;
    public string video;
}

[System.Serializable]
public class QuestionList2
{
    public QuestionData2[] questions;
}
