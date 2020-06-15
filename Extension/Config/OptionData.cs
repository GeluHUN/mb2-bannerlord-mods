using Extension.Utils;

namespace Extension.Config
{
    public static class Options
    {
        public static class Campaign
        {
            public static string Id => "campaign";
            public static Category Category => Configuration.Instance[Id];

            public static class DonationToNotables
            {
                public static string Id => "donationtonotables";
                public static Group Group => Category[Id] as Group;

                static DonationToNotables()
                {
                    Group.Create(Id, Category,
                        name: "Gifts to notables",
                        hint: "If enabled the player can give gold gifts to notables to be able to increase their relation with the player.");
                }
            }

            public static class Rebellions
            {
                public static string Id => "rebellions";
                public static Group Group => Category[Id] as Group;

                static Rebellions()
                {
                    Group.Create(Id, Category,
                        name: "Enable rebellions",
                        hint: new RichtextBuilder()
                            .Append("If enabled towns can rebel up against their owner if their loyalty gets below 25.")
                            .AppendDoubleLine()
                            .Append("It is disabled in the original game.")
                            .ToString());
                }
            }

            public static class StoryModeGenericXpModel
            {
                public static string Id => "trainingfieldgivesxp";
                public static Group Group => Category[Id] as Group;

                static StoryModeGenericXpModel()
                {
                    Group.Create(Id, Category,
                        name: "Traning field gives xp",
                        hint: new RichtextBuilder()
                            .Append("If true, the using the training field for practice gives skill experiance.")
                            .AppendLine()
                            .Append("It is disabled in the original game.")
                            .ToString());
                }
            }

            public static class WanderersAreNice
            {
                public static string Id => "wanderersarealwaysnice";
                public static Group Group => Category[Id] as Group;

