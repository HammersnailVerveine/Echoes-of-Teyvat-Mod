using GenshinMod.Common.GameObjects;
using GenshinMod.Common.ModObjects;
using GenshinMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
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
                        if (albedo.skillActive && albedo.skillCooldown <= 0 && projectile.type != ModContent.ProjectileType<Content.Characters.Albedo.Projectiles.AlbedoProjectileSkillMain>()
                            && projectile.type != ModContent.ProjectileType<Content.Characters.Albedo.Projectiles.AlbedoProjectileBlast>())
                        { // Albedo E
                            foreach (Projectile proj in Main.projectile)
                            {
                                if (proj.active && proj.owner == genshinProjectile.Owner.whoAmI && proj.ModProjectile is Content.Characters.Albedo.Projectiles.AlbedoProjectileSkillMain albedoFlower)
                                {
                                    if (Vector2.Distance(proj.Center, npc.Center) < Content.Characters.Albedo.Projectiles.AlbedoProjectileSkillMain.Range)
                                    {
                                        albedoFlower.SpawnProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Content.Characters.Albedo.Projectiles.AlbedoProjectileBlast>(), albedo.AbilitySkill.GetScaling2(), 0f);
                                        albedo.skillCooldown = 120;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    if (character is Content.Characters.Noelle.CharacterNoelle noelle &&
                        (projectile.type == ModContent.ProjectileType<ProjectileClaymoreCharged>() || projectile.type == ModContent.ProjectileType<ProjectileClaymoreNormal>()))
                    { // Noelle E heal
                        if (Main.rand.NextBool(2) && noelle.HealTimer < 1)
                        {
                            foreach (GenshinShield shield in noelle.GenshinPlayer.Shields)
                            {
                                if (shield is Content.Characters.Noelle.Shields.ShieldNoelle)
                                {
                                    foreach (GenshinCharacter toHeal in noelle.GenshinPlayer.CharacterTeam)
                                        toHeal.Heal(noelle.AbilitySkill.GetScaling3());
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