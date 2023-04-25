using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace GenshinMod.Common.GameObjects
{
    public abstract class GenshinShield
    {
        public int Health; // Shield remaining health
        public int Duration; // Shield remaining duration
        public int TimeSpent = 0; // Time spent active
        public GenshinElement Element = GenshinElement.NONE; // Shield Element
        public GenshinPlayer GenshinPlayer;
        public Player Player => GenshinPlayer.Player;

        public void UpdateBase()
        {
            Update();
        }

        public void OnKillBase(bool killByDamage)
        {
            OnKill(killByDamage);
        }

        public void ResetEffects()
        {
            Duration--;
            TimeSpent++;
        }

        public GenshinShield Initialize(GenshinPlayer genshinPlayer, int health, int duration, GenshinElement element = GenshinElement.NONE, int ai = 0) // "ai" is used to communicate more information if needed
        {
            GenshinPlayer = genshinPlayer;
            OnInitialize(ref health, ref duration, ref element, ai);
            Health = health;
            Duration = duration;
            if (element != GenshinElement.NONE) Element = element;
            return this;
        }

        public virtual void OnInitialize(ref int health, ref int duration, ref GenshinElement element, int value) { }
        public virtual void Update() { }
        public virtual void OnKill(bool killByDamage) { }
        public virtual void Draw(SpriteBatch spriteBatch, Color lightColor, GenshinPlayer genshinPlayer) { }
    }
}
