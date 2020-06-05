using Extension.Config;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace Extension.Features.Sandbox
{
    class ClanTierModelExt : DefaultClanTierModel
    {
        int ClanTierMaximum => Options.Sandbox.Clan.ClanTierModel.ClanTierMaximum.Value;
        int CompanionLimitPerTier => Options.Sandbox.Clan.ClanTierModel.CompanionLimitPerTier.Value;
        int PartyLimitPerTier => Options.Sandbox.Clan.ClanTierModel.PartyLimitPerTier.Value;

        public override int MaxClanTier => ClanTierMaximum;

        public override int GetPartyLimitForTier(Clan clan, int clanTierToCheck)
        {
            return (clanTierToCheck + 1) * PartyLimitPerTier;
        }

        public override int GetCompanionLimitForTier(int clanTier)
        {
            return (clanTier + 1) * CompanionLimitPerTier;
        }

        public override int CalculateInitialRenown(Clan clan)
        {
            int tierLimit = TierRenownLimits[clan.Tier];
            int nextTierLimit = (clan.Tier >= MaxClanTier) ? (TierRenownLimits[MaxClanTier] + 1500) : TierRenownLimits[clan.Tier + 1];
            int maxValue = (int)(nextTierLimit - (nextTierLimit - tierLimit) * 0.4f);
            return MBRandom.RandomInt(tierLimit, maxValue);
        }

        public override int CalculateTier(Clan clan)
        {
            int result = MinClanTier;
            for (int i = MinClanTier + 1; i <= MaxClanTier; i++)
            {
                if (clan.Renown >= TierRenownLimits[i])
                {
                    result = i;
                }
            }
            return result;
        }

        public override int GetRequiredRenownForTier(int tier)
        {
            return TierRenownLimits[tier];
        }

        static readonly int[] TierRenownLimits = new int[]
        {
            0,
            50,
            150,
            350,
            900,
            2350,
            6150,
            15000,
            30000,
            45000,
            60000
        };

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.Clan.ClanTierModel.ClanTierMaximum.Set(
                value: 6,
                defaultValue: 6,
                min: 4,
                max: 10);
            Options.Sandbox.Clan.ClanTierModel.CompanionLimitPerTier.Set(
                value: 2,
                defaultValue: 2,
                min: 1,
                max: 5);
            Options.Sandbox.Clan.ClanTierModel.PartyLimitPerTier.Set(
                value: 2,
                defaultValue: 2,
                min: 1,
                max: 5);
            Options.Sandbox.Clan.Group.Classes.Add(typeof(ClanTierModelExt));
        }
    }
}
