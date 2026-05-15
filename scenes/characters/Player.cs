using Godot;
using System;
using Game;
using GameEnum;

public partial class Player : CharacterBody2D
{
    private static readonly Vector2 ToolTargetOffset = new Vector2(0, 4);

    private const float WALK_SPEED = 75;
    private const float RUN_SPEED = 125;
    private const float RUN_ACCELERATION = 250;
    private const float FOCUS_SPEED = 20f;
    private bool isFocusing = false;

    public bool IsFocusing => isFocusing;
    private bool isRunning = false;

    [Export]
	public float speed { get; set; } = WALK_SPEED;
	
	public bool canMove = true;
	
	Vector2 direction;
    public Vector2 lastDirection { get; private set; }
    AnimationNodeStateMachinePlayback move_state_machine;
	AnimationNodeStateMachinePlayback tool_state_machine;
	AnimationTree animationTree;
	
	public  GameEnums.Tool currentTool;// = GameEnums.Tool.AXE;
	public GameEnums.Seed currentSeed;

	ToolUi toolUi;

	
	[Signal]
	public delegate void ToolUseEventHandler(GameEnums.Tool tool, Vector2 pos);
	
	public override void _Ready(){
		animationTree = GetNode<AnimationTree>("Animation/AnimationTree");
		
		//carrega as variaveis
		move_state_machine = (AnimationNodeStateMachinePlayback)
		animationTree.Get("parameters/MoveStateMachine/playback");
		
		tool_state_machine = (AnimationNodeStateMachinePlayback)
		animationTree.Get("parameters/ToolStateMachine/playback");

		toolUi = GetNode<ToolUi>("ToolUI");
		
		currentTool = GameEnums.Tool.AXE;
		currentSeed = GameEnums.Seed.CORN;
		
		//Conecta os signals
		animationTree.Connect("animation_finished", new Callable(this, nameof(OnAnimationTreeAnimationFinished)));
		animationTree.Connect("animation_started", new Callable(this, nameof(OnAnimationTreeAnimationStarted)));
	}
	
	public override void _PhysicsProcess(double delta){
        isRunning = Input.IsKeyPressed(Key.Shift);
        isFocusing = Input.IsKeyPressed(Key.Ctrl);

        if (isFocusing)
        {
            speed = FOCUS_SPEED;
        }
        else if (isRunning)
        {
            speed = Mathf.MoveToward(
                speed,
                RUN_SPEED,
                RUN_ACCELERATION * (float)delta
            );
        }
        else
        {
            speed = WALK_SPEED;
        }


        if (canMove){
			getBasicInput();
			Move();
			Animate(); 
		}
		
		if (direction != Vector2.Zero && !isFocusing)
        {
			lastDirection = direction;
		}
	}
	
	public void getBasicInput(){
		if ((Input.IsActionJustPressed("tool_forward")) || (Input.IsActionJustPressed("tool_backward"))){
			int dir = (int)Input.GetAxis("tool_backward", "tool_forward");
			int toolIndex = (int) currentTool;
			int toolCount = Enum.GetValues(typeof(GameEnums.Tool)).Length;
			toolIndex = Math.Abs((toolIndex + dir + toolCount) % toolCount);
			
			currentTool = (GameEnums.Tool) toolIndex;

			toolUi.Reveal(true);
		}
		
		if (Input.IsActionJustPressed("seed_forward")){
            int seedIndex = (int) currentSeed;
			int seedCount = Enum.GetValues(typeof(GameEnums.Seed)).Length;
			seedIndex = Math.Abs((seedIndex + 1 + seedCount) % seedCount);
			
			currentSeed = (GameEnums.Seed) seedIndex;
			toolUi.Reveal();
		}
		
		if (!isRunning && Input.IsActionJustPressed("action")){
			if (Data.TOOL_STATE_ANIMATIONS.TryGetValue(currentTool, out string animation)){
				tool_state_machine.Travel(animation);
			}
			
			animationTree.Set("parameters/ToolOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
		}
	}
	
	public void Move(){
		direction = Input.GetVector("left", "right", "up", "down");
		
		/*if (direction != Vector2.Zero){
			lastDirection = direction;
		}*/
		
		Velocity = direction * speed;
		MoveAndSlide();
	}
	
	public void Animate(){
		
		if (direction != Vector2.Zero){
			move_state_machine.Travel("Walk");
			Vector2 direction_animation = new Vector2(
				Mathf.Sign(lastDirection.X),
				Mathf.Sign(lastDirection.Y)
			);
			animationTree.Set("parameters/MoveStateMachine/Idle/blend_position", direction_animation);
			if (Data.TOOL_STATE_ANIMATIONS.TryGetValue(currentTool, out string animation)){
				string path = $"parameters/ToolStateMachine/{animation}/blend_position";
				animationTree.Set(path, direction_animation);
			}										
			animationTree.Set("parameters/MoveStateMachine/Walk/blend_position", direction_animation);
		} else {
			move_state_machine.Travel("Idle");
		}
	}

    public Vector2 GetToolTargetPosition()
    {
        Vector2 dir = new Vector2(Mathf.Sign(lastDirection.X), Mathf.Sign(lastDirection.Y));
        return GlobalPosition + (dir * Data.TILE_SIZE) + ToolTargetOffset;
    }
	
	public void ToolUseEmit()
	{
		EmitSignal(SignalName.ToolUse, (int)currentTool, GetToolTargetPosition());
	}

	public void OnAnimationTreeAnimationFinished(StringName animName)
	{ 
		canMove = true;
	}
	
	
	public void OnAnimationTreeAnimationStarted(StringName animName)
	{
        canMove = false;
    }
}
