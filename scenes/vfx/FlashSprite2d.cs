using Godot;
using System;

public partial class FlashSprite2d : Sprite2D
{
	private static readonly StringName ProgressParameter = "Progress";
	private ShaderMaterial shaderMaterial;
	private Tween flashTween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		shaderMaterial = Material as ShaderMaterial;
		SetFlashProgress(0.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Flash(float startDuration = 0.2f, float endDuration = 0.2f)
	{
		if (shaderMaterial == null)
			return;

		flashTween?.Kill();
		flashTween = CreateTween();

		if (startDuration > 0.0f)
		{
			SetFlashProgress(0.0f);
			flashTween.TweenMethod(Callable.From<float>(SetFlashProgress), 0.0f, 1.0f, startDuration)
				.SetTrans(Tween.TransitionType.Sine);
		}
		else
		{
			SetFlashProgress(1.0f);
		}

		flashTween.TweenMethod(Callable.From<float>(SetFlashProgress), 1.0f, 0.0f, endDuration)
			.SetTrans(Tween.TransitionType.Sine);
    }

	private void SetFlashProgress(float value)
	{
		shaderMaterial?.SetShaderParameter(ProgressParameter, value);
	}
}
