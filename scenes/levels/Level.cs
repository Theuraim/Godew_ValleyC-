using Godot;
using System;
using Game;
using GameEnum;
using System.Collections.Generic;
using System.Linq;

public partial class Level : Node2D
{
	Player player;
    TileMapLayer soilLayer;
	TileMapLayer grassLayer;
    TileMapLayer waterLayer;
    TileMapLayer debugLayer;
    Vector2I lastCell;
	Vector2I gridCoord;
    private HashSet<Vector2I> wetSoil = new();
    private HashSet<Vector2I> plantedCell = new();
    private PackedScene plantSc;

    private Vector2I GlobalToMap(TileMapLayer layer, Vector2 globalPosition)
    {
        return layer.LocalToMap(layer.ToLocal(globalPosition));
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		player = GetNode<Player>("Objects/Player");
		
		soilLayer = GetNode<TileMapLayer>("Layers/SoilLayer");

        waterLayer = GetNode<TileMapLayer>("Layers/WaterLayer");

        grassLayer = GetNode<TileMapLayer>("Layers/GrassLayer");

		debugLayer = GetNode<TileMapLayer>("Layers/DebugLayer");

        plantSc = GD.Load<PackedScene>("res://scenes/Objects/plant.tscn");
        player.Connect(Player.SignalName.ToolUse, new Callable(this, nameof(OnToolUse)));
	}


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double _delta)
	{
        debugLayer.Clear();

        if (player == null)
            return;

        if (!player.IsFocusing)
            return;


        Vector2 pos = player.GetToolTargetPosition();
		gridCoord = GlobalToMap(soilLayer, pos);


		//GD.Print("posicao do debug: " + gridCoord);
        debugLayer.SetCell(gridCoord, 0, new Vector2I(0, 0));
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
        Vector2I soilCell = GlobalToMap(soilLayer, pos);
        Vector2I grassCell = GlobalToMap(grassLayer, pos);
		Vector2I cell = tool == GameEnums.Tool.WATER || tool == GameEnums.Tool.SEED
			? soilCell
			: grassCell;
 
        //Vector2I gridCoord = new Vector2I((int) (pos.X / Data.TILE_SIZE), (int) (pos.Y / Data.TILE_SIZE));

        switch (tool)
		{
			case GameEnums.Tool.HOE:
				//TileMapLayer soilLayer = GetNode<TileMapLayer>("Layers/SoilLayer"); 

				TileData grassData = grassLayer.GetCellTileData(grassCell);
                //Vector2I atlas = grassLayer.GetCellAtlasCoords(grassCell);


                if (grassData == null)
					return;

                if (!grassData.GetCustomData("farmable").AsBool())
					return;
                
				//if (!fAtlasCoord.Contains(atlas))
				//	return;

                soilLayer.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { soilCell }, 0, 0);
				GD.Print("arando tile do solo na posicao: " + soilCell + " usando a grama da posicao: " + grassCell + " é aravel? : " + grassData.GetCustomData("farmable").AsBool());
				
				break;
			case GameEnums.Tool.WATER:

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
                cell.X += pos.X < 0 ? -1 : 0;
                cell.Y += pos.Y < 0 ? -1 : 0;

                // Implementar lógica para o machado
                if ((!IsGrass(cell)) && (!IsSoil(cell)))
				{
					GD.Print("Fishing");
				}
				break;
			case GameEnums.Tool.SEED:
				cell = GlobalToMap(soilLayer, pos);
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
			case GameEnums.Tool.AXE:
			case GameEnums.Tool.SWORD:
				var objects = GetTree().GetNodesInGroup("Objects").OfType<Node2D>();
                foreach (Node2D subject in objects)
                {

                    GD.Print($"Hitting {subject} with {tool}");

                    if (!(subject.GlobalPosition.DistanceTo(pos) <= 20))
						continue;

                    GD.Print($"Hitting {subject} with {tool}");
					Tree tree = tool == GameEnums.Tool.AXE ? subject as Tree : null;	

					if (tree != null)
						tree.Hit(tool);
					// else if (tool == GameEnums.Tool.SWORD)
					//{
					//	GD.Print($"Attacking {subject} with {tool}");
					//	subject.Hit(tool); }
                }
				break;

        }
	}
}
