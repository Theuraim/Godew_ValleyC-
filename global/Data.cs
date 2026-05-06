using Godot;
using System.Collections.Generic;
using GameEnum;


namespace Game {
	public static class Data
	{
		// =========================
		// ENUMS
		// =========================
		/*public enum Style { BASIC, COWBOY, ENGLISH, BASEBALL, BEANIE, STRAW, CAP }
		public enum State { DEFAULT, FISHING, BUILDING, SHOP }
		public enum Tool { AXE, HOE, SWORD, WATER, FISH, SEED }
		public enum Machine { SPRINKLER, FISHER, SCARECROW, DELETE }
		public enum Seed { TOMATO, CORN, PUMPKIN, WHEAT }
		public enum Item { WOOD, APPLE, TOMATO, CORN, WHEAT, PUMPKIN, FISH }
		public enum Shop { MAIN, HAT }*/

		// =========================
		// CONSTANTS
		// =========================
		public const int TILE_SIZE = 16;

		// =========================
		// DATA CLASSES
		// =========================
		public class PlantData
		{
			public Texture2D Texture;
			public Texture2D IconTexture;
			public string Name;
			public int HFrames;
			public float GrowSpeed;
			public int DeathMax;
			public GameEnums.Item Reward;
		}

		public class MachineData
		{
			public string Name;
			public Dictionary<GameEnums.Item, int> Cost;
			public Texture2D Icon;
			public Color Color;
		}

		public class StyleData
		{
			public string Name;
			public Dictionary<GameEnums.Item, int> Cost;
			public Texture2D Icon;
			public Color Color;
		}

		// =========================
		// PLAYER SKINS
		// =========================
		public static readonly Dictionary<GameEnums.Style, Texture2D> PLAYER_SKINS = new()
		{
			{ GameEnums.Style.BASIC, GD.Load<Texture2D>("res://graphics/characters/main/main_basic.png") },
			{ GameEnums.Style.BASEBALL, GD.Load<Texture2D>("res://graphics/characters/main/main_blue.png") },
			{ GameEnums.Style.COWBOY, GD.Load<Texture2D>("res://graphics/characters/main/main_cowboy.png") },
			{ GameEnums.Style.ENGLISH, GD.Load<Texture2D>("res://graphics/characters/main/main_grey.png") },
			{ GameEnums.Style.STRAW, GD.Load<Texture2D>("res://graphics/characters/main/main_straw.png") },
			{ GameEnums.Style.BEANIE, GD.Load<Texture2D>("res://graphics/characters/main/main_red.png") }
		};

		// =========================
		// PLANTS
		// =========================
		public static readonly Dictionary<GameEnums.Seed, PlantData> PLANT_DATA = new()
		{
			{
				GameEnums.Seed.TOMATO,
				new PlantData {
					Texture = GD.Load<Texture2D>("res://graphics/plants/tomato.png"),
					IconTexture = GD.Load<Texture2D>("res://graphics/icons/tomato.png"),
					Name = "Tomato",
					HFrames = 3,
					GrowSpeed = 0.6f,
					DeathMax = 3,
					Reward = GameEnums.Item.TOMATO
				}
			},
			{
				GameEnums.Seed.CORN,
				new PlantData {
					Texture = GD.Load<Texture2D>("res://graphics/plants/corn.png"),
					IconTexture = GD.Load<Texture2D>("res://graphics/icons/corn.png"),
					Name = "Corn",
					HFrames = 3,
					GrowSpeed = 1.0f,
					DeathMax = 2,
					Reward = GameEnums.Item.CORN
				}
			},
			{
				GameEnums.Seed.PUMPKIN,
				new PlantData {
					Texture = GD.Load<Texture2D>("res://graphics/plants/pumpkin.png"),
					IconTexture = GD.Load<Texture2D>("res://graphics/icons/pumpkin.png"),
					Name = "Pumpkin",
					HFrames = 3,
					GrowSpeed = 0.3f,
					DeathMax = 3,
					Reward = GameEnums.Item.PUMPKIN
				}
			},
			{
				GameEnums.Seed.WHEAT,
				new PlantData {
					Texture = GD.Load<Texture2D>("res://graphics/plants/wheat.png"),
					IconTexture = GD.Load<Texture2D>("res://graphics/icons/wheat.png"),
					Name = "Wheat",
					HFrames = 3,
					GrowSpeed = 1.0f,
					DeathMax = 3,
					Reward = GameEnums.Item.WHEAT
				}
			}
		};

