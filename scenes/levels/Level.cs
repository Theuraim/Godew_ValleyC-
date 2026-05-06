using Godot;
using System;
using Game;
using GameEnum;
using System.Collections.Generic;

public partial class Level : Node2D
{
	CharacterBody2D player;
    TileMapLayer soilLayer;
	TileMapLayer grassLayer;
    TileMapLayer debugLayer;
    Vector2I lastCell;
	Vector2I gridCoord;
    private HashSet<Vector2I> wetSoil = new();
    private HashSet<Vector2I> plantedCell = new();
    private PackedScene plantSc;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		player = GetNode<CharacterBody2D>("Objects/Player");
		
		soilLayer = GetNode<TileMapLayer>("Layers/SoilLayer");

        grassLayer = GetNode<TileMapLayer>("Layers/GrassLayer");

		debugLayer = GetNode<TileMapLayer>("Layers/DebugLayer");

        plantSc = GD.Load<PackedScene>("res://scenes/Objects/plant.tscn");

        player.Connect(Player.SignalName.ToolUse, new Callable(this, nameof(OnToolUse)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double _delta)
	{
		Vector2 playerPos = player.GlobalPosition;
		Vector2 lastDirection = Input.GetVector("left", "right", "up", "down");

        Vector2 dir = new Vector2(Mathf.Sign(lastDirection.X), Mathf.Sign(lastDirection.Y));

        Vector2 pos = playerPos + (dir * Data.TILE_SIZE);
		gridCoord = grassLayer.LocalToMap(pos);

		debugLayer.Clear();
        debugLayer.SetCell(gridCoord, 0, new Vector2I(1, 3));
        //wetSoil.Remove(lastCell);
    }

    
	private bool IsSoil(Vector2I cell)
	{
		return soilLayer.GetCellSourceId(cell) != -1;
	}
	
	private bool IsGrass(Vector2I cell)
	{
		return grassLayer.GetCellSourceId(cell) != -1;
    }


    private void UpdateSoilVisual(Vector2I cell)
	{
		Random random = new Random();
		TileMapLayer wetSoilLayer = GetNode<TileMapLayer>("Layers/WetSoilLayer");
		// exemplo: tile 0 = seco, tile 1 = molhado
		wetSoilLayer.SetCell(cell, 0, new Vector2I(random.Next(0, 3),0));
	}

	private void OnToolUse(GameEnums.Tool tool, Vector2 pos)
	{
		Vector2I cell;

		//Vector2I gridCoord = new Vector2I((int) (pos.X / Data.TILE_SIZE), (int) (pos.Y / Data.TILE_SIZE));

		switch (tool)
		{
			case GameEnums.Tool.HOE:
				//TileMapLayer soilLayer = GetNode<TileMapLayer>("Layers/SoilLayer"); 
				cell = grassLayer.LocalToMap(pos);

				TileData grasscell = grassLayer.GetCellTileData(cell);

				if (grasscell != null)
				{
					Variant data = grasscell.GetCustomData("farmable");

					bool isFarmable = data.VariantType != Variant.Type.Nil ? data.AsBool() : false;

					GD.Print($"Cell {cell} farmable: {isFarmable}");

					if (isFarmable)
					{
						soilLayer.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { cell }, 0, 0);
						GD.Print(cell);
					}
					else
					{
						GD.Print($"Cell {cell} is not farmable.");
					}
				}
				break;
			case GameEnums.Tool.WATER:
				cell = soilLayer.LocalToMap(pos);

				if (!IsSoil(cell))
				{
					GD.Print($"Cell {cell} is not soil.");
					return;
				}

				if (wetSoil.Contains(cell))
					return; // não adiciona novamente se já estiver molhado

				GD.Print($"Watering cell: {cell}");
				wetSoil.Add(cell);

				UpdateSoilVisual(cell);

				lastCell = cell;
				break;
			case GameEnums.Tool.FISH:
				cell = grassLayer.LocalToMap(pos);

				// Implementar lógica para o machado
				if (!IsGrass(cell))
				{
					GD.Print("Fishing");
				}
				break;
			case GameEnums.Tool.SEED:
				cell = soilLayer.LocalToMap(pos);
				// Implementar lógica para a semente
				if (IsSoil(cell))
				{
					if (plantedCell.Contains(cell))
					{
						GD.Print($"Cell {cell} is already planted.");
						return; // não planta novamente se já estiver plantado
                    }

                    var plantInstance = plantSc.Instantiate<Plant>();
					plantInstance.Setup(cell, GetNode<Node>("Objects"));

                    plantedCell.Add(cell);
                }
				break;
		}
	}
}
