namespace HutongGames.PlayMaker.Actions
{
public class CanclePlayerIntaraction : FsmStateAction
{
    // Start is called before the first frame update
    public AgentController m_playerController;
    private HumanoidMovingAgent m_agent;
    private bool interactionStarted = false;
    public override void OnEnter()
    {
        m_agent = m_playerController.GetComponent<HumanoidMovingAgent>();
        m_playerController.ControlDisabled = false;
        m_agent.cancleInteraction();
        Finish();
    }
}

}