		// =========================
		// MACHINE COSTS
		// =========================
		public static readonly Dictionary<GameEnums.Machine, MachineData> MACHINE_UPGRADE_COST = new()
		{
			{
				GameEnums.Machine.SPRINKLER,
				new MachineData {
					Name = "Sprinkler",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.TOMATO, 30 },
						{ GameEnums.Item.WHEAT, 20 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/sprinkler.png"),
					Color = Colors.SeaGreen
				}
			},
			{
				GameEnums.Machine.FISHER,
				new MachineData {
					Name = "Fisher",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.WOOD, 25 },
						{ GameEnums.Item.FISH, 15 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/fisher.png"),
					Color = Colors.SlateGray
				}
			},
			{
				GameEnums.Machine.SCARECROW,
				new MachineData {
					Name = "Scarecrow",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.PUMPKIN, 15 },
						{ GameEnums.Item.CORN, 15 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/scarecrow.png"),
					Color = Colors.Burlywood
				}
			}
		};

		// =========================
		// HOUSE COST
		// =========================
		public static readonly Dictionary<int, Dictionary<GameEnums.Item, int>> HOUSE_COST = new()
		{
			{
				1, new Dictionary<GameEnums.Item, int> {
					{ GameEnums.Item.WOOD, 30 },
					{ GameEnums.Item.APPLE, 20 }
				}
			},
			{
				2, new Dictionary<GameEnums.Item, int> {
					{ GameEnums.Item.WOOD, 40 },
					{ GameEnums.Item.APPLE, 30 }
				}
			}
		};

		// =========================
		// STYLE UPGRADES
		// =========================
		public static readonly Dictionary<GameEnums.Style, StyleData> STYLE_UPGRADES = new()
		{
			{
				GameEnums.Style.COWBOY,
				new StyleData {
					Name = "Cowboy",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.WOOD, 8 },
						{ GameEnums.Item.CORN, 6 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/cowboy.png"),
					Color = Colors.SandyBrown
				}
			},
			{
				GameEnums.Style.ENGLISH,
				new StyleData {
					Name = "Oldie",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.CORN, 8 },
						{ GameEnums.Item.WHEAT, 6 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/english.png"),
					Color = Colors.LightGray
				}
			},
			{
				GameEnums.Style.BASEBALL,
				new StyleData {
					Name = "Baseball",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.TOMATO, 8 },
						{ GameEnums.Item.APPLE, 6 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/blue.png"),
					Color = Colors.SkyBlue
				}
			},
			{
				GameEnums.Style.BEANIE,
				new StyleData {
					Name = "Beanie",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.PUMPKIN, 8 },
						{ GameEnums.Item.WHEAT, 6 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/beanie.png"),
					Color = Colors.IndianRed
				}
			},
			{
				GameEnums.Style.STRAW,
				new StyleData {
					Name = "Straw",
					Cost = new Dictionary<GameEnums.Item, int> {
						{ GameEnums.Item.FISH, 8 },
						{ GameEnums.Item.WOOD, 6 }
					},
					Icon = GD.Load<Texture2D>("res://graphics/icons/straw.png"),
					Color = Colors.Burlywood
				}
			}
		};

		// =========================
		// TOOL ANIMATIONS
		// =========================
		public static readonly Dictionary<GameEnums.Tool, string> TOOL_STATE_ANIMATIONS = new()
		{
			{ GameEnums.Tool.HOE, "Hoe" },
			{ GameEnums.Tool.AXE, "Axe" },
			{ GameEnums.Tool.WATER, "Water" },
			{ GameEnums.Tool.SWORD, "Sword" },
			{ GameEnums.Tool.FISH, "Fish" },
			{ GameEnums.Tool.SEED, "Seed" }
		};
	}
}
