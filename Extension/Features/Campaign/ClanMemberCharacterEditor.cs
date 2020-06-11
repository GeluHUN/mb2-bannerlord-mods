using Extension.Config;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;

namespace Extension.Features.Campaign
{
    [HarmonyPatch(typeof(SkillVM))]
    class SkillVMPatch
    {
        [HarmonyPatch("InitializeValues")]
        [HarmonyPostfix]
        public static void InitializeValues_Postfix(SkillVM __instance)
        {
            CharacterVM characterVM = (CharacterVM)Traverse.Create(__instance)
                .Field("_developerVM")
                .GetValue();
            Hero hero = characterVM.Hero;
            bool value = hero.PartyBelongedTo != null
                    && (hero.PartyBelongedTo == MobileParty.MainParty
                        || hero.Clan == Clan.PlayerClan);
            Traverse.Create(__instance)
                .Field("_isInSamePartyAsPlayer")
                .SetValue(value);
        }

        static internal bool Prepare()
        {
            return Options.Campaign.EditClanMembers.Group.Enabled;
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.EditClanMembers.Group.Enabled = true;
        }
    }

    [HarmonyPatch(typeof(CharacterAttributeItemVM))]
    class CharacterAttributeItemVMPatch
    {
        [HarmonyPatch(MethodType.Constructor,
            new Type[] {
                typeof(Hero),
                typeof(CharacterAttributesEnum),
                typeof(CharacterVM),
                typeof(Action<CharacterAttributeItemVM>),
                typeof(Action<CharacterAttributeItemVM>) })]
        [HarmonyPostfix]
        public static void Constructor_Postfix(CharacterAttributeItemVM __instance)
        {
            CharacterVM characterVM = (CharacterVM)Traverse.Create(__instance)
                .Field("_characterVM")
                .GetValue();
            Hero hero = characterVM.Hero;
            Traverse.Create(__instance)
                .Field("_isInSamePartyAsPlayer")
                .SetValue(
                    hero.PartyBelongedTo != null
                    && (hero.PartyBelongedTo == MobileParty.MainParty
                        || hero.Clan == Clan.PlayerClan));
        }

        static internal bool Prepare()
        {
            return Options.Campaign.EditClanMembers.Group.Enabled;
        }
    }

    [HarmonyPatch(typeof(CharacterVM))]
    class CharacterVMPatch
    {
        [HarmonyPatch("CanAddFocusToSkillWithFocusAmount")]
        [HarmonyPrefix]
        public static bool CanAddFocusToSkillWithFocusAmount_Prefix(CharacterVM __instance, ref bool __result, int currentFocusAmount)
        {
            __result = currentFocusAmount < 5 && __instance.UnspentCharacterPoints > 0;
            return false;
        }

        static internal bool Prepare()
        {
            return Options.Campaign.EditClanMembers.Group.Enabled;
        }
    }
}
