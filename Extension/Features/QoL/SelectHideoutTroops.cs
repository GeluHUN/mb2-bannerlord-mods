using Extension.Config;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Extension.Utils;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace Extension.Features.QoL
{
    class SelecHideoutTroops : CampaignBehaviorExt
    {
        internal TroopRoster SelectedTroops = null;
        int HideoutBattlePlayerMaxTroopCount;

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                SelectedTroops = null;
            }
            dataStore.SyncData("SelecHideoutTroops_SelectedTroops", ref SelectedTroops);
        }

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            HideoutBattlePlayerMaxTroopCount = Helper.TheCampaign.Models.TroopCountLimitModel.GetHideoutBattlePlayerMaxTroopCount();
            AddGameMenus(starter);
        }

        void AddGameMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption("hideout_place", "select_troops",
                new RichtextBuilder()
                    .Append("Select troops (")
                    .AppendImg("Icons\\Party@2x")
                    .Append("{HIDEOUT_TROOPS_SELECTED}/{HIDEOUT_MAX_TROOPS})")
                    .ToString(),
                UpdateMenu,
                StartTransfer,
                false,
                3);
            starter.AddGameMenuOption("hideout_after_wait", "select_troops",
                new RichtextBuilder()
                    .Append("Select troops (")
                    .AppendImg("Icons\\Party@2x")
                    .Append("{HIDEOUT_TROOPS_SELECTED}/{HIDEOUT_MAX_TROOPS})")
                    .ToString(),
                UpdateMenu,
                StartTransfer,
                false,
                1);
        }

        bool UpdateMenu(MenuCallbackArgs menu)
        {
            UpdateSelectedTroops();
            int count = SelectedTroops != null ? SelectedTroops.TotalManCount : 1;
            MBTextManager.SetTextVariable("HIDEOUT_TROOPS_SELECTED", $"{count}");
            MBTextManager.SetTextVariable("HIDEOUT_MAX_TROOPS", $"{HideoutBattlePlayerMaxTroopCount}");
            return true;
        }

        void StartTransfer(MenuCallbackArgs menu)
        {
            PartyScreenLogic partyScreenLogic = CreateLogic();
            Traverse traverse = Traverse.Create(PartyScreenManager.Instance);
            traverse.Field("_serverPartyScreenLogic").SetValue(partyScreenLogic);
            traverse.Field("_currentMode").SetValue(PartyScreenMode.TroopsManage);
            PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
            partyState.InitializeLogic(partyScreenLogic);
            Game.Current.GameStateManager.PushState(partyState, 0);
        }

        void UpdateSelectedTroops()
        {
            if (SelectedTroops == null)
            {
                SelectedTroops = new TroopRoster();
                SelectedTroops.AddToCounts(MobileParty.MainParty.Leader, 1);
            }
            else
            {
                foreach (CharacterObject troop in SelectedTroops.Troops.ToList())
                {
                    int current = SelectedTroops.GetTroopCount(troop);
                    if (MobileParty.MainParty.MemberRoster.Contains(troop))
                    {
                        int max = MobileParty.MainParty.MemberRoster.GetTroopCount(troop);
                        if (current > max)
                        {
                            SelectedTroops.RemoveTroop(troop, current - max);
                        }
                    }
                    else
                    {
                        SelectedTroops.RemoveTroop(troop, current);
                    }
                }
                SelectedTroops.RemoveZeroCounts();
            }
        }

        PartyScreenLogic CreateLogic()
        {
            (TroopRoster leftSide, TroopRoster rightSide) = PrepareParties();
            PartyScreenLogic result = new PartyScreenLogic();
            result.SetShowProgressBar(true);
            result.Initialize(
                leftSide,
                new TroopRoster(),
                MobileParty.MainParty,
                false,
                new TextObject("Assault team", null),
                HideoutBattlePlayerMaxTroopCount,
                new TextObject("Select hideout assault", null));
            result.InitializeTrade(
                PartyScreenLogic.TransferState.Transferable,
                PartyScreenLogic.TransferState.NotTransferable,
                PartyScreenLogic.TransferState.NotTransferable);
            result.SetDoneConditionHandler(new PartyPresentationDoneButtonConditionDelegate(IsDoneActive));
            result.SetDoneHandler(new PartyPresentationDoneButtonDelegate(OnDone));
            result.MemberRosters[0] = leftSide;
            result.MemberRosters[1] = rightSide;
            return result;
        }

        (TroopRoster LeftSide, TroopRoster RightSide) PrepareParties()
        {
            UpdateSelectedTroops();
            TroopRoster leftSide = new TroopRoster
            {
                SelectedTroops
            };
            TroopRoster rightSide = new TroopRoster
            {
                MobileParty.MainParty.MemberRoster.ToFlattenedRoster()
            };
            foreach (TroopRosterElement element in leftSide)
            {
                if (rightSide.Contains(element.Character))
                {
                    rightSide.RemoveTroop(element.Character, element.Number);
                }
            }
            return (leftSide, rightSide);
        }

        Tuple<bool, string> IsDoneActive(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, int leftLimitNum, int rightLimitNum)
        {
            return new Tuple<bool, string>(
                leftMemberRoster.TotalManCount <= leftLimitNum,
                $"Maximum troops allowed is {leftLimitNum}");
        }

        bool OnDone(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, bool isForced, List<MobileParty> leftParties = null, List<MobileParty> rigthParties = null)
        {
            SelectedTroops.Clear();
            SelectedTroops.Add(leftMemberRoster);
            return true;
        }

        static internal void Initialize_Configuration()
        {
            Options.QoL.SelecHideoutTroops.Group.Classes.Add(typeof(SelecHideoutTroops));
        }
    }

    [HarmonyPatch(typeof(PartyGroupTroopSupplier))]
    class PartyGroupTroopSupplierPatch
    {
        [HarmonyPatch(MethodType.Constructor, new[] { typeof(MapEvent), typeof(BattleSideEnum), typeof(Dictionary<CharacterObject, int>) })]
        [HarmonyPrefix]
        static internal void PartyGroupTroopSupplier_Ctor(MapEvent mapEvent, BattleSideEnum side, ref Dictionary<CharacterObject, int> priorTroopsDictionary)
        {
            if (mapEvent.IsHideoutBattle && side == BattleSideEnum.Attacker)
            {
                priorTroopsDictionary = new Dictionary<CharacterObject, int>();
                TroopRoster selectedTroops = Helper.GetCampaignBehavior<SelecHideoutTroops>().SelectedTroops;
                if (selectedTroops != null)
                {
                    foreach (TroopRosterElement troop in selectedTroops)
                    {
                        priorTroopsDictionary.Add(troop.Character, troop.Number);
                    }
                }
            }
        }

        static internal bool Prepare()
        {
            return Options.QoL.SelecHideoutTroops.Group.Enabled;
        }
    }

    [HarmonyPatch(typeof(PartyVM))]
    class PartyVMPatch
    {
        [HarmonyPatch("RefreshValues")]
        [HarmonyPostfix]
        static internal void RefreshValues_Postfix(PartyVM __instance)
        {
            Traverse.Create(__instance)
                .Method("RefreshPartyInformation")
                .GetValue();
        }

        static internal bool Prepare()
        {
            return Options.QoL.SelecHideoutTroops.Group.Enabled;
        }
    }
}
