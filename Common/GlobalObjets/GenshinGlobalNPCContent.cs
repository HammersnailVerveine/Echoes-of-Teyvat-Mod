using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.GlobalObjets
{
    public class GenshinGlobalNPCContent : GlobalNPC
    {
        // This class is used to handle "content-specific" effects that are not common to every character or element in the mod
        // --> Albedo flower hits for example, that are specific to Albedo

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (projectile.ModProjectile is GenshinProjectile genshinProjectile)
            {
                foreach (GenshinCharacter character in genshinProjectile.OwnerGenshinPlayer.CharacterTeam)
                {
                    if (character is Content.Characters.Albedo.CharacterAlbedo albedo)
                    {
                        if (albedo.skillActive && albedo.skillCooldown <= 0)
                        {
                            foreach (Projectile proj in Main.projectile)
                            {
                                if (proj.active && proj.owner == genshinProjectile.Owner.whoAmI && proj.ModProjectile is Content.Characters.Albedo.Projectiles.AlbedoProjectileSkillMain albedoFlower)
                                {
                                    if (Vector2.Distance(proj.Center, npc.Center) < Content.Characters.Albedo.Projectiles.AlbedoProjectileSkillMain.Range)
                                    {
                                        albedoFlower.SpawnProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Content.Characters.Lisa.Projectiles.LisaProjectileBurstHit>(), albedo.AbilitySkill.GetScaling2(), 0f);
                                        albedo.skillCooldown = 60;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}