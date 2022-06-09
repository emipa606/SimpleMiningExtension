using System.Collections.Generic;
using Verse;

namespace oreprocessing;

[StaticConstructorOnStartup]
public static class StaticConstructorClass
{
    static StaticConstructorClass()
    {
        foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (!thingDef.IsWorkTable)
            {
                continue;
            }

            var thingDefExtension = ThingDefExtension.Get(thingDef);
            if (thingDefExtension.inheritRecipesFrom == null)
            {
                continue;
            }

            var list = new List<RecipeDef>(thingDef.AllRecipes);
            NonPublicFields.ThingDef_allRecipesCached.SetValue(thingDef, null);
            foreach (var thingDef2 in thingDefExtension.inheritRecipesFrom)
            {
                foreach (var recipeDef in thingDef2.AllRecipes)
                {
                    if (!thingDefExtension.Allows(recipeDef))
                    {
                        continue;
                    }

                    if (thingDef.recipes == null)
                    {
                        thingDef.recipes = new List<RecipeDef>();
                    }

                    if (!list.Contains(recipeDef))
                    {
                        thingDef.recipes.Add(recipeDef);
                    }
                }
            }
        }
    }
}