using System.IO;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Extension.Resources
{
    class TwoDimensionEngineResourceContext : ITwoDimensionResourceContext
    {
        Texture ITwoDimensionResourceContext.LoadTexture(ResourceDepot resourceDepot, string name)
        {
            string fullPath = resourceDepot.GetFilePath($"{name}.png");
            return new Texture(new EngineTexture(LoadTextureFromPath(fullPath)));
        }

        TaleWorlds.Engine.Texture LoadTextureFromPath(string fullPath)
        {
            return TaleWorlds.Engine.Texture.LoadTextureFromPath(Path.GetFileName(fullPath),
                                                                 Path.GetDirectoryName(fullPath));
        }
    }
}
