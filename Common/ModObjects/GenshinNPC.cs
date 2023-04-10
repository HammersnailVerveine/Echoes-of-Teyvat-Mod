﻿using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.GlobalObjets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects
{
    public abstract class GenshinNPC : ModNPC
    {
        public GenshinGlobalNPC GenshinGlobalNPC;
        public Vector2 TargetPosition = Vector2.Zero;

        public bool TargetLocalPlayer => NPC.target == Main.myPlayer;

        public int TimeAlive = 0;
        public abstract void SafeSetDefaults();
        public virtual void SafeAI() { }
        public virtual void OnFirstFrame() { }

        public sealed override void SetDefaults()
        {
            NPC.value = 0f;
            GenshinGlobalNPC globalNPC = NPC.GetGlobalNPC<GenshinGlobalNPC>();
            GenshinGlobalNPC = globalNPC;
            SafeSetDefaults();
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