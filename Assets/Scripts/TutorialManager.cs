using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private GameObject tutorialPopupPrefab;
    [SerializeField] private GameObject tutorialPopupPrefabNoButton;
    [SerializeField] private RectTransform canvasTransform;

    private HashSet<string> seenTutorials = new();
    private Queue<string> pendingMessages = new();
    private bool popupActive = false;
    private static string noteTypeTail = "_Note";
    private string AltNoteMessage = "This is an Alt Note. It is much like a tap note, with the only difference being that you are to press the designated alt key (left arrow)";
    private string HoldNoteMessage = "This is a Hold Note. To hit this note, you hold space starting at its head, and release at its tail";
    private string SkipNoteMessage = "This is a Skip Note. Tapping any key for this note results in a miss, you want to let this one slip by";
    private string TapNoteMessage = "This is a Tap Note. Hit it by pressing the Space Bar once it reaches the hit zone";

    public string NoteTypeTail => noteTypeTail;
   private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowIfUnseen("Cast_Line", "Try walking towards the lake and use [MB1] to cast your rod!");
    }

    public bool HasSeen(string tutorialId)
    {
        return seenTutorials.Contains(tutorialId);
    }

    public void ShowIfUnseen(string tutorialId, string message)
    {
        if (seenTutorials.Contains(tutorialId)) return;

        seenTutorials.Add(tutorialId);
        pendingMessages.Enqueue(message);

        if (!popupActive)
        {
            ShowNext(tutorialPopupPrefab);
        }
    }

    private void ShowNext(GameObject popUpPrefab)
    {
        if (pendingMessages.Count == 0)
        {
            popupActive = false;
            return;
        }

        popupActive = true;
        string message = pendingMessages.Dequeue();

        GamePauseManager.Instance.RequestPause();
        GameObject popup = Instantiate(popUpPrefab, canvasTransform);
        popup.GetComponent<TutorialPopup>().Show(message, OnPopupDismissed);
    }

    private void OnPopupDismissed()
    {
        GamePauseManager.Instance.ReleasePause();
        if (pendingMessages.Count > 0 && IsNewNoteMessage(pendingMessages.ElementAt(0))) {

            ShowNext(tutorialPopupPrefabNoButton); // immediately shows the next queued tutorial, if any

        }
        else
        {
            ShowNext(tutorialPopupPrefab);
        }
    }

    private bool IsNewNoteMessage(string message)
    {
        if(message == AltNoteMessage || message == HoldNoteMessage || message == TapNoteMessage)
        {
            return true;
        }

        return false;
    }

       

    public void ShowIfUnseenNoteType(string noteType)
    {
        if (seenTutorials.Contains(noteType + NoteTypeTail)) return;
        string message = "";
        if (noteType == NoteType.Altkey.ToString())
        {
            message = AltNoteMessage;
        }
        else if (noteType == NoteType.Hold.ToString())
        {
            message = HoldNoteMessage;
        }
        else if (noteType == NoteType.Skip.ToString())
        {
            message = SkipNoteMessage;
        }
        else
        {
            message = TapNoteMessage;
        }
            
        seenTutorials.Add(noteType + NoteTypeTail);

        pendingMessages.Enqueue(message);

        if (!popupActive)
        {
            ShowNext(tutorialPopupPrefabNoButton);
        }
    }
}