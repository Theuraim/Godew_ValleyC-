using Godot;
using System;

namespace GameEnum
{
	public static class GameEnums
	{
		public enum Style
		{
			BASIC,
			COWBOY,
			ENGLISH,
			BASEBALL,
			BEANIE,
			STRAW,
			CAP
		}

		public enum State
		{
			DEFAULT,
			FISHING,
			BUILDING,
			SHOP
		}

		public enum Tool
		{
			AXE,
			HOE,
			SWORD,
			WATER,
			FISH,
			SEED
		}

		public enum Machine
		{
			SPRINKLER,
			FISHER,
			SCARECROW,
			DELETE
		}

		public enum Seed
		{
			TOMATO,
			CORN,
			PUMPKIN,
			WHEAT
		}

		public enum Item
		{
			WOOD,
			APPLE,
			TOMATO,
			CORN,
			WHEAT,
			PUMPKIN,
			FISH
		}

		public enum Shop
		{
			MAIN,
			HAT
		}
	}
}
