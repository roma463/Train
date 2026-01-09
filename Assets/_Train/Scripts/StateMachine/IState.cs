
public interface IState
{
    public void Execute();
    public void FixedExecute();
    public void OnEnterState();
    public void OnExitState();
}
