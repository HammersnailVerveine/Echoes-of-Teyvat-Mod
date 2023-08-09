using GenshinMod.Common.GlobalObjets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects
{
    public abstract class GenshinNPC : ModNPC
    {
        public GenshinGlobalNPC GenshinGlobalNPC;
        public Vector2 TargetPosition = Vector2.Zero;
        public Vector2 TargetPositionForced = Vector2.Zero;

        public int TimeAlive = 0;
        public int TimerInState = 0;

        public bool NoBestiaryEntry = true;
        public bool TargetLocalPlayer => NPC.target == Main.myPlayer;
        public Vector2 GetTargetPosition => TargetPositionForced != Vector2.Zero ? TargetPositionForced : PlayerTarget.Center;
        public Vector2 VectorToTarget => GetTargetPosition - NPC.Center;
        public Player PlayerTarget => Main.player[NPC.target];
        public bool TileCollision() => Collision.AnyCollision(NPC.position, NPC.velocity, NPC.width, NPC.height).Length() > 0f;
        public bool TileCollision(Vector2 direction) => Collision.AnyCollision(NPC.position - direction * 0.01f, direction, NPC.width, NPC.height).Length() > 0f;
        public void SetFrame(int frame) => NPC.frame.Y = NPC.frame.Height * frame;
        public void UpdateTargetPosition() => TargetPosition = PlayerTarget.Center;
        public abstract void SafeSetDefaults();
        public abstract void SafeSetStaticDefaults();
        public virtual void SafeAI() { }
        public virtual void SafeResetEffects() { }
        public virtual void SafeOnHitByProjectile(Projectile projectile, GenshinGlobalProjectile globalProjectile) { }
        public virtual void OnTakeDamage(Player player, int damage) { } // Called anytime the npc takes damage. Damage is the remaining damage dealt to the npc. Player can be null.
        public virtual void OnFirstFrame() { }
        public virtual bool SafePreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => true;


        public const int AI_Field_State = 0;
        public const int AI_Field_Misc = 1;

        public float AI_Misc
        {
            get => NPC.ai[AI_Field_Misc];
            set => NPC.ai[AI_Field_Misc] = value;
        }

        public float AI_State
        {
            get => NPC.ai[AI_Field_State];
            set => NPC.ai[AI_Field_State] = value;
        }

        public void SetAI(int field, float value, bool netUpdate = true)
        {
            switch (field)
            {
                case AI_Field_State:
                    TimerInState = 0;
                    NPC.ai[AI_Field_State] = value;
                    break;
                case AI_Field_Misc:
                    NPC.ai[AI_Field_Misc] = value;
                    break;
                default:
                    break;
            }
            if (netUpdate) NPC.netUpdate = true;
        }

        public sealed override void SetDefaults()
        {
            NPC.value = 0f;
            GenshinGlobalNPC globalNPC = NPC.GetGlobalNPC<GenshinGlobalNPC>();
            GenshinGlobalNPC = globalNPC;
            SafeSetDefaults();
        }

        public sealed override void SetStaticDefaults()
        {
            SafeSetStaticDefaults();
            if (NoBestiaryEntry)
            {
                NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
                {
                    Hide = true
                };
                NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            }
        }

        public sealed override void AI()
        {
            if (TimeAlive == 0)
            {
                NPC.TargetClosest();
                OnFirstFrame();
            }

            if (!GenshinGlobalNPC.ReactionFrozen)
            {
                SafeAI();
                TimerInState++;
            }

            TimeAlive++;
            UpdateTargetPosition();
            TargetPositionForced = Vector2.Zero;
        }

        public sealed override void ResetEffects()
        {
            SafeResetEffects();
        }

        public sealed override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            SafeOnHitByProjectile(projectile, projectile.GetGlobalProjectile<GenshinGlobalProjectile>());
        }

        public sealed override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (TimeAlive > 0)
            {
                return SafePreDraw(spriteBatch, screenPos, drawColor);
            }
            return true;
        }

        public void SpawnDust<T>(float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1) where T : ModDust => SpawnDust(ModContent.DustType<T>(), velocity, scale, offSet, quantity, chanceDenominator);

        public void SpawnDust(int type, float velocity = 0f, float scale = 1f, int offSet = 10, int quantity = 1, int chanceDenominator = 1)
        {
            if (!Main.rand.NextBool(chanceDenominator)) return;
            for (int i = 0; i < quantity; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(NPC.position - new Vector2(offSet, offSet), NPC.width + offSet * 2, NPC.height + offSet * 2, type)];
                dust.velocity = new Vector2(Main.rand.NextFloat(-velocity, velocity), Main.rand.NextFloat(-velocity, velocity));
                dust.scale = scale;
            }
        }
    }
}