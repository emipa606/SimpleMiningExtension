using System.Reflection;
using Verse;

namespace oreprocessing;

[StaticConstructorOnStartup]
public static class NonPublicFields
{
    public static readonly FieldInfo ThingDef_allRecipesCached =
        typeof(ThingDef).GetField("allRecipesCached", BindingFlags.Instance | BindingFlags.NonPublic);
}