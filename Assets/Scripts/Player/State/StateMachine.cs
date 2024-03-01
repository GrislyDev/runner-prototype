public class StateMachine
{

	private static StateMachine _instance;
	public static StateMachine Instance => _instance ??= new StateMachine();


	public static IState Idle { get; private set; }
	public static IState Running { get; private set; }
	public static IState Jump { get; private set; }
	public static IState Roll { get; private set; }
	public static IState Death { get; private set; }

	private IState _currentState;

	public StateMachine()
	{
		Idle = new IdleState();
		Running = new RunningState();
		Jump = new JumpState();
		Roll = new RollState();
		Death = new DeathState();

		_currentState = Idle;
	}

	public void ChangeState(IState state)
	{
		if (state == null)
		{
			UnityEngine.Debug.LogError("Invalid state provided");
			return;
		}

		_currentState?.Exit();
		_currentState = state;
		_currentState.Enter();
	}

	public void UpdateState() => _currentState?.Execute();
}