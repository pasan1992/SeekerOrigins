namespace HutongGames.PlayMaker.Actions
{
public class PlayerInteractAction : FsmStateAction
{
    // Start is called before the first frame update
    public AgentController m_playerController;
    private HumanoidMovingAgent m_agent;
    private bool interactionStarted = false;
    public Interactable Interactable;
    public override void OnEnter()
    {
        if(m_playerController == null)
        {
            m_playerController = PlayerController.getInstance();
        }
        m_agent = m_playerController.GetComponent<HumanoidMovingAgent>();
    }

    public override void OnUpdate()
    {
        StartInteraction();
    }

    private void StartInteraction()
    {
        interactionStarted = m_agent.InteractWith(Interactable);
        m_playerController.ControlDisabled = true;
        m_agent.stopAiming();
        if(interactionStarted)
        {
            m_playerController.ControlDisabled = false;
            Finish();
        }
    }
}

}
