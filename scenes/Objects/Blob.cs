using GameEnum;
using Godot;
using System;

public partial class Blob : CharacterBody2D
{
	FlashSprite2d flash;
	Vector2 direction;
	AnimationPlayer animationPlayer;
	int speed = 20;
	int pushDistance = 130;
	[Export]
	public Vector2 PushDirection { get; set; }
	Player player;
    private int _health = 3;

    public int Health
    {
        get { return _health; }
        set
        {
            // Garante que a vida não fique negativa
            _health = value;

            // Opcional: Chamar uma função ao mudar a vida
            if (_health <= 0)
            {
				Death();
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        flash = GetNode<FlashSprite2d>("FlashSprite2D");
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double _delta)
	{
		direction = (player.Position - Position).Normalized();
		Velocity = direction * speed + PushDirection;
		MoveAndSlide();
	}

	public void Push()
	{
		Tween tween = GetTree().CreateTween();
		Vector2 target = (player.Position - Position).Normalized() * -1 * pushDistance;
		tween.TweenProperty(this, "PushDirection", target, 0.1);
		tween.TweenProperty(this, "PushDirection", Vector2.Zero, 0.2);
	}
	public void Hit(GameEnums.Tool tool)
	{
		if (tool == GameEnums.Tool.SWORD)
		{
			flash.Flash();
			Push();
			Health--;
		}
	}

	public void Death()
	{
		speed = 0;
		animationPlayer.CurrentAnimation = "explode";

	}

}
