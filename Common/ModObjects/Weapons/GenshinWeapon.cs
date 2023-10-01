using GenshinMod.Common.GameObjects;
using GenshinMod.Common.GameObjects.Enums;
using GenshinMod.Common.ModObjects.Weapons.Projectiles;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GenshinMod.Common.ModObjects.Weapons
{
    public abstract class GenshinWeapon : ModItem
    {
        public WeaponType WeaponType;
        public StatType SubstatType;
        public GenshinRarity Rarity;
        public int BaseAttack;
        public float BaseSubstat;
        public int Level = 1; // Max 5
        public int Refinement = 1; // Max 5

        public GenshinCharacter Character;

        public float EffectiveAttack => BaseAttack / 5f * Level;
        public float EffectiveSubstat => BaseSubstat / 5f * Level;
        public Player Player => Character.Player;
        public GenshinPlayer genshinPlayer => Character.GenshinPlayer;
        public float RefineValue(float value) => value + (value * (Refinement - 1) * 0.25f);

        public virtual string CombatTexture => "GenshinMod/Content/Weapons/" + WeaponTypeUtils.ToString(WeaponType) + "/" + Name + "_Combat";

        protected override bool CloneNewInstances => true;

        public sealed override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("A GenshinMod weapon");
            SafeSetStaticDefaults();
        }

        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Default;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.None;
            Item.damage = 0;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 0, 1);
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 1f;

            SafeSetDefaultsWeaponType();
            SafeSetDefaults();
        }

        public void Equip(GenshinCharacter character)
        {
            character.Weapon = this;
            this.Character = character;
        }

        public static GenshinWeapon GetWeapon(int type)
        {
            Item item = new Item();
            item.SetDefaults(type);
            if (item.ModItem is GenshinWeapon weapon)
            {
                return weapon;
            }
            return null;
        }

        public static GenshinWeapon GetWeakestWeapon(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.BOW:
                    return GetWeapon(ModContent.ItemType<Content.Weapons.Bow.BowHuntersBow>());
                case WeaponType.CATALYST:
                    return GetWeapon(ModContent.ItemType<Content.Weapons.Catalyst.CatalystApprenticeNotes>());
                case WeaponType.POLEARM:
                    return null;
                case WeaponType.CLAYMORE:
                    return GetWeapon(ModContent.ItemType<Content.Weapons.Claymore.ClaymoreWasterGreatsword>());
                case WeaponType.SWORD:
                    return GetWeapon(ModContent.ItemType<Content.Weapons.Sword.SwordDullBlade>());
            }
            return null;
        }

        public virtual void KillProjectile()
        {
            var anchorType = ModContent.ProjectileType<WeaponAnchor>();

            if (Player.ownedProjectileCounts[anchorType] > 0)
            {
                var proj = Main.projectile.First(i => i.active && i.owner == Player.whoAmI && i.type == anchorType);
                if (proj != null && proj.ModProjectile is WeaponAnchor anchor)
                    anchor.OnKill(proj.timeLeft);
            }
        }

        public virtual void WeaponResetEffects() { }
        public virtual void WeaponOnSwapIn() { }
        public virtual void WeaponOnSwapOut() { }
        public virtual void WeaponPostUpdate() { } // Called during Character PostUpdate
        public virtual void WeaponUpdate() { } // Called during Character PreUpdate
        public virtual void WeaponPostUpdateActive() { } // Called during Active Character PostUpdate
        public virtual void WeaponUpdateActive() { } // Called during Active Character PreUpdate
        public virtual void SpawnVanityWeapon() { } // Called during Active Character PostUpdate
        public virtual void SafeSetStaticDefaults() { }
        public virtual void SafeSetDefaults() { }
        public virtual void SafeSetDefaultsWeaponType() { }
    }
}
