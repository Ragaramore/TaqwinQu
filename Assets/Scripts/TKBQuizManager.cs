using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TKBQuizManager : MonoBehaviour
{
    public enum QuestionMode
    {
        ImageChoice,
        VoiceChoice
    }

    [System.Serializable]
    public class QuizOption
    {
        public string label;
        public Sprite image;
    }

    [System.Serializable]
    public class QuizQuestion
    {
        [Header("Question")]
        public string prompt;
        public QuestionMode mode = QuestionMode.ImageChoice;

        [Header("Voice Over")]
        public AudioClip voiceOverClip;
        public bool autoPlayVoiceOver = true;

        [Header("Options")]
        public QuizOption optionA = new QuizOption();
        public QuizOption optionB = new QuizOption();
        [Range(0, 1)] public int correctAnswerIndex = 0;

        [Header("Feedback")]
        [TextArea(2, 4)] public string correctFeedback = "Benar!";
        [TextArea(2, 4)] public string wrongFeedback = "Coba lagi ya.";
    }

    [Header("Quiz Content")]
    [SerializeField] private List<QuizQuestion> questions = new List<QuizQuestion>();

    [Header("UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private Text questionText;
    [SerializeField] private Text progressText;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Image optionAImage;
    [SerializeField] private Image optionBImage;
    [SerializeField] private Text optionALabel;
    [SerializeField] private Text optionBLabel;
    [SerializeField] private Button optionAButton;
    [SerializeField] private Button optionBButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button repeatVoiceButton;

    [Header("Audio")]
    [SerializeField] private AudioSource voiceOverSource;

    [Header("Behavior")]
    [SerializeField] private bool showQuizOnStart = false;
    [SerializeField] private bool shuffleQuestions = false;
    [SerializeField] private bool lockUntilAnsweredCorrectly = true;
    [SerializeField] private float feedbackShowDuration = 1.0f;

    private readonly List<QuizQuestion> runtimeQuestions = new List<QuizQuestion>();
    private int currentQuestionIndex = -1;
    private bool answeredThisQuestion;
    private Coroutine feedbackRoutine;

    private void Awake()
    {
        if (voiceOverSource == null)
        {
            voiceOverSource = GetComponent<AudioSource>();
        }

        if (voiceOverSource == null)
        {
            voiceOverSource = gameObject.AddComponent<AudioSource>();
        }

        voiceOverSource.playOnAwake = false;
        voiceOverSource.spatialBlend = 0f;
    }

    private void Start()
    {
        PrepareQuestions();
        HookButtons();

        if (quizPanel != null)
        {
            quizPanel.SetActive(showQuizOnStart);
        }

        if (questions.Count == 0)
        {
            SetFeedback("Belum ada pertanyaan di Quiz Manager.");
            SetInteractable(false);
            return;
        }

        if (showQuizOnStart)
        {
            ShowQuestion(0);
        }
    }

    private void OnDestroy()
    {
        UnhookButtons();
    }

    private void PrepareQuestions()
    {
        runtimeQuestions.Clear();
        runtimeQuestions.AddRange(questions);

        if (shuffleQuestions)
        {
            for (int i = 0; i < runtimeQuestions.Count; i++)
            {
                int randomIndex = Random.Range(i, runtimeQuestions.Count);
                (runtimeQuestions[i], runtimeQuestions[randomIndex]) = (runtimeQuestions[randomIndex], runtimeQuestions[i]);
            }
        }
    }

    private void HookButtons()
    {
        if (optionAButton != null)
        {
            optionAButton.onClick.RemoveListener(AnswerA);
            optionAButton.onClick.AddListener(AnswerA);
        }

        if (optionBButton != null)
        {
            optionBButton.onClick.RemoveListener(AnswerB);
            optionBButton.onClick.AddListener(AnswerB);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextQuestion);
            nextButton.onClick.AddListener(NextQuestion);
        }

        if (repeatVoiceButton != null)
        {
            repeatVoiceButton.onClick.RemoveListener(ReplayVoiceOver);
            repeatVoiceButton.onClick.AddListener(ReplayVoiceOver);
        }
    }

    private void UnhookButtons()
    {
        if (optionAButton != null)
        {
            optionAButton.onClick.RemoveListener(AnswerA);
        }

        if (optionBButton != null)
        {
            optionBButton.onClick.RemoveListener(AnswerB);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextQuestion);
        }

        if (repeatVoiceButton != null)
        {
            repeatVoiceButton.onClick.RemoveListener(ReplayVoiceOver);
        }
    }

    public void ShowQuestion(int questionIndex)
    {
        if (runtimeQuestions.Count == 0)
        {
            SetFeedback("Belum ada pertanyaan.");
            return;
        }

        StopFeedbackRoutine();
        HideNextButton();

        currentQuestionIndex = Mathf.Clamp(questionIndex, 0, runtimeQuestions.Count - 1);
        answeredThisQuestion = false;

        QuizQuestion currentQuestion = runtimeQuestions[currentQuestionIndex];

        if (questionText != null)
        {
            questionText.text = currentQuestion.prompt;
        }

        if (progressText != null)
        {
            progressText.text = (currentQuestionIndex + 1).ToString() + " / " + runtimeQuestions.Count.ToString();
        }

        if (optionAImage != null)
        {
            optionAImage.sprite = currentQuestion.optionA != null ? currentQuestion.optionA.image : null;
            optionAImage.enabled = optionAImage.sprite != null;
        }

        if (optionBImage != null)
        {
            optionBImage.sprite = currentQuestion.optionB != null ? currentQuestion.optionB.image : null;
            optionBImage.enabled = optionBImage.sprite != null;
        }

        if (optionALabel != null)
        {
            optionALabel.text = currentQuestion.optionA != null ? currentQuestion.optionA.label : "";
        }

        if (optionBLabel != null)
        {
            optionBLabel.text = currentQuestion.optionB != null ? currentQuestion.optionB.label : "";
        }

        SetFeedback(string.Empty);
        SetInteractable(true);

        if (currentQuestion.autoPlayVoiceOver && currentQuestion.voiceOverClip != null)
        {
            PlayVoiceOver(currentQuestion.voiceOverClip);
        }
        else
        {
            StopVoiceOver();
        }
    }

    public void OpenQuiz()
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
        }

        HideNextButton();

        if (runtimeQuestions.Count == 0)
        {
            PrepareQuestions();
        }

        if (runtimeQuestions.Count > 0)
        {
            ShowQuestion(0);
        }
    }

    public void CloseQuiz()
    {
        StopFeedbackRoutine();
        HideNextButton();

        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }
    }

    public void AnswerA()
    {
        Answer(0);
    }

    public void AnswerB()
    {
        Answer(1);
    }

    public void Answer(int selectedIndex)
    {
        if (currentQuestionIndex < 0 || currentQuestionIndex >= runtimeQuestions.Count)
        {
            return;
        }

        if (answeredThisQuestion && lockUntilAnsweredCorrectly)
        {
            return;
        }

        QuizQuestion currentQuestion = runtimeQuestions[currentQuestionIndex];
        bool isCorrect = selectedIndex == currentQuestion.correctAnswerIndex;

        if (isCorrect)
        {
            answeredThisQuestion = true;
            SetFeedback(currentQuestion.correctFeedback);
            SetInteractable(false);

            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(true);
                nextButton.interactable = true;
            }

            if (feedbackRoutine != null)
            {
                StopCoroutine(feedbackRoutine);
            }

            feedbackRoutine = StartCoroutine(ShowFeedbackThenEnableNext());
        }
        else
        {
            SetFeedback(currentQuestion.wrongFeedback);

            if (!lockUntilAnsweredCorrectly)
            {
                ShowQuestion(currentQuestionIndex);
            }
        }
    }

    private System.Collections.IEnumerator ShowFeedbackThenEnableNext()
    {
        yield return new WaitForSeconds(feedbackShowDuration);
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    public void NextQuestion()
    {
        if (!answeredThisQuestion)
        {
            HideNextButton();
            return;
        }

        StopFeedbackRoutine();

        int nextIndex = currentQuestionIndex + 1;
        if (nextIndex >= runtimeQuestions.Count)
        {
            SetFeedback("Selesai! Hebat!");
            SetInteractable(false);
            if (nextButton != null)
            {
                nextButton.interactable = false;
            }
            return;
        }

        ShowQuestion(nextIndex);
    }

    public void ReplayVoiceOver()
    {
        if (currentQuestionIndex < 0 || currentQuestionIndex >= runtimeQuestions.Count)
        {
            return;
        }

        QuizQuestion currentQuestion = runtimeQuestions[currentQuestionIndex];
        if (currentQuestion.voiceOverClip != null)
        {
            PlayVoiceOver(currentQuestion.voiceOverClip);
        }
    }

    private void PlayVoiceOver(AudioClip clip)
    {
        if (voiceOverSource == null || clip == null)
        {
            return;
        }

        voiceOverSource.Stop();
        voiceOverSource.clip = clip;
        voiceOverSource.Play();
    }

    private void StopVoiceOver()
    {
        if (voiceOverSource != null && voiceOverSource.isPlaying)
        {
            voiceOverSource.Stop();
        }
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    private void SetInteractable(bool state)
    {
        if (optionAButton != null)
        {
            optionAButton.interactable = state;
        }

        if (optionBButton != null)
        {
            optionBButton.interactable = state;
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }
    }

    private void HideNextButton()
    {
        if (nextButton != null)
        {
            nextButton.interactable = false;
            nextButton.gameObject.SetActive(false);
        }
    }

    private void StopFeedbackRoutine()
    {
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }
    }
}
