using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [System.Serializable]
    public class InteractableProperties
    {

        public enum InteractableType {FastInteraction,PickupInteraction,TimedInteraction,ContinousInteraction,DialogInteraction,NullInteraction}
        public enum InteractionAction { LookAround = 1,Sit = 2,UseObject =3,Drinking = 4,Deativated=5,Crouching=6,Tracking=7,Idle=8,Making=9, Walking_Jump=10, NotSpecified = -99}
        public InteractableType Type = InteractableType.PickupInteraction;
        public bool interactionEnabled = false;
        public string itemName = "";
        public float interactionTime;
        public int interactionID;
        public InteractionAction InteractionAnim = InteractionAction.NotSpecified;
        public Vector3 offset = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public bool enablePositionRequirment = true;
        public bool placeObjectInHand = false;
        public Transform actualObject = null;
        public bool PlayerRestricted = false;
    }

    [System.Serializable]
    public class VisualProperites
    {
        public Vector3 nameTagOffset;
        public Vector3 holdingPositionOffset;
        public Vector3 holdingRotationOffset;
    }

   [SerializeField]
    public InteractableProperties properties;

    [SerializeField]
    public VisualProperites visualProperties;

    private bool interacting;
    
    private Outline m_outLine;

    // To Reset the actual object loaction after interaction
    private Vector3 m_relativePosition;
    private Vector3 m_relativeRotation;

    
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

    public virtual void Awake()
    {
        m_outLine = this.GetComponent<Outline>();
        setOutLineState(false);
        
        if(properties.actualObject != null)
        {
            m_relativePosition = properties.actualObject.localPosition;
            m_relativeRotation = properties.actualObject.localRotation.eulerAngles;
        }

        if(properties.InteractionAnim !=InteractableProperties.InteractionAction.NotSpecified)
        {
            properties.interactionID = (int)properties.InteractionAnim;
        }
    }

    public virtual void OnPickUpAction()
    {
        properties.interactionEnabled = false;
        this.gameObject.SetActive(false);
        setOutLineState(false);
    }

    public virtual void OnEquipAction()
    {
        properties.interactionEnabled = false;
        setOutLineState(false);
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
        setOutLineState(true);
    }

    public virtual void setOutLineState(bool state)
    {
        if(m_outLine)
        {
            if(state)
            {
                m_outLine.OutlineWidth = 0.7F;
                m_outLine.OutlineColor = Color.blue;
            }
            else
            {
                m_outLine.OutlineWidth = 0.2f;
                m_outLine.OutlineColor = Color.white;
            }
            
        }
    }

    public virtual void interact()
    {
        interacting = true;

        if(onInteractionStartCallback!=null)
            onInteractionStartCallback();

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
}
