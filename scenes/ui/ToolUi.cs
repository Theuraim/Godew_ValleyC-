using GameEnum;
using Godot;
using Godot.Collections;
using System;

public partial class ToolUi : Control
{
    HBoxContainer toolContainer;
    HBoxContainer seedContainer;
    Timer hideTimer;
    public static readonly Dictionary<GameEnums.Tool, Texture2D> TOOL_TEXTURES = new()
    {
        { GameEnums.Tool.AXE, GD.Load<Texture2D>("res://graphics/icons/axe.png") },
        { GameEnums.Tool.HOE, GD.Load<Texture2D>("res://graphics/icons/hoe.png") },
        { GameEnums.Tool.WATER, GD.Load<Texture2D>("res://graphics/icons/water.png") },
        { GameEnums.Tool.SWORD, GD.Load<Texture2D>("res://graphics/icons/sword.png") },
        { GameEnums.Tool.FISH, GD.Load<Texture2D>("res://graphics/icons/fish.png") },
        { GameEnums.Tool.SEED, GD.Load<Texture2D>("res://graphics/icons/wheat.png") }
    };

    public static readonly Dictionary<GameEnums.Seed, Texture2D> SEED_TEXTURES = new()
    {
        { GameEnums.Seed.CORN, GD.Load<Texture2D>("res://graphics/icons/corn.png") },
        { GameEnums.Seed.PUMPKIN, GD.Load<Texture2D>("res://graphics/icons/pumpkin.png") },
        { GameEnums.Seed.TOMATO, GD.Load<Texture2D>("res://graphics/icons/tomato.png") },
        { GameEnums.Seed.WHEAT, GD.Load<Texture2D>("res://graphics/icons/wheat.png") }
    };

    private PackedScene toolTextureScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        toolContainer = GetNode<HBoxContainer>("ToolContainer");
        seedContainer = GetNode<HBoxContainer>("SeedContainer");
        toolTextureScene = GD.Load<PackedScene>("res://scenes/ui/tool_ui_texture.tscn");
        hideTimer = GetNode<Timer>("HideTimer");

        foreach (HBoxContainer container in new Godot.Collections.Array<HBoxContainer> { toolContainer, seedContainer })
        {
            container.Hide();
        }
        
        TextureSetup(Enum.GetValues<GameEnums.Tool>(), TOOL_TEXTURES, toolContainer);
        TextureSetup(Enum.GetValues<GameEnums.Seed>(), SEED_TEXTURES, seedContainer);

        hideTimer.Connect("timeout", new Callable(this, nameof(OnHideTimeout)));
    }

	public void TextureSetup(GameEnums.Tool[] enum_list, Dictionary<GameEnums.Tool, Texture2D> textures, HBoxContainer container)
    {
        foreach (GameEnums.Tool t in enum_list)
        {
            ToolUiTexture tool_texture = toolTextureScene.Instantiate<ToolUiTexture>();
            tool_texture.Setup(t, textures[t]);
            container.AddChild(tool_texture);
        }
    }
	public void TextureSetup(GameEnums.Seed[] enum_list, Dictionary<GameEnums.Seed, Texture2D> textures, HBoxContainer container)
    {
        foreach (GameEnums.Seed s in enum_list)
        {
            ToolUiTexture tool_texture = toolTextureScene.Instantiate<ToolUiTexture>();
            tool_texture.Setup(s, textures[s]);
            container.AddChild(tool_texture);
        }
    }

    public void Reveal(Boolean tool = false)
    {
        hideTimer.Start();

        HBoxContainer currentContainer = tool ? toolContainer : seedContainer;

        foreach (HBoxContainer container in new Godot.Collections.Array<HBoxContainer> { toolContainer, seedContainer })
        {
            container.Hide();
        }


        currentContainer.Show();


        if (tool)
        {
            GameEnums.Tool target = GetParent<Player>().currentTool;

            foreach (ToolUiTexture texture in toolContainer.GetChildren())
            {
                texture.Highlight(target == texture.toolEnum);
            }
        }
        else
        {
            GameEnums.Seed target = GetParent<Player>().currentSeed;

            foreach (ToolUiTexture texture in seedContainer.GetChildren())
            {
                texture.Highlight(target == texture.seedEnum);
            }
        }
    }

    public void OnHideTimeout() {
        foreach (HBoxContainer container in new Godot.Collections.Array<HBoxContainer> { toolContainer, seedContainer })
        {
            container.Hide();
        }
    }
}
