using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Extension.Resources
{
    sealed class ResourceManager
    {
        string ModuleId => Module.ModuleId;
        SpriteData SpriteData = null;

        public static ResourceManager Instance => Singleton.Instance;

        public void Load()
        {
            UIResourceManager.UIResourceDepot.CollectResources();
            LoadSprites();
            UIResourceManager.BrushFactory.Initialize();
            UIResourceManager.WidgetFactory.CheckForUpdates();
        }

        public void Unload()
        {
            // TODO implement proper unload
        }

        public void Refresh()
        {
            UIResourceManager.UIResourceDepot.CollectResources();
            UnloadSprites();
            LoadSprites();
            UIResourceManager.BrushFactory.Initialize();
            UIResourceManager.WidgetFactory.CheckForUpdates();
        }

        void LoadSprites()
        {
            if (SpriteData == null)
            {
                ResourceDepot resourceDepot = new ResourceDepot(BasePath.Name);
                resourceDepot.AddLocation($"Modules/{ModuleId}/GUI/GauntletUI/");
                resourceDepot.CollectResources();
                SpriteData = new SpriteData("spriteData");
                SpriteData.Load(resourceDepot);
                foreach (KeyValuePair<string, SpriteCategory> cat in SpriteData.SpriteCategories)
                {
                    new Traverse(cat.Value).Property<SpriteData>("SpriteData").Value = UIResourceManager.SpriteData;
                    UIResourceManager.SpriteData.SpriteCategories.Add(cat.Key, cat.Value);
                }
                foreach (KeyValuePair<string, Sprite> name in SpriteData.SpriteNames)
                {
                    UIResourceManager.SpriteData.SpriteNames.Add(name.Key, name.Value);
                }
                foreach (KeyValuePair<string, SpritePart> part in SpriteData.SpritePartNames)
                {
                    UIResourceManager.SpriteData.SpritePartNames.Add(part.Key, part.Value);
                }
                UIResourceManager.SpriteData.SpriteCategories[ModuleId].Load(new TwoDimensionEngineResourceContext(), resourceDepot);
            }
        }

        void UnloadSprites()
        {
            if (SpriteData != null)
            {
                UIResourceManager.SpriteData.SpriteCategories[ModuleId].Unload();
                foreach (KeyValuePair<string, SpriteCategory> cat in SpriteData.SpriteCategories)
                {
                    UIResourceManager.SpriteData.SpriteCategories.Remove(cat.Key);
                }
                foreach (KeyValuePair<string, Sprite> name in SpriteData.SpriteNames)
                {
                    UIResourceManager.SpriteData.SpriteNames.Remove(name.Key);
                }
                foreach (KeyValuePair<string, SpritePart> part in SpriteData.SpritePartNames)
                {
                    UIResourceManager.SpriteData.SpritePartNames.Remove(part.Key);
                }
                SpriteData = null;
            }
        }

        ResourceManager()
        {
        }

        private class Singleton
        {
            static Singleton()
            {
            }

            internal static readonly ResourceManager Instance = new ResourceManager();
        }
    }
}
