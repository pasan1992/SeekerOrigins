using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{

    [System.Serializable]
    public class DialogStatment
    {
        public string text;
        public AudioClip audio;
        public string charName;
        public float time;
    }

    // Start is called before the first frame update
    public static DialogManager instance = null;

    private Queue<DialogStatment> currentDialogStatments;
    private DialogStatment currentDialog;
    private AudioSource m_audioSource;

    public GameEvents.BasicNotifactionEvent onDialogEndCallback;
    public TMP_Text DialogText;
    public TMP_Text ObjectiveText;
    public GameObject dialogUI;

    private bool pause = false;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        m_audioSource = this.GetComponent<AudioSource>();
        currentDialogStatments = new Queue<DialogStatment>();
        dialogUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!pause && !m_audioSource.isPlaying)
        {
            NextDialog();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            SkipDialog();
        }
    }

    public void SetObjectiveText(string text)
    {
        ObjectiveText.text = text;
    }

    public void StartDialog(DialogStatment[] dialogList, GameEvents.BasicNotifactionEvent endCallback)
    {
        currentDialog = null;
        m_audioSource.Stop();
        currentDialogStatments = new Queue<DialogStatment>(dialogList);

        if(onDialogEndCallback !=null)
        {
            onDialogEndCallback();
        }
        onDialogEndCallback = endCallback;
    }

    public void SkipDialog()
    {
        m_audioSource.Stop();
        //DialogText.text = "";
        NextDialog();
    }
    private void NextDialog()
    {  
        if(currentDialogStatments.Count == 0)
        {
            if(onDialogEndCallback !=null)
            {
                onDialogEndCallback();
                DialogText.text = "";
                onDialogEndCallback = null;
            }
            dialogUI.SetActive(false);
            return;
        }
        dialogUI.SetActive(true);
        pause=true;
        DialogText.text = "";
        StopAllCoroutines();
        StartCoroutine(waitAndPlay());
    }

    private IEnumerator waitAndPlay()
    {
        if(currentDialog !=null)
        {
            yield return new WaitForSeconds(currentDialog.time);
        }
        currentDialog = currentDialogStatments.Dequeue();
        pause=false;
        playDialog(currentDialog);
    }

    private void playDialog(DialogStatment statement)
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(statement.audio);
        DialogText.text = statement.charName + " : " + statement.text + " (ENTER:SKIP)";
    }

    public int getRemaningLines()
    {
        if(currentDialogStatments !=null)
        {
            return currentDialogStatments.Count;
        }
        return 0;
    }
}
