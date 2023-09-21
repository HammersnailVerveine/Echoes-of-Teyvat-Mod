namespace GenshinMod.Common.GameObjects.Enums
{
    public enum GenshinModMessageType : byte
    {
        CharacterUseAbility,
        CharacterUseAbilityServer,
        CharacterStartHoldAbility,
        CharacterStartHoldAbilityServer,
        CharacterStopHoldAbility,
        CharacterStopHoldAbilityServer,
        PlayerSendCurrentCharacter,
        PlayerSendCurrentCharacterServer,
        PlayerRequestCharacter,
        PlayerRequestCharacterServer,
        NPCSyncElement,
        NPCSyncElementServer,
        CombatText,
        CombatTextReaction,
        CombatTextReactionServer,
        CombatTextServer
    }
}