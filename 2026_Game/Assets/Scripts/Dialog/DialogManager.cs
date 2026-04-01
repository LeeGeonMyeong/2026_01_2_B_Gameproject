using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("Dialog References")]
    [SerializeField] private DialogDatabaseSO dialogDatabase;

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button NextButton;

    [Header("Dialog Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool useTypewriterEffect = true;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private DialogSO currentDialog;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Database 체크
        if (dialogDatabase != null)
        {
            dialogDatabase.Initailize();
        }
        else
        {
            Debug.LogError("Dialog Database is not assigned to DialogManager!");
        }

        // NextButton 체크
        if (NextButton != null)
        {
            NextButton.onClick.AddListener(NextDialog);
        }
        else
        {
            Debug.LogError("NextButton is not assigned in Inspector!");
        }

        // UI 필드 체크
        if (characterNameText == null) Debug.LogError("CharacterNameText is not assigned!");
        if (dialogText == null) Debug.LogError("DialogText is not assigned!");
        if (portraitImage == null) Debug.LogError("PortraitImage is not assigned!");
        if (dialogPanel == null) Debug.LogError("DialogPanel is not assigned!");
    }

    private void Start()
    {
        CloseDialog();
        StartDialog(1); // 자동으로 첫 번째 대화 시작
    }

    public void StartDialog(int dialogId)
    {
        if (dialogDatabase == null) return;

        DialogSO dialog = dialogDatabase.GetDialogByld(dialogId);
        if (dialog != null)
        {
            StartDialog(dialog);
        }
        else
        {
            Debug.LogError($"Dialog with ID {dialogId} not found!");
        }
    }

    public void StartDialog(DialogSO dialog)
    {
        if (dialog == null) return;

        currentDialog = dialog;
        dialogPanel?.SetActive(true);
        ShowDialog();
    }

    public void ShowDialog()
    {
        if (currentDialog == null) return;

        if (characterNameText != null)
            characterNameText.text = currentDialog.characterName;
        else
            Debug.LogWarning("CharacterNameText is null!");

        if (useTypewriterEffect && dialogText != null)
            StartTypingEffect(currentDialog.text);
        else if (dialogText != null)
            dialogText.text = currentDialog.text;

        // 초상화 설정
        if (portraitImage != null)
        {
            if (currentDialog.portrait != null)
            {
                portraitImage.sprite = currentDialog.portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else if (!string.IsNullOrEmpty(currentDialog.portraitPath))
            {
                Sprite portrait = Resources.Load<Sprite>(currentDialog.portraitPath);
                if (portrait != null)
                {
                    portraitImage.sprite = portrait;
                    portraitImage.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"Portrait not found at path: {currentDialog.portraitPath}");
                    portraitImage.gameObject.SetActive(false);
                }
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }
    }

    public void CloseDialog()
    {
        dialogPanel?.SetActive(false);
        currentDialog = null;
        StopTypingEffect();
    }

    public void NextDialog()
    {
        if (isTyping)
        {
            StopTypingEffect();
            if (dialogText != null && currentDialog != null)
                dialogText.text = currentDialog.text;
            isTyping = false;
            return;
        }

        if (currentDialog != null && currentDialog.nextld > 0)
        {
            DialogSO nextDialog = dialogDatabase.GetDialogByld(currentDialog.nextld);
            if (nextDialog != null)
            {
                currentDialog = nextDialog;
                ShowDialog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }

    private IEnumerator TypeText(string text)
    {
        if (dialogText == null) yield break;

        dialogText.text = "";
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void StopTypingEffect()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    private void StartTypingEffect(string text)
    {
        if (dialogText == null) return;

        isTyping = true;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }
}