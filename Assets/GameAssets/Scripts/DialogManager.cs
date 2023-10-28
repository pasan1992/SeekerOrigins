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
        public float time = 0.2f;

        public DialogManager.Characters character;
        public Transform characterTransfrom;
    }

    // Start is called before the first frame update
    public static DialogManager instance = null;

    private Queue<DialogStatment> currentDialogStatments;
    private DialogStatment currentDialog;
    private AudioSource m_audioSource;

    public GameEvents.BasicNotifactionEvent onDialogEndCallback;
    public TMP_Text DialogText;
    public TMP_Text ObjectiveText;

    public Image CharacterImage;

    public GameObject dialogUI;

    private bool pause = false;

    public FloatingGameUI m_dialogIcon;

    [SerializeField] GameObject _waitingImg;


    public enum Characters {Alex_afraid,Alex_angry,Alex_happy,Alex_normal,Govnor_Happy,Govnor_Disapointed,Govnor_Normal,Govnor_Angry
                            ,Kangarian_Angry,Kangarian_Normal,Fang_normal};
    public Dictionary<DialogManager.Characters,Sprite> imageDict;

    private SoundManager soundManager;

    private bool noSkip = false;

    public void setNoSkip(bool noSkip)
    {
        this.noSkip = noSkip;
    }

    private PlayerController m_playercont;

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
        initalizeImages();
    }

    public void Start()
    {
        m_audioSource = this.GetComponent<AudioSource>();
        currentDialogStatments = new Queue<DialogStatment>();
        dialogUI.SetActive(false);
        m_playercont = PlayerController.getInstance();
    }

    private void initalizeImages()
    {
        imageDict = new Dictionary<DialogManager.Characters,Sprite>();
        foreach (Characters type in System.Enum.GetValues(typeof(Characters)))
        {   
            string rp_path = "";
            switch(type)
            {
                case Characters.Fang_normal:
                rp_path = "Images/Fang_normal";
                break;
                case Characters.Kangarian_Angry:
                rp_path = "Images/Kangarian_Angry";
                break;
                case Characters.Kangarian_Normal:
                rp_path = "Images/Kangarian_Normal";
                break;
                case Characters.Alex_afraid:
                rp_path = "Images/Alex_afraid";
                break;
                case Characters.Alex_angry:
                rp_path = "Images/Alex_angry";
                break;
                case Characters.Alex_happy:
                rp_path = "Images/Alex_happy";
                break;
                case Characters.Alex_normal:
                rp_path = "Images/Alex_normal";
                break;
                case Characters.Govnor_Disapointed:
                rp_path = "Images/Govnor_disapointed";
                break;
                case Characters.Govnor_Happy:
                rp_path = "Images/Govnor_happy";
                break;
                case Characters.Govnor_Normal:
                rp_path = "Images/Govnor_normal";
                break;
                case Characters.Govnor_Angry:
                rp_path = "Images/Gavnor_angry";
                break;
            }
            imageDict.Add(type,Resources.Load<Sprite>(rp_path));
        }
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
        onDialogEndCallback += endCallback;
    }

    public void SkipDialog()
    {
        if(noSkip)
        {
            return;
        }
        
        m_audioSource.Stop();
        //DialogText.text = "";
        currentDialogStatments = new Queue<DialogStatment>();
        NextDialog();
    }
    private void NextDialog()
    {  
        if(soundManager == null)
        {
            soundManager = SoundManager.getInstance();
        }

        if(currentDialogStatments.Count == 0)
        {
            if(onDialogEndCallback !=null)
            {
                onDialogEndCallback();
                DialogText.text = "";
                _waitingImg.SetActive(true);

                onDialogEndCallback = null;
                soundManager.setDialogPlaying(false);

            }
            dialogUI.SetActive(false);
            m_dialogIcon.gameObject.SetActive(false);
            return;
        }
        soundManager.setDialogPlaying(true);
        dialogUI.SetActive(true);
        pause=true;
        DialogText.text = "";
        _waitingImg.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(waitAndPlay());
    }

    private IEnumerator waitAndPlay()
    {

        if (currentDialog !=null)
        {
            if (currentDialog.time == 0)
            {
                currentDialog.time = 0.2f;
            }
            yield return new WaitForSeconds(currentDialog.time);
        }
        currentDialog = currentDialogStatments.Dequeue();
        pause=false;
        playDialog(currentDialog);
    }

    private void playDialog(DialogStatment statement)
    {
        _waitingImg.SetActive(false);

        m_audioSource.Stop();
        m_audioSource.PlayOneShot(statement.audio);
        //DialogText.text = statement.charName + " : " + statement.text + " (ENTER:SKIP)";
        DialogText.text = statement.text;

        CharacterImage.enabled = true;   
        CharacterImage.sprite = imageDict[statement.character];
        
        if(statement.characterTransfrom !=null)
        {
            m_dialogIcon.gameObject.SetActive(true);
            m_dialogIcon.target = statement.characterTransfrom;
        }
        else if(statement.character == Characters.Alex_afraid || statement.character == Characters.Alex_angry || statement.character == Characters.Alex_happy ||
        statement.character == Characters.Alex_normal)
        {
            m_dialogIcon.gameObject.SetActive(true);
            m_dialogIcon.target = m_playercont.getICyberAgent().getHeadTransfrom();
        }
        else{
            m_dialogIcon.gameObject.SetActive(false);
        }
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
