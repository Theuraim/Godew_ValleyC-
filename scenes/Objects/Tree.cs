using Godot;
using Game;
using GameEnum;
using System;

public partial class Tree : StaticBody2D
{
    //tree should flash when hit
    //apple 
    //tree is killable
    FlashSprite2d flash;
    Node2D appleSpawn;
    Node2D appleContainer;
    Sprite2D stump;
    CollisionShape2D collisionShape;

    Texture2D APPLETEXTURE = GD.Load<Texture2D>("res://graphics/plants/apple.png");

    private int _health = 3;

    public int Health
    {
        get { return _health; }
        set
        {
            // Garante que a vida não fique negativa
            _health = value;

            // Opcional: Chamar uma função ao mudar a vida
             if (_health <= 0) {
                flash.Hide();
                stump.Show();
                RectangleShape2D shape2D = new RectangleShape2D();
                shape2D.Size = new Vector2(12, 6); // Ajuste o tamanho conforme necessário
                collisionShape.Shape = shape2D;
            }
        }
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		flash = GetNode<FlashSprite2d>("FlashSprite2D");
        appleSpawn = GetNode<Node2D>("AppleSpawnPositions");
        appleContainer = GetNode<Node2D>("Apples");
        stump = GetNode<Sprite2D>("Stump");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        CreateApples(3);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double _delta)
	{
	}

	public void Hit(GameEnums.Tool tool)
	{
        if (tool == GameEnums.Tool.AXE && flash != null)
        {
            GD.Print("Tree hit with axe!");
            flash.Flash();
            getApple();
            Health--;
        }
    }

    public void CreateApples(int num)
    {
        Godot.Collections.Array<Node> appleMarkers = appleSpawn.GetChildren().Duplicate(true);

        for (int i = 0; i <= num; i++)
        {
            Random random = new Random();
            int index = random.Next(0, appleMarkers.Count);
            Node2D posMarker = appleMarkers[index] as Node2D;
            appleMarkers.RemoveAt(index);

            Sprite2D sprite = new Sprite2D();
            sprite.Texture = APPLETEXTURE;


            appleContainer.AddChild(sprite);
            sprite.Position = posMarker.Position;
        }
    }

    public void getApple()
    {
        if (appleContainer.GetChildCount() > 0)
        {
            Node apple = appleContainer.GetChildren().PickRandom();
            apple.QueueFree();
            GD.Print("Apple collected!");
        }
        else
        {
            GD.Print("No apples left to collect!");
        }
    }
}
