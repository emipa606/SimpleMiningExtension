using System.Collections.Generic;
using Verse;

namespace oreprocessing;

public class ThingDefExtension : DefModExtension
{
    private static readonly ThingDefExtension defaultValues = new();

    public ThingFilter allowedProductFilter;

    public List<RecipeDef> allowedRecipes;

    public ThingFilter disallowedProductFilter;

    public List<RecipeDef> disallowedRecipes;

    public List<ThingDef> inheritRecipesFrom;

    public static ThingDefExtension Get(Def def)
    {
        return def.GetModExtension<ThingDefExtension>() ?? defaultValues;
    }

    public override IEnumerable<string> ConfigErrors()
    {
        if (inheritRecipesFrom == null)
        {
            yield return "inheritRecipesFrom is null.";
            yield break;
        }

        var nonWorkbenchDefNames = new List<string>();
        var recipeLessThingDefNames = new List<string>();
        foreach (var thing in inheritRecipesFrom)
        {
            if (!thing.IsWorkTable)
            {
                nonWorkbenchDefNames.Add(thing.defName);
            }
            else if (thing.AllRecipes.NullOrEmpty())
            {
                recipeLessThingDefNames.Add(thing.defName);
            }
        }

        if (nonWorkbenchDefNames.Any())
        {
            yield return
                $"the following ThingDefs in inheritRecipesFrom are not workbenches: {nonWorkbenchDefNames.ToCommaList()}";
        }

        if (recipeLessThingDefNames.Any())
        {
            yield return
                $"the following workbenches in inheritRecipesFrom do not have any recipes: {recipeLessThingDefNames.ToCommaList()}";
        }
    }

    public bool Allows(RecipeDef recipe)
    {
        var producedThingDef = recipe.ProducedThingDef;
        return (producedThingDef == null ||
                (allowedProductFilter == null || allowedProductFilter.Allows(producedThingDef)) &&
                (disallowedProductFilter == null || !disallowedProductFilter.Allows(producedThingDef))) &&
               (allowedRecipes == null || allowedRecipes.Contains(recipe)) &&
               (disallowedRecipes == null || !disallowedRecipes.Contains(recipe));
    }
}