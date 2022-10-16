using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.GameObjects
{
    public class GenshinGlobalProjectile : GlobalProjectile
    {
        public GenshinCharacter OwnerCharacter;
        public override bool InstancePerEntity => true;
    }
}  