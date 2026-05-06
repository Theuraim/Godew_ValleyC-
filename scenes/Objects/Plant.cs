using Godot;
using System;
using Game;
using GameEnum;

public partial class Plant : StaticBody2D
{
	Vector2I coord;

	public void Setup(Vector2I cell, Node parent)
	{
		Position = cell * Data.TILE_SIZE + new Vector2I(8, 5);
		parent.AddChild(this);
		coord = cell;
    }
}
