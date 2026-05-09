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
    private HashSet<Vector2I> blockedHoeBorderCells = new();
    private static readonly Vector2I[] CardinalDirections =
    {
        Vector2I.Up,
        Vector2I.Down,
        Vector2I.Left,
        Vector2I.Right
    };

    private Vector2I GlobalToMap(TileMapLayer layer, Vector2 globalPosition)
    {
        return layer.LocalToMap(layer.ToLocal(globalPosition));
    }

    private HashSet<Vector2I> GetGrassBorderCells(TileMapLayer groundLayer, TileMapLayer maskLayer)
    {
        HashSet<Vector2I> dryGrassCells = new HashSet<Vector2I>(groundLayer.GetUsedCells());
        HashSet<Vector2I> blockedCells = new HashSet<Vector2I>(maskLayer.GetUsedCells());

        dryGrassCells.ExceptWith(blockedCells);
        blockedCells.UnionWith(BuildExteriorBorderCells(dryGrassCells));

        return blockedCells;
    }

    private static HashSet<Vector2I> BuildExteriorBorderCells(HashSet<Vector2I> grassCells)
    {
        HashSet<Vector2I> borderCells = new HashSet<Vector2I>();

        if (grassCells.Count == 0)
            return borderCells;

        int minX = grassCells.Min(cell => cell.X) - 1;
        int minY = grassCells.Min(cell => cell.Y) - 1;
        int maxX = grassCells.Max(cell => cell.X) + 1;
        int maxY = grassCells.Max(cell => cell.Y) + 1;

        Queue<Vector2I> queue = new Queue<Vector2I>();
        HashSet<Vector2I> exteriorCells = new HashSet<Vector2I>();
        Vector2I start = new Vector2I(minX, minY);

        queue.Enqueue(start);
        exteriorCells.Add(start);

        while (queue.Count > 0)
        {
            Vector2I current = queue.Dequeue();

            foreach (Vector2I direction in CardinalDirections)
            {
                Vector2I next = current + direction;

                if (next.X < minX || next.X > maxX || next.Y < minY || next.Y > maxY)
                    continue;

                if (grassCells.Contains(next) || !exteriorCells.Add(next))
                    continue;

                queue.Enqueue(next);
            }
        }

        foreach (Vector2I cell in grassCells)
        {
            foreach (Vector2I direction in CardinalDirections)
            {
                if (!exteriorCells.Contains(cell + direction))
                    continue;

                borderCells.Add(cell);
                break;
            }
        }

        return borderCells;
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
        blockedHoeBorderCells = GetGrassBorderCells(grassLayer, waterLayer);

        player.Connect(Player.SignalName.ToolUse, new Callable(this, nameof(OnToolUse)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double _delta)
	{
        Vector2 pos = player.GetToolTargetPosition();
		gridCoord = GlobalToMap(soilLayer, pos);


		//GD.Print("posicao do debug: " + gridCoord);

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

    public static readonly Vector2I[] fAtlasCoord = new Vector2I[]
	{
        new Vector2I(1, 1),
        new Vector2I(0, 4),
        new Vector2I(1, 4),
        new Vector2I(2, 4),
        new Vector2I(3, 4)
    };

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

				TileData grasscell = grassLayer.GetCellTileData(grassCell);
                Vector2I atlas = grassLayer.GetCellAtlasCoords(grassCell);


                if (grasscell == null)
					return;

                if (!(bool)grasscell.GetCustomData("farmable").AsBool())
					return;
                
				//if (!fAtlasCoord.Contains(atlas))
				//	return;

                if (blockedHoeBorderCells.Contains(grassCell))
                    return;

                soilLayer.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { soilCell }, 0, 0);
				GD.Print("arando tile do solo na posicao: " + soilCell + " usando a grama da posicao: " + grassCell + " é aravel? : " + grasscell.GetCustomData("farmable").AsBool());
				
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
		}
	}
}
