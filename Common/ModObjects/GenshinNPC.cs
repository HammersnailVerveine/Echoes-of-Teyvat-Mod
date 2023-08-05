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

        public bool NoBestiaryEntry = true;

        public bool TargetLocalPlayer => NPC.target == Main.myPlayer;
        public Vector2 GetTargetPosition => TargetPositionForced != Vector2.Zero ? TargetPositionForced : TargetPosition;

        public Player PlayerTarget => Main.player[NPC.target];

        public int TimeAlive = 0;
        public abstract void SafeSetDefaults();
        public abstract void SafeSetStaticDefaults();
        public virtual void SafeAI() { }
        public virtual void OnFirstFrame() { }
        public virtual bool SafePreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => true;

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
            SafeAI();
            TimeAlive++;
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