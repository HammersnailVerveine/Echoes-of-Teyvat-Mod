using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Content.Characters.Amber.Projectiles
{
    public class AmberProjectileBurstMain : GenshinProjectile
    {
        public Texture2D ArrowTexture;
        public int ArrowType;

        public override void SetDefaults()
        {
            Projectile.width = 240;
            Projectile.height = 240;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            GenshinPlayer ownerPlayer = Owner.GetModPlayer<GenshinPlayer>();
            ArrowTexture = ModContent.Request<Texture2D>(ownerPlayer.CharacterCurrent.Weapon.Texture + "_Arrow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            ArrowType = ModContent.ProjectileType<AmberProjectileBurstArrow>();
        }

        public override void SafeAI()
        {
            if (TimeSpent % 6 == 0 && TimeSpent > 12)
            {
                ResetImmunity();
                Projectile.friendly = true;
            }

            if (TimeSpent % 3 == 0)
            {
                Vector2 position = new Vector2(Projectile.Center.X + Main.rand.NextFloat(-Projectile.width, Projectile.width) * 0.5f, Projectile.position.Y - 160);
                int projID = SpawnProjectile(position, Vector2.UnitY * 16f, ArrowType, Projectile.damage, Projectile.knockBack);
                if (Main.projectile[projID].ModProjectile is AmberProjectileBurstArrow arrow)
                {
                    arrow.ArrowTexture = ArrowTexture;
                    arrow.Projectile.rotation = arrow.Projectile.velocity.ToRotation();
                }
                if (TimeSpent % 6 == 0) SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
            }
        }
    }
}
