using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [System.Serializable]
    public class InteractableProperties
    {

        public enum InteractableType {FastInteraction,PickupInteraction,TimedInteraction,ContinousInteraction,DialogInteraction,FixedContinousInteraction,NullInteraction}
   
        public enum InteractionAction { LookAround = 1,Sit = 2,UseObject =3,Drinking = 4,Deativated=5,Crouching=6,Tracking=7,Idle=8,Making=9, Walking_Jump=10,Prayer1=11,Anger=12,Prayer2=13,Typing=14, Leaning=15, KneelingInspecting=16, SillyDancing = 17, SittingAngry =18, SittingClap =19, SittingDazed = 20, Defeted = 21, Sleeping = 22,Pickup=23, NotSpecified = -99}
        public InteractableType Type = InteractableType.PickupInteraction;
        private bool _interactionEnabled = true;

        public bool ObjectiveInteratable = false;
        public bool showIndicatorOnOBjActive = true;

        public bool interactionEnabled {
                    get
        {
            return _interactionEnabled;
        }

        set
        {
            _interactionEnabled = value;
            if(interatabilityChange !=null & ObjectiveInteratable)
            {
                interatabilityChange(value);
            }
            

        }
        }
        public string itemName = "";
        public float interactionTime;
        public InteractionAction InteractionAnim = InteractionAction.NotSpecified;
        public Vector3 offset = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public bool enablePositionRequirment = true;
        public bool placeObjectInHand = false;
        public Transform actualObject = null;
        public bool PlayerRestricted = false;
        public GameEvents.BasicEnableDisableEvent interatabilityChange; 
        public string InteractionSoundName = "";
    }

    [System.Serializable]
    public class VisualProperites
    {
        public Vector3 nameTagOffset;
        public Vector3 holdingPositionOffset;
        public Vector3 holdingRotationOffset;
        public InGameInidactor.IndicatorTypes indicatorType = InGameInidactor.IndicatorTypes.Arrow;

        public Vector3 indicator_offset = Vector3.zero;
        public Transform objectTransfrom;
    }

   [SerializeField]
    public InteractableProperties properties;

    [SerializeField]
    public VisualProperites visualProperties;

    private bool interacting;
    
    protected Outline m_outLine;

    // To Reset the actual object loaction after interaction
    private Vector3 m_relativePosition;
    private Vector3 m_relativeRotation;

    protected GameObject m_indicator;

    
    private GameEvents.BasicNotifactionEvent onInteractionStartCallback;
    private GameEvents.BasicNotifactionEvent onInteractionStopCallback;

    public GameEvents.BasicNotifactionEvent OnInteractionStopCallback { get => onInteractionStopCallback; 
    set
    {
        onInteractionStopCallback += value;
    }
    }

    public GameEvents.BasicNotifactionEvent OnInteractionStartCallback { get => onInteractionStartCallback; 
    set 
    {
        onInteractionStartCallback +=value;
    }
    }

    public enum outLineState {Disabled,white,Blue};
    public AudioSource m_baseAudioSource;

    public virtual void Awake()
    {
        if(visualProperties.objectTransfrom !=null)
        {
            m_outLine = visualProperties.objectTransfrom.GetComponent<Outline>();
        }

        if(m_outLine == null)
        {
            m_outLine = this.GetComponent<Outline>();
            if(m_outLine ==null)
            {
                m_outLine = this.GetComponentInChildren<Outline>();
            }
        }

        setOutLineState(outLineState.white);
        
        if(properties.actualObject != null)
        {
            m_relativePosition = properties.actualObject.localPosition;
            m_relativeRotation = properties.actualObject.localRotation.eulerAngles;
        }

        // if(properties.InteractionAnim !=InteractableProperties.InteractionAction.NotSpecified)
        // {
        //     properties.interactionID = (int)properties.InteractionAnim;
        // }

        // set event on interatability change
        properties.interatabilityChange +=setInteratableIndicator;
        
        m_baseAudioSource = this.GetComponent<AudioSource>();
        if(m_baseAudioSource == null)
        {
            this.gameObject.AddComponent<AudioSource>();
            m_baseAudioSource = this.GetComponent<AudioSource>();
        }
    }

    public void Start()
    {
        setOutLineState(outLineState.white);

        if(properties.ObjectiveInteratable)
        {
            SetInteratable(false);
        }
    }

    public virtual void OnPickUpAction()
    {
        properties.interactionEnabled = false;
        this.gameObject.SetActive(false);
        setOutLineState(outLineState.Disabled);
    }

    public virtual void OnEquipAction()
    {
        properties.interactionEnabled = false;
        setOutLineState(outLineState.Disabled);
        
    }
    
    public virtual void OnPlaceOnHoster()
    {

    }

    public virtual void OnPlaceOnHand()
    {

    }

    public virtual void OnItemDrop()
    {
        properties.interactionEnabled = true;
        interacting = false;
        setOutLineState(outLineState.white);
    }

    public virtual void setOutLineState(outLineState state)
    {
        if(!properties.interactionEnabled)
        {
            state = outLineState.Disabled;
        }   

        if(m_outLine)
        {
            switch(state)
            {
                case outLineState.Disabled:
                    m_outLine.OutlineWidth = 0;
                break;
                case outLineState.white:
                        m_outLine.OutlineWidth = 0.5f;
                        m_outLine.OutlineColor = Color.gray;
                break;
                case outLineState.Blue:
                        m_outLine.OutlineWidth = 0.5f;
                        m_outLine.OutlineColor = Color.white;
                break;
            }
            
        }
    }

    public virtual void interact()
    {
        interacting = true;

        if(onInteractionStartCallback!=null)
        {
            onInteractionStartCallback();
        }

        CommonFunctions.PlaySound(properties.InteractionSoundName,m_baseAudioSource);
        //Debug.Log("interact");
    }

    public virtual bool isInteracting()
    {
        return interacting;
    }

    public virtual void stopInteraction()
    {
        interacting = false;

        if(onInteractionStopCallback !=null)
            onInteractionStopCallback();

        resetObject();
    }

    public virtual void OnInteractionStart()
    {

    }

    public void resetObject()
    {
        if(properties.placeObjectInHand && properties.actualObject !=null)
        {
            properties.actualObject.parent  = this.transform;
            properties.actualObject.transform.localPosition = m_relativePosition;
            properties.actualObject.transform.localRotation = Quaternion.Euler(m_relativeRotation);
        }
    }

    public static bool IsValidIntearction(Interactable.InteractableProperties.InteractableType type)
    {
        return (type != InteractableProperties.InteractableType.NullInteraction);
    }

    private void setInteratableIndicator(bool int_enabled)
    {
        if(int_enabled)
        {
            if(properties.ObjectiveInteratable && properties.showIndicatorOnOBjActive)
            {
                if(m_indicator == null)
                {
                    m_indicator = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.Obj_Indicator);
                }
                
                m_indicator.SetActive(true);
                m_indicator.transform.position = this.transform.position + Vector3.up * 1 +visualProperties.indicator_offset;
                setOutLineState(outLineState.white);
                m_indicator.GetComponent<InGameInidactor>().IndicatorType = visualProperties.indicatorType;
            }

            return;
        }
        if(m_indicator!=null)
        {
            m_indicator.SetActive(false);
            m_indicator = null;
        }
        setOutLineState(outLineState.Disabled);     
    }

    public void SetInteratable(bool int_enabled)
    {
        properties.interactionEnabled = int_enabled;
    }

    #region editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 finalPosition = this.transform.position + ( Quaternion.Euler(properties.rotation) * Vector3.forward) * 1;
        Gizmos.DrawLine(transform.position+ properties.offset,finalPosition);
        Gizmos.DrawSphere(finalPosition,0.2f);

        //Gizmos.DrawCube(transform.position + new Vector3(0, 0.3f, 0), new Vector3(0.6f, 0.6f, 0.6f));

        var indiPosition = this.transform.position + Vector3.up * 1 +visualProperties.indicator_offset;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(indiPosition,0.5f);
    }
    #endregion
}
