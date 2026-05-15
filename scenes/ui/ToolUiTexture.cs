using GameEnum;
using Godot;
using System;

public partial class ToolUiTexture : Control
{
	public GameEnums.Tool toolEnum;
	public GameEnums.Seed seedEnum;
	TextureRect textureRect;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Setup(GameEnums.Tool newTool, Texture2D maintexture)
    {
        toolEnum = newTool;
        textureRect = GetNode<TextureRect>("TextureRect");
        textureRect.Texture = maintexture;
    }
    public void Setup(GameEnums.Seed newSeed, Texture2D maintexture)
    {
        seedEnum = newSeed;
        textureRect = GetNode<TextureRect>("TextureRect");
        textureRect.Texture = maintexture;
    }

    public void Highlight(Boolean selected)
    {
        Tween tween = CreateTween();
        Vector2 targetSize = selected? new Vector2(20f, 20f) : new Vector2(16f, 16f);
        tween.TweenProperty(textureRect, "custom_minimum_size", targetSize, 0.1);
    }
}
