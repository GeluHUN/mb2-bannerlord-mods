using Extension.Config;
using Extension.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace Extension.Features.Ecyclopedia
{
    class HeroVMExt : HeroViewModel
    {
        readonly Hero Hero;
        TalkToHero TalkToHero => Helper.GetCampaignBehavior<TalkToHero>();

        public HeroVMExt(Hero hero, StanceTypes stance)
            : base(stance)
        {
            Hero = hero;
        }

        public void ExecuteTalkTo()
        {
            TalkToHero.InitiateTalk(Hero);
            OnPropertyChanged("IsEnabled");
        }

        [DataSourceProperty]
        public bool IsVisible => TalkToHero.CanTalkTo(Hero);

        [DataSourceProperty]
        public bool IsEnabled => !TalkToHero.QueuedToTalk(Hero);
    }

    [HarmonyPatch(typeof(EncyclopediaHeroPageVM))]
    class EncyclopediaHeroPageVMPatch
    {
        [HarmonyPatch("Refresh")]
        [HarmonyPrefix]
        public static void Refresh_Prefix(EncyclopediaHeroPageVM __instance)
        {
            __instance.HeroCharacter = new HeroVMExt(__instance.Obj as Hero, CharacterViewModel.StanceTypes.EmphasizeFace);
        }

        static internal bool Prepare()
        {
            return Options.Encyclopedia.TalkToHero.Group.Enabled;
        }
    }
}