                static WanderersAreNice()
                {
                    Group.Create(Id, Category,
                        name: "Wanderers are nice",
                        hint: new RichtextBuilder()
                            .Append("If enabled every wanderer will be always nice and strong:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Traits (honor, valor, mercy etc.) will be always positive")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Attributes (vigor, control etc.) will never be less then 3")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class Persuasion
            {
                public static string Id => "persuasion";
                public static Group Group => Category[Id] as Group;

                static Persuasion()
                {
                    Group.Create(Id, Category,
                        name: "Persuasion",
                        hint: "If enabled then player persuasion attempts are always succesful");
                }
            }

            public static class RoyalArmory
            {
                public static string Id => "royalarmory";
                public static Group Group => Category[Id] as Group;

                static RoyalArmory()
                {
                    Group.Create(Id, Category,
                        name: "Royal armory in towns",
                        hint: "If enabled then player can buy high tier armor in towns.");
                }
            }

            public static class PartyOrders
            {
                public static string Id => "partyorders";
                public static Group Group => Category[Id] as Group;

                static PartyOrders()
                {
                    Group.Create(Id, Category,
                        name: "Party orders",
                        hint: "If enabled then player has more control over the clan's parties.");
                }
            }

            public static class EditClanMembers
            {
                public static string Id => "editclanmembers";
                public static Group Group => Category[Id] as Group;

                static EditClanMembers()
                {
                    Group.Create(Id, Category,
                        name: "Edit clan members",
                        hint: "If enabled then player can edit clan member attributes even when not in the same party.");
                }
            }

            public static class Bandits
            {
                public static string Id => "bandits";
                public static Group Group => Category[Id] as Group;

                public static class KillingBanditsIncreaseRelations
                {
                    public static string SettlementDistanceId => "settlementdistance";
                    public static IntOption SettlementDistance => Group[SettlementDistanceId] as IntOption;

                    public static string LooterIncreaseRelationId => "looterincreaserelation";
                    public static FloatOption LooterIncreaseRelation => Group[LooterIncreaseRelationId] as FloatOption;

                    public static string BanditIncreaseRelationId => "banditincreaserelation";
                    public static FloatOption BanditIncreaseRelation => Group[BanditIncreaseRelationId] as FloatOption;

                    static KillingBanditsIncreaseRelations()
                    {
                        IntOption.Create(SettlementDistanceId, Group,
                            name: "Settlement maximum distance",
                            hint: "Sets the maximum distance a settlement's notables will give relation increase from the kills to the player.");
                        FloatOption.Create(LooterIncreaseRelationId, Group,
                            name: "Looter relation increase",
                            hint: "Defeating one looter will increase relations to notables by this number.");
                        FloatOption.Create(BanditIncreaseRelationId, Group,
                            name: "Bandit relation increase",
                            hint: "Defeating one bandit will increase relations to notables by this number.");
                    }
                }

                public static class TroopCountLimit
                {
                    public static string HideoutBattlePlayerMaxTroopCountId => "hideoutbattleplayermaxtroopcount";
                    public static IntOption HideoutBattlePlayerMaxTroopCount => Group[HideoutBattlePlayerMaxTroopCountId] as IntOption;

                    static TroopCountLimit()
                    {
                        IntOption.Create(HideoutBattlePlayerMaxTroopCountId, Group,
                            name: "Hideout assault troops",
                            hint: new RichtextBuilder()
                                .Append("The maximum troops the player can bring to a hideout encounter.")
                                .AppendDoubleLine()
                                .Append("Original size is 8.")
                                .ToString());
                    }
                }

                static Bandits()
                {
                    Group.Create(Id, Category,
                        name: "Bandit and Looters",
                        hint: new RichtextBuilder()
                            .Append("Changes Bandit and Looter mechanics:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Defeating bandist increases relations with nearby notables")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Set the maximum number of troops for hideout encounters")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class TroopTraining
            {
                public static string Id => "trooptraining";
                public static Group Group => Category[Id] as Group;

                public static string TrainSessionLengthId => "trainsessionlength";
                public static IntOption TrainSessionLength => Group[TrainSessionLengthId] as IntOption;

                public static string RestLengthId => "restlength";
                public static IntOption RestLength => Group[RestLengthId] as IntOption;

                public static string HighestTrainableTroopTierId => "highesttrainabletrooptier";
                public static IntOption HighestTrainableTroopTier => Group[HighestTrainableTroopTierId] as IntOption;

                public static string TroopTierGoldCostId => "trooptiergoldcost";
                public static IntOption TroopTierGoldCost => Group[TroopTierGoldCostId] as IntOption;

                public static string FieldUsageFeePercantageId => "fieldusagefeepercantage";
                public static FloatOption FieldUsageFeePercantage => Group[FieldUsageFeePercantageId] as FloatOption;

                public static string ExperienceIncreasePerHourId => "experienceincreaseperhour";
                public static IntOption ExperienceIncreasePerHour => Group[ExperienceIncreasePerHourId] as IntOption;

                public static string MaxWoundChanceId => "maxwoundchance";
                public static PercentOption MaxWoundChance => Group[MaxWoundChanceId] as PercentOption;

                static TroopTraining()
                {
                    Group.Create(Id, Category,
                        name: "Troop training",
                        hint: "If enabled then the player can train their troops at the training field for gold.");
                    IntOption.Create(TrainSessionLengthId, Group,
                        name: "Session length",
                        hint: "A full train session is this many hours long.");
                    IntOption.Create(RestLengthId, Group,
                        name: "Rest length",
                        hint: "After training the player can not train again for this many hours.");
                    IntOption.Create(HighestTrainableTroopTierId, Group,
                        name: "Highest trainable",
                        hint: "Highest trainable troop tier.");
                    IntOption.Create(TroopTierGoldCostId, Group,
                        name: "Troop tier cost",
                        hint: "Each troop costs this much gold per troop level.");
                    FloatOption.Create(FieldUsageFeePercantageId, Group,
                        name: "Field cost",
                        hint: "Field usage cost percentage.");
                    IntOption.Create(ExperienceIncreasePerHourId, Group,
                        name: "Experience",
                        hint: "Each hour while training each troop gets this many experience.");
                    PercentOption.Create(MaxWoundChanceId, Group,
                        name: "Wound chance",
                        hint: "Wound chance for tier 0 troops every hour. Higher tiers decresed this a bit by each tier.");
                }
            }

            public static class SkillTraining
            {
                public static string Id => "skilltraining";
                public static Group Group => Category[Id] as Group;

                public static string TrainSessionLengthId => "trainsessionlength";
                public static IntOption TrainSessionLength => Group[TrainSessionLengthId] as IntOption;

                public static string RestLengthId => "restlength";
                public static IntOption RestLength => Group[RestLengthId] as IntOption;

                public static string SkillLevelGoldCostId => "skilllevelgoldcost";
                public static IntOption SkillLevelGoldCost => Group[SkillLevelGoldCostId] as IntOption;

                public static string HighestTrainableSkillLevelId => "highesttrainableskilllevel";
                public static IntOption HighestTrainableSkillLevel => Group[HighestTrainableSkillLevelId] as IntOption;

                public static string ExperienceIncreasePerHourId => "experienceincreaseperhour";
                public static IntOption ExperienceIncreasePerHour => Group[ExperienceIncreasePerHourId] as IntOption;

                static SkillTraining()
                {
                    Group.Create(Id, Category,
                        name: "Skill training",
                        hint: "If enabled then the player can train the main hero's skill at towns for gold.");
                    IntOption.Create(TrainSessionLengthId, Group,
                        name: "Session length",
                        hint: "A full train session is this many hours long.");
                    IntOption.Create(RestLengthId, Group,
                        name: "Rest length",
                        hint: "After training the player can not train again for this many hours.");
                    IntOption.Create(SkillLevelGoldCostId, Group,
                        name: "Gold cost",
                        hint: "Each skill level costs this much gold to level up.");
                    IntOption.Create(HighestTrainableSkillLevelId, Group,
                        name: "Highest trainable",
                        hint: "Highest trainable skill level.");
                    IntOption.Create(ExperienceIncreasePerHourId, Group,
                        name: "Experience",
                        hint: "Each hour while training the hero's skill gets this many experience.");
                }
            }

            public static class StrongerHeroAtStart
            {
                public static string Id => "strongerheroatstart";
                public static Group Group => Category[Id] as Group;

                static StrongerHeroAtStart()
                {
                    Group.Create(Id, Category,
                        name: "Strgoner hero at start",
                        hint: "If enabled then the player hero starts a new game with pre-setup skills. Note, that this also causes the hero to level up at start acordingly.");
                }
            }

            static Campaign()
            {
                Category.Create(Id,
                    name: "Campaign",
                    hint: "Changes effecting the player's Hero, campaign, stories and quests");
            }
        }

        public static class Battles
        {
            public static string Id => "battles";
            public static Category Category => Configuration.Instance[Id];

            public static class BattleParams
            {
                public static string Id => "battleparams";
                public static Group Group => Category[Id] as Group;

                public static class BiggerBattleSize
                {
                    public static string MaximumBattleSizeId => "maximumbattlesize";
                    public static IntOption MaximumBattleSize => Group[MaximumBattleSizeId] as IntOption;

                    static BiggerBattleSize()
                    {
                        IntOption.Create(MaximumBattleSizeId, Group,
                            name: "Battle size",
                            hint: new RichtextBuilder()
                                .StartStyle("Red")
                                .Append("Currently not working, causes a crash when above 1000")
                                .EndStyle()
                                .AppendDoubleLine()
                                .Append("Set the maximum number of troops that can participate in a tactical battle.")
                                .AppendDoubleLine()
                                .Append("Default original game value is 500, maximum is 1000.")
                                .AppendDoubleLine()
                                .Append("Note, that the original game settings value won't reflect the changes in the options menu.")
                                .ToString());
                    }
                }

                public static class HumanSize
                {
                    public static string BodyCapsuleRadiusId => "bodycapsuleradius";
                    public static FloatOption BodyCapsuleRadius => Group[BodyCapsuleRadiusId] as FloatOption;

                    static HumanSize()
                    {
                        FloatOption.Create(BodyCapsuleRadiusId, Group,
                            name: "Human capsule",
                            hint: new RichtextBuilder()
                                .Append("Size of a human body collision capsule in tactical battles.")
                                .AppendDoubleLine()
                                .Append("Original game value is 0.37.")
                                .ToString());
                    }
                }

                public static class BattleAIParameters
                {
                    public static string DefensivenessId => "defensiveness";
                    public static FloatOption Defensiveness => Group[DefensivenessId] as FloatOption;

                    public static string EnableHoldTheShieldHighId => "enableholdtheshieldhigh";
                    public static BoolOption EnableHoldTheShieldHigh => Group[EnableHoldTheShieldHighId] as BoolOption;

                    public static string EnableOverheadShieldsId => "enableoverheadshields";
                    public static BoolOption EnableOverheadShields => Group[EnableOverheadShieldsId] as BoolOption;

                    static BattleAIParameters()
                    {
                        FloatOption.Create(DefensivenessId, Group,
                            name: "Defensiveness",
                            hint: new RichtextBuilder()
                                .Append("Individual troop defensiveness factor added to the original value. This effects:")
                                .AppendDoubleLine()
                                .StartStyle("Smaller")
                                .AppendBulletpointL1("Use of shield against missiles (melee skill also effects this)")
                                .EndStyle()
                                .StartStyle("Smaller")
                                .AppendBulletpointL1("Chance to use the shield for defense against an attack (melee skill also effects this)")
                                .EndStyle()
                                .StartStyle("Smaller")
                                .AppendBulletpointL1("Attack after a parry")
                                .EndStyle()
                                .StartStyle("Smaller")
                                .AppendBulletpointL1("Do an attack")
                                .EndStyle()
                                .AppendDoubleLine()
                                .Append("Original game value is between 0 and 2, default is 2, but during charge and no formation it's 0.")
                                .ToString());
                        BoolOption.Create(EnableHoldTheShieldHighId, Group,
                            name: "Use the shield",
                            hint: new RichtextBuilder()
                                .StartStyle("Red")
                                .Append("Currently not working")
                                .EndStyle()
                                .AppendDoubleLine()
                                .Append("If enabled troops use the shield in these cases too:")
                                .AppendDoubleLine()
                                .StartStyle("Smaller")
                                .AppendBulletpointL1("When unit is detached from the formation")
                                .EndStyle()
                                .StartStyle("Smaller")
                                .AppendBulletpointL1("In loose and scatter formations")
                                .EndStyle()
                                .StartStyle("Smaller")
                                .AppendBulletpointL1("In the shield wall formation when not in the front hold the shield up")
                                .EndStyle()
                                .ToString());
                        BoolOption.Create(EnableOverheadShieldsId, Group,
                            name: "Shields can be held overhead",
                            hint: new RichtextBuilder()
                                .Append("If enabled every shield can be held overhead.")
                                .ToString());
                    }
                }

                static BattleParams()
                {
                    Group.Create(Id, Category,
                        name: "Tactical battle parameters",
                        hint: new RichtextBuilder()
                            .Append("Changes to battle parameters:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Battle size")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Human body collision size")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Shields can be held overhead")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Unit defensiveness")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Shield usage")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class Atmosphere
            {
                public static string Id => "atmosphere";
                public static Group Group => Category[Id] as Group;

                public static class CustomTimeOfDay
                {
                    public static string EnableTimeOfDayId => "enabletimeofday";
                    public static BoolOption EnableTimeOfDay => Group[EnableTimeOfDayId] as BoolOption;

                    public static string TimeOfDayId => "timeofday";
                    public static FloatOption TimeOfDay => Group[TimeOfDayId] as FloatOption;

                    static CustomTimeOfDay()
                    {
                        BoolOption.Create(EnableTimeOfDayId, Group,
                            name: "Use fixed time of day",
                            hint: "If enabled then every battle will be fought in the same time of day regardless of the actual campaign time.");
                        FloatOption.Create(TimeOfDayId, Group,
                            name: "Fixed time of day value",
                            hint: "Use the value as a fixed time of day for every battle.");
                    }
                }

                static Atmosphere()
                {
                    Group.Create(Id, Category,
                        name: "Atmosphere",
                        hint: new RichtextBuilder()
                            .Append("If enabled then overrides the game's dynamic time and weather coditions with the following fixed parameters:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Time of day")
                            .EndStyle()
                            .ToString());
                }
            }

            static Battles()
            {
                Category.Create(Id,
                    name: "Battles",
                    hint: "Changes effecting tactical (not simulated) battles");
            }
        }

        public static class Sandbox
        {
            public static string Id => "sandbox";
            public static Category Category => Configuration.Instance[Id];

            public static class Clan
            {
                public static string Id => "clan";
                public static Group Group => Category[Id] as Group;

                public static class ClanTierModel
                {
                    public static string ClanTierMaximumId => "clantiermaximum";
                    public static IntOption ClanTierMaximum => Group[ClanTierMaximumId] as IntOption;

                    public static string CompanionLimitPerTierId => "companionlimitpertier";
                    public static IntOption CompanionLimitPerTier => Group[CompanionLimitPerTierId] as IntOption;

                    public static string PartyLimitPerTierId => "partylimitpertier";
                    public static IntOption PartyLimitPerTier => Group[PartyLimitPerTierId] as IntOption;

                    static ClanTierModel()
                    {
                        IntOption.Create(ClanTierMaximumId, Group,
                            name: "Maximum clan tier",
                            hint: new RichtextBuilder()
                                .Append("Maximum clan tier, up to 10.")
                                .AppendDoubleLine()
                                .Append("Original size is 6.")
                                .AppendDoubleLine()
                                .StartStyle("Red")
                                .Append("Changing this value needs a totally new game start!")
                                .EndStyle()
                                .ToString());
                        IntOption.Create(CompanionLimitPerTierId, Group,
                            name: "Companion limit",
                            hint: new RichtextBuilder()
                                .Append("Companion limit per clan tier plus one.")
                                .AppendLine()
                                .StartStyle("Explanation")
                                .Append("Formula: (tier+1)*value")
                                .EndStyle()
                                .AppendDoubleLine()
                                .Append("Original game mechanic is tier plus 3.")
                                .ToString());
                        IntOption.Create(PartyLimitPerTierId, Group,
                            name: "Party limit",
                            hint: new RichtextBuilder()
                                .Append("Party limit per clan tier plus one.")
                                .AppendLine()
                                .StartStyle("Explanation")
                                .Append("Formula: (tier+1)*value")
                                .EndStyle()
                                .AppendDoubleLine()
                                .Append("Original game mechanic is complicated.")
                                .ToString());
                    }
                }

                public static class WorkshopModel
                {
                    public static string WorkshopLimitPerTierId => "workshoplimitpertier";
                    public static IntOption WorkshopLimitPerTier => Group[WorkshopLimitPerTierId] as IntOption;

                    static WorkshopModel()
                    {
                        IntOption.Create(WorkshopLimitPerTierId, Group,
                            name: "Workshop limit",
                            hint: new RichtextBuilder()
                                .Append("Workshop limit per clan tier plus one.")
                                .AppendLine()
                                .StartStyle("Explanation")
                                .Append("Formula: (tier+1)*value")
                                .EndStyle()
                                .AppendDoubleLine()
                                .Append("Original game mechanic is tier plus 1.")
                                .ToString());
                    }
                }

                static Clan()
                {
                    Group.Create(Id, Category,
                        name: "Clan",
                        hint: new RichtextBuilder()
                            .Append("Changes to the clan mechanics:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Clan maximum tier")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Clan resources (party, companions and workshops) per tier")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class Prosperity
            {
                public static string Id => "prosperity";
                public static Group Group => Category[Id] as Group;

                public static class SettlementProsperityModel
                {
                    public static string VillageImmigrationRangeId => "villageimmigrationrange";
                    public static IntOption VillageImmigrationRange => Group[VillageImmigrationRangeId] as IntOption;

                    public static string FoodShortageBaseId => "foodshortagebase";
                    public static FloatOption FoodShortageBase => Group[FoodShortageBaseId] as FloatOption;

                    public static string UnderSiegeBaseId => "undersiegebase";
                    public static FloatOption UnderSiegeBase => Group[UnderSiegeBaseId] as FloatOption;

                    public static string StarvingBaseId => "starvingbase";
                    public static FloatOption StarvingBase => Group[StarvingBaseId] as FloatOption;

                    public static string LowFoodBaseId => "lowfoodbase";
                    public static FloatOption LowFoodBase => Group[LowFoodBaseId] as FloatOption;

                    public static string FoodAbundanceBaseId => "foodabundancebase";
                    public static FloatOption FoodAbundanceBase => Group[FoodAbundanceBaseId] as FloatOption;

                    public static string FoodExcessBaseId => "foodexcessbase";
                    public static FloatOption FoodExcessBase => Group[FoodExcessBaseId] as FloatOption;

                    static SettlementProsperityModel()
                    {
                        IntOption.Create(VillageImmigrationRangeId, Group,
                            name: "Village immigration range",
                            hint: "The range of setllements to get immigrants from.");
                        FloatOption.Create(UnderSiegeBaseId, Group,
                            name: "Under siege",
                            hint: new RichtextBuilder()
                                .Append("If town is under siege then prosperity is decreased with this value multiplied by the prosperity level plus one.")
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Formula: -value * (properity level + 1)")
                                .EndStyle()
                                .ToString());
                        FloatOption.Create(StarvingBaseId, Group,
                            name: "Starving",
                            hint: new RichtextBuilder()
                                .Append("If town is starving then prosperity is decreased with this value multiplied by the prosperity level plus one.")
                                .AppendLine()
                                .Append("Note, that the original game already decreases the prosperity by food change / 2 when the town is starving.")
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Starving: food stock < 0 and food change < 0")
                                .EndStyle()
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Formula: -value * (properity level + 1)")
                                .EndStyle()
                                .ToString());
                        FloatOption.Create(FoodShortageBaseId, Group,
                            name: "Food shortage",
                            hint: new RichtextBuilder()
                                .Append("If town has food shortage then prosperity is decreased with this value multiplied by the prosperity level plus one.")
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Food shortage: food stock > 200 and food change < 0")
                                .EndStyle()
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Formula: value * (-properity level + food change / 5)")
                                .EndStyle()
                                .ToString());
                        FloatOption.Create(LowFoodBaseId, Group,
                            name: "Low food",
                            hint: new RichtextBuilder()
                                .Append("If town is low on food then prosperity is decreased with this value multiplied by the prosperity level plus one.")
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Low food: food stock < 200 and food change < 0")
                                .EndStyle()
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Formula: value * (-properity level + food change / 10)")
                                .EndStyle()
                                .ToString());
                        FloatOption.Create(FoodExcessBaseId, Group,
                            name: "Food excess",
                            hint: new RichtextBuilder()
                                .Append("If town has excess food then prosperity is increased with this value multiplied by the prosperity level plus one.")
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Excess food: food stock > 200 and food change > 10")
                                .EndStyle()
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Formula: value * (properity level + food change / 10)")
                                .EndStyle()
                                .ToString());
                        FloatOption.Create(FoodAbundanceBaseId, Group,
                            name: "Food abundance",
                            hint: new RichtextBuilder()
                                .Append("If town has abundant food then prosperity is increased with this value multiplied by the prosperity level plus one.")
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Abundant food: food stock > 600 and food change > 20")
                                .EndStyle()
                                .AppendDoubleLine()
                                .StartStyle("Explanation")
                                .Append("Formula: value * (properity level + food change / 5)")
                                .EndStyle()
                                .ToString());
                    }
                }

                static Prosperity()
                {
                    Group.Create(Id, Category,
                        name: "Settlement prosperity",
                        hint: new RichtextBuilder()
                            .Append("Modifies the settlement prosperity mechanics:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("When towns starve or under siege there is no prosperity increase")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Towns react to the amount of food stock and food change with bigger prosperity changes")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Villages get immigration bonus from nearby towns and castles")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class SimulatedBattle
            {
                public static string Id => "simulatedbattle";
                public static Group Group => Category[Id] as Group;

                public static class CombatSimulationModel
                {
                    public static string MountsDontCountInSiegeId => "mountsdontcountinsiege";
                    public static BoolOption MountsDontCountInSiege => Group[MountsDontCountInSiegeId] as BoolOption;

                    public static string UnbreakableDefensesId => "unbreakabledefenses";
                    public static BoolOption UnbreakableDefenses => Group[UnbreakableDefensesId] as BoolOption;

                    static CombatSimulationModel()
                    {
                        BoolOption.Create(MountsDontCountInSiegeId, Group,
                            name: "Mounts don't count in sieges",
                            hint: new RichtextBuilder()
                                .Append("If enabled mounted troops don't get an advantage in siege battles.")
                                .AppendDoubleLine()
                                .Append("In original game mounted troops always get 20% advantage.")
                                .ToString());
                        BoolOption.Create(UnbreakableDefensesId, Group,
                            name: "Unbrakeable defenses",
                            hint: "If enabled siege or raid defenders always win in simulated battles, except the player.");
                    }
                }

                static SimulatedBattle()
                {
                    Group.Create(Id, Category,
                        name: "Simulated battles",
                        hint: new RichtextBuilder()
                            .Append("Changes the simulated battle mechanics:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Faction attack and defense advantages")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class SurvivalChances
            {
                public static string Id => "survivalchances";
                public static Group Group => Category[Id] as Group;

                public static class PartyHealingModel
                {
                    public static string PlayerTroopsCantDieId => "playertroopscantdie";
                    public static BoolOption PlayerTroopsCantDie => Group[PlayerTroopsCantDieId] as BoolOption;

                    public static string LootersCantKillId => "looterscantkill";
                    public static BoolOption LootersCantKill => Group[LootersCantKillId] as BoolOption;

                    static PartyHealingModel()
                    {
                        BoolOption.Create(PlayerTroopsCantDieId, Group,
                            name: "Player troops can't die",
                            hint: "If enabled the player's troops never die, only get wounded in battles.");
                        BoolOption.Create(LootersCantKillId, Group,
                            name: "Looters can't kill",
                            hint: "If enabled looters can never kill, only wound soldiers.");
                    }
                }

                static SurvivalChances()
                {
                    Group.Create(Id, Category,
                        name: "Survival chances",
                        hint: new RichtextBuilder()
                            .Append("Changes if a soldier dies or only gets wounded in a battle:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Looters can't kill")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Palyer's soldiers can't die")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class BoostAI
            {
                public static string Id => "boostai";
                public static Group Group => Category[Id] as Group;

                public static class BoostLordTroops
                {
                    public static string MaximumTierToBoostId => "maximumtiertoboost";
                    public static IntOption MaximumTierToBoost => Group[MaximumTierToBoostId] as IntOption;

                    static BoostLordTroops()
                    {
                        IntOption.Create(MaximumTierToBoostId, Group,
                            name: "Maximum troop tier to boost",
                            hint: new RichtextBuilder()
                                .Append("Maximum tier of a troop that can get the passive experience boost.")
                                .AppendDoubleLine()
                                .Append("Note, that though the troops get enough experience each day to level up instantly, but it's up to the NPC Lords to actually level up their troops and pay the gold cost of it.")
                                .ToString());
                    }
                }

                public static class DonateFoodToLords
                {
                    public static string EnableDonateFoodToLordsId => "enabledonatefoodtolords";
                    public static BoolOption EnableDonateFoodToLords => Group[EnableDonateFoodToLordsId] as BoolOption;

                    static DonateFoodToLords()
                    {
                        BoolOption.Create(EnableDonateFoodToLordsId, Group,
                            name: "Donate food to starving lords",
                            hint: "If enabled then broke and starving NPC lords get 5 days worth of grain.");
                    }
                }

                public static class DonateGoldToTowns
                {
                    public static string EnableDonateDonateGoldToTownsId => "enabledonatedonategoldtotowns";
                    public static BoolOption EnableDonateDonateGoldToTowns => Group[EnableDonateDonateGoldToTownsId] as BoolOption;

                    static DonateGoldToTowns()
                    {
                        BoolOption.Create(EnableDonateDonateGoldToTownsId, Group,
                            name: "Town gold donation",
                            hint: "If enabled then towns below 5000 gold get as much gold to start the day with at least 5000 gold.");
                    }
                }

                static BoostAI()
                {
                    Group.Create(Id, Category,
                        name: "AI boost",
                        hint: new RichtextBuilder()
                            .Append("Boosts to the NPC lords in the following ways:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Troops get passive experience daily")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Broke and starving NPC lords get food supply")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class Militia
            {
                public static string Id => "militia";
                public static Group Group => Category[Id] as Group;

                public static class MoreMilitiaAtStart
                {
                    public static string StartingMilitiaMultiplierId => "startingmilitiamultiplier";
                    public static IntOption StartingMilitiaMultiplier => Group[StartingMilitiaMultiplierId] as IntOption;

                    static MoreMilitiaAtStart()
                    {
                        IntOption.Create(StartingMilitiaMultiplierId, Group,
                            name: "Starting militia multiplier",
                            hint: "At a new game start the starting militia will be value times stronger.");
                    }
                }

                public static class SettlementMilitiaModel
                {
                    public static string EliteTroopRateIncreaseId => "elitetrooprateincrease";
                    public static FloatOption EliteTroopRateIncrease => Group[EliteTroopRateIncreaseId] as FloatOption;

                    public static string WartimeRecruitmentRateId => "wartimerecruitmentrate";
                    public static FloatOption WartimeRecruitmentRate => Group[WartimeRecruitmentRateId] as FloatOption;

                    static SettlementMilitiaModel()
                    {
                        FloatOption.Create(EliteTroopRateIncreaseId, Group,
                            name: "Increased elite militia ratio",
                            hint: new RichtextBuilder()
                                .Append("Plus elite militia ratio.")
                                .AppendDoubleLine()
                                .Append("Original game value starts from 0.1 and increased by governor perks.")
                                .ToString());
                        FloatOption.Create(WartimeRecruitmentRateId, Group,
                            name: "Wartime recruitment rate",
                            hint: "If a faction is in war then militia recruitment rate is multiplied by this number for all owned settlements.");
                    }
                }

                static Militia()
                {
                    Group.Create(Id, Category,
                        name: "Militia",
                        hint: new RichtextBuilder()
                            .Append("Changes to militia:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("More militia at start")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Increased elite troop ratio")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Increased recruitment in wars")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class Prisoners
            {
                public static string Id => "prisoners";
                public static Group Group => Category[Id] as Group;

                public static class PrisonerRecruitment
                {
                    public static string ConformityChangePerHourId => "conformitychangeperhour";
                    public static IntOption ConformityChangePerHour => Group[ConformityChangePerHourId] as IntOption;

                    public static string DailyRecruitedPrisonersId => "dailyrecruitedprisoners";
                    public static FloatOption DailyRecruitedPrisoners => Group[DailyRecruitedPrisonersId] as FloatOption;

                    static PrisonerRecruitment()
                    {
                        IntOption.Create(ConformityChangePerHourId, Group,
                            name: "Conformity amount",
                            hint: new RichtextBuilder()
                                .Append("Amount of conformity gained every hour.")
                                .AppendDoubleLine()
                                .Append("Original game value is 1.")
                                .ToString());
                        FloatOption.Create(DailyRecruitedPrisonersId, Group,
                            name: "Daily recruited prisoners",
                            hint: "Multiplies the daily number of maximum recruitable prisoners.");
                    }
                }

                static Prisoners()
                {
                    Group.Create(Id, Category,
                        name: "Prisoners",
                        hint: new RichtextBuilder()
                            .Append("Changes some prisoner mechanics:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Increased conformity gain")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Increased daily recruitable prisoners")
                            .EndStyle()
                            .ToString());
                }
            }

            static Sandbox()
            {
                Category.Create(Id,
                    name: "Sandbox",
                    hint: "Changes effecting the single player sandbox behaviour");
            }
        }

        public static class QoL
        {
            public static string Id => "qol";
            public static Category Category => Configuration.Instance[Id];

            public static class CivilianSaddles
            {
                public static string Id => "civiliansaddles";
                public static Group Group => Category[Id] as Group;

                static CivilianSaddles()
                {
                    Group.Create(Id, Category,
                        name: "Civilian saddles",
                        hint: new RichtextBuilder()
                            .Append("Enable/disable civilian saddles in game.")
                            .AppendLine()
                            .Append("If enabled all cloth and leather saddles are usuable as civilian outfit.")
                            .ToString());
                }
            }

            public static class SavePartyFormation
            {
                public static string Id => "savepartyformation";
                public static Group Group => Category[Id] as Group;

                static SavePartyFormation()
                {
                    Group.Create(Id, Category,
                        name: "Save party formations",
                        hint: "If enabled the game saves the party formations.");
                }
            }

            public static class EnterTownWithHorse
            {
                public static string Id => "entertownwithhorse";
                public static Group Group => Category[Id] as Group;

                static EnterTownWithHorse()
                {
                    Group.Create(Id, Category,
                        name: "Enter town on horse",
                        hint: "If enabled the player will enter a town or village riding their civilian horse.");
                }
            }

            public static class InstantBattleWithBandits
            {
                public static string Id => "instantbattlewithbandits";
                public static Group Group => Category[Id] as Group;

                static InstantBattleWithBandits()
                {
                    Group.Create(Id, Category,
                        name: "Instant battle with bandits",
                        hint: new RichtextBuilder()
                            .Append("When enabled the player can instantly attack bandits and looters.")
                            .AppendLine()
                            .Append("The player can talk to the bandits through the menu if needed.")
                            .ToString());
                }
            }

            public static class FasterForward
            {
                public static string Id => "fasterforward";
                public static Group Group => Category[Id] as Group;

                public static int NumberOfSettings => 5;

                static FasterForward()
                {
                    Group.Create(Id, Category,
                        name: "Faster forward",
                        hint: "If enabled time forward speed can be changed.");
                    for (int i = 0; i < NumberOfSettings; i++)
                    {
                        IntOption.Create($"{Id}{i + 1}", Group,
                            name: $"Fast forward setting {i + 1}",
                            hint: $"Pressing CTRL+{i + 1} will set fast time forward to this value");
                    }
                }
            }

            public static class SelecHideoutTroops
            {
                public static string Id => "selechideouttroops";
                public static Group Group => Category[Id] as Group;

                static SelecHideoutTroops()
                {
                    Group.Create(Id, Category,
                        name: "Select hideout troops",
                        hint: "When enabled the player can select the bandit hideout assault team.");
                }
            }

            public static class Statistics
            {
                public static string Id => "statistics";
                public static Group Group => Category[Id] as Group;

                static Statistics()
                {
                    Group.Create(Id, Category,
                        name: "Statistics",
                        hint: "When enabled the game gathers statistical event data. Press CTRL+s to show or hide the overlay window.");
                }
            }

            public static class Alerts
            {
                public static string Id => "alerts";
                public static Group Group => Category[Id] as Group;

                static Alerts()
                {
                    Group.Create(Id, Category,
                        name: "Game alerts",
                        hint: new RichtextBuilder()
                            .Append("When enabled important events are displayed in the bottom right corner.")
                            .AppendLine()
                            .Append("Each event has a detailed tooltip, and can be clicked to jump to the event source.")
                            .AppendLine()
                            .Append("Pressing CTRL+A shows/hides the event overlay.")
                            .AppendLine()
                            .Append("The following events are displayed:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Faction town is starving (high priority)")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Faction town is sieged (high priority)")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Faction party was attacked (medium priority)")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Faction village is raided (medium priority)")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Faction party attacks something (low priority)")
                            .EndStyle()
                            .ToString());
                }
            }

            static QoL()
            {
                Category.Create(Id,
                    name: "Small things",
                    hint: "Quality of life smaller changes");
            }
        }

        public static class Encyclopedia
        {
            public static string Id => "encyclopedia";
            public static Category Category => Configuration.Instance[Id];

            public static class Settlement
            {
                public static string Id => "settlement";
                public static Group Group => Category[Id] as Group;

                static Settlement()
                {
                    Group.Create(Id, Category,
                        name: "Settlement",
                        hint: new RichtextBuilder()
                            .Append("Additions to the settlement Encyclopedia pages:")
                            .AppendDoubleLine()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Many more filter options")
                            .EndStyle()
                            .StartStyle("Smaller")
                            .AppendBulletpointL1("Detailed settlement economy information")
                            .EndStyle()
                            .ToString());
                }
            }

            public static class TalkToHero
            {
                public static string Id => "talktohero";
                public static Group Group => Category[Id] as Group;

                static TalkToHero()
                {
                    Group.Create(Id, Category,
                        name: "Talk to heros",
                        hint: "If enabled then the player can talk to heroes through the Encyclopedia hero page.");
                }
            }

            static Encyclopedia()
            {
                Category.Create(Id,
                    name: "Encyclopedia",
                    hint: "Changes and additions to the Encyclopedia");
            }
        }
    }
}
