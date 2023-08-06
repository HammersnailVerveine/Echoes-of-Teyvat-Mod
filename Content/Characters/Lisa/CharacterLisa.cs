using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Characters.Lisa.Abilities;
using GenshinMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace GenshinMod.Content.Characters.Lisa
{
    public class CharacterLisa : GenshinCharacter
    {
        public Texture2D TextureSkill;
        public List<LisaSkillNPCTimer> SkillNPCTimers;

        public override void SetDefaults()
        {
            Name = "Lisa";
            Element = GenshinElement.ELECTRO;
            WeaponType = WeaponType.CATALYST;
            AbilityNormal = new AbilityLisaNormal().Initialize(this);
            AbilityCharged = new AbilityLisaCharged().Initialize(this);
            AbilitySkill = new AbilityLisaSkill().Initialize(this);
            AbilityBurst = new AbilityLisaBurst().Initialize(this);

            BaseHealthMax = 9570;
            BaseAttackMax = 232;
            BaseDefenseMax = 573;

            BurstQuotes[0] = "Let's spark things up a little";
            BurstQuotes[1] = "Surrender and I'll be gentle";
            BurstQuotes[2] = "Try not to enjoy this too much";

            TextureSkill = ModContent.Request<Texture2D>("GenshinMod/Content/Characters/Lisa/Projectiles/LisaProjectileSkill", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            SkillNPCTimers = new List<LisaSkillNPCTimer>();
        }

        public override void SafeResetEffects()
        {
            for (int i = SkillNPCTimers.Count - 1; i >= 0; i--)
            {
                SkillNPCTimers[i].Timer--;
                if (SkillNPCTimers[i].Timer <= 0 || SkillNPCTimers[i].Stacks < 1 || !SkillNPCTimers[i].npc.active) SkillNPCTimers.RemoveAt(i);
            }
        }

        public int GetNPCStacks(NPC npc)
        {
            foreach (LisaSkillNPCTimer timer in SkillNPCTimers)
            {
                if (timer.npc == npc)
                {
                    int nb = timer.Stacks;
                    timer.Stacks = 0;
                    return nb;
                }
            }
            return 0;
        }

        public void TryApplyStackLisa(NPC npc)
        {
            foreach (LisaSkillNPCTimer timer in SkillNPCTimers)
            {
                if (timer.npc == npc)
                {
                    timer.TryApplyStack();
                    return;
                }
            }

            SkillNPCTimers.Add(new LisaSkillNPCTimer(npc));
            return;
        }

        public override void DrawEffects(SpriteBatch spriteBatch, Color lightColor)
        {
            int nbDots = 150;
            float segment = (MathHelper.TwoPi / nbDots);
            Color color = new Color(155, 155, 255);
            float rotationBonus = 0.0174533f * GenshinPlayer.Timer; // 1 degree in radians
            float lightFactor = ((float)Math.Sin(GenshinPlayer.Timer * 0.25f) + 1.5f) * 0.25f * (AbilitySkill.HoldTime > 45 ? 1f : (1f / 30f) * (AbilitySkill.HoldTime - 15));

            if (AbilitySkill.HoldTime > 15)
            {
                for (int i = 0; i < nbDots; i++)
                {
                    Vector2 position = Player.Center + (Vector2.UnitY * AbilityLisaSkill.Range).RotatedBy(segment * i + rotationBonus);
                    Vector2 drawPosition = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Player.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                    //float sizeFactor = ((float)Math.Sin(TimeSpent * 0.125f)) * 0.33f + 0.8f;
                    //spriteBatch.Draw(TextureSkill, drawPosition, null, Color.White * lightFactor, Main.rand.NextFloat(3.14f), TextureSkill.Size() * 0.5f, 1.75f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureSkill, drawPosition, null, color * lightFactor * 0.5f, Main.rand.NextFloat(3.14f), TextureSkill.Size() * 0.5f, 0.7f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureSkill, drawPosition, null, color * lightFactor * 0.25f, Main.rand.NextFloat(3.14f), TextureSkill.Size() * 0.5f, 1.5f, SpriteEffects.None, 0f);

                    GenshinProjectile.SpawnDust<LisaDustRound>(position, Vector2.Zero, 1f, 1f, 10, 1, 120);
                }

                for (int i = 0; i < nbDots; i++)
                {
                    Vector2 position = (Vector2.UnitY * AbilityLisaSkill.Range).RotatedBy(segment * i + rotationBonus);

                    if (!AbilitySkill.HoldFull)
                    {
                        position.Normalize();
                        position *= (AbilityLisaSkill.Range / (AbilitySkill.HoldTimeFull - 15)) * (AbilitySkill.HoldTime - 15);
                    }

                    position = Player.Center + position;

                    Vector2 drawPosition = Vector2.Transform(position - Main.screenPosition + new Vector2(0f, Player.gfxOffY), Main.GameViewMatrix.EffectMatrix);
                    //float sizeFactor = ((float)Math.Sin(TimeSpent * 0.125f)) * 0.33f + 0.8f;
                    spriteBatch.Draw(TextureSkill, drawPosition, null, color * lightFactor * 0.5f, Main.rand.NextFloat(3.14f), TextureSkill.Size() * 0.5f, 0.7f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureSkill, drawPosition, null, color * lightFactor * 0.25f, Main.rand.NextFloat(3.14f), TextureSkill.Size() * 0.5f, 1.5f, SpriteEffects.None, 0f);

                }

                if (Main.rand.NextBool(10))
                {
                    Vector2 position = Player.Center + (Vector2.UnitY * Main.rand.NextFloat(100f, AbilityLisaSkill.Range)).RotatedByRandom(MathHelper.ToRadians(360));
                    GenshinProjectile.SpawnDust<LisaDustRound>(position, Vector2.Zero, 1f, 1f, 10);
                }

                if (GenshinPlayer.Timer % 5 == 0 && AbilitySkill.HoldFull)
                {
                    Vector2 direction = (Vector2.UnitY * Main.rand.NextFloat(5f, 15f)).RotatedByRandom(MathHelper.ToRadians(360));
                    GenshinProjectile.SpawnDust<LisaDustRound>(Player.Center, direction, 1f, 1f, 10);
                }
            }

            foreach (LisaSkillNPCTimer timer in SkillNPCTimers)
            {
                Vector2 drawPosition = Vector2.Transform(timer.npc.Center - Main.screenPosition + new Vector2(6f, 6f), Main.GameViewMatrix.EffectMatrix);
                if (timer.Stacks == 1) drawPosition.X += 4f;

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, timer.Stacks.ToString(), drawPosition, color, 0f, Vector2.Zero, Vector2.One);
            }
        }
    }

    public class LisaSkillNPCTimer
    {
        public NPC npc;
        public int Timer;
        public int Stacks;

        public LisaSkillNPCTimer(NPC npc)
        {
            this.npc = npc;
            Timer = 60 * 15;
            Stacks = 1;
        }

        public void TryApplyStack()
        {
            Stacks++;
            Timer = 60 * 15;
            if (Stacks > 3) Stacks = 3;
        }
    }
}
