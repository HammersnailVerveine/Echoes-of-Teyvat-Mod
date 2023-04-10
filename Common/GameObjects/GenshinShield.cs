using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.GameObjects
{
    public abstract class GenshinShield
    {
        public int Health; // Shield remaining health
        public int Duration; // Shield remaining duration
		public int TimeSpent = 0; // Time spent active
        public GenshinElement Element = GenshinElement.NONE; // Shield Element

        public void UpdateBase(GenshinPlayer genshinPlayer) {
            Update(genshinPlayer);
        }

        public void ResetEffects()
        {
            Duration--;
			TimeSpent++;
        }

        public GenshinShield Initialize(int health, int duration, GenshinElement element = GenshinElement.NONE, int ai = 0) // "ai" is used to communicate more information if needed
        {
            OnInitialize(ref health, ref duration, ref element, ai);
            Health = health;
            Duration = duration;
            if (element != GenshinElement.NONE) Element = element;
            return this;
        }

        public virtual void OnInitialize(ref int health, ref int duration, ref GenshinElement element, int value) { }
        public virtual void Update(GenshinPlayer genshinPlayer) { }
        public virtual void Draw(SpriteBatch spriteBatch, Color lightColor, GenshinPlayer genshinPlayer) { }
	}
}
