# GitHub Copilot Instructions for Simple Mining Extension (Continued)

## Mod Overview and Purpose

**Mod Name:** Simple Mining Extension (Continued)  
**Author:** WolfgangPolska/ Nero the Fam  
**PackageId:** Mlie.SimpleMiningExtension  
**Display name:** Simple Mining Extension (Continued)

This mod is an update and continuation of the original "Nero the Fams" mod. It enhances the mining mechanics in RimWorld by introducing smelting processes for vanilla metals, adding new mines that can spawn infestations, and incorporating a feature for accidental mining incidents. Additionally, it introduces adobe bricks and a drying process for bricks, expanding the player's resource management options.

## Key Features and Systems

### Smelting System
- Vanilla metals (steel, gold, silver) require smelting from ores.
- Advanced smelting unlocks plasteel and uranium smelting at electric smelters.

### Mines
- New mine buildings that mimic vanilla drills but have infestation chances.
- Optional feature to disable mining accidents based on pawn's mining skill.
- Pawns gain a "Miners Hunger" hediff after mining, increasing hunger and reducing plant work speed and yield.

### Adobe Bricks
- New building "Brick Spot" for crafting adobe bricks.
- "Drying Racks" for drying bricks, which take approximately 3 in-game days to become ready.
- Bricks can dry on their own, influenced by temperature and humidity.

### Supported Mods
- **WD Fueled Heaters:** Accepts coal as fuel.
- **Coal Expanded:** Uses coal from this mod and removes SME's coal items from the game.

### New Buildings
- **Coal Generator:** Similar stats to the chemfuel generator.
- **Blast Furnace:** Similar to the electric smelter but uses coal.
- **Sieve:** Used to find ores, highlights them on map.
- **Fueled Kiln:** Tribal smelting solution.
- **Brick Spot:** Allows crafting of adobe bricks.

### Ores
- Added coal for fueling stoves, smithies, and coal-powered generators.
- Ore items for every vanilla metallic resource.

### Research
- **Mining:** Unlocks mines.
- **Electric Smelting:** Unlocks electric smelter.
- **Advanced Smelting:** Unlocks refining of plasteel and uranium at the electric smelter.

## Coding Patterns and Conventions

1. **Folder Structure**
   - **Source:**
     - Contains all C# source files.
   - **Defs:**
     - Contains all XML definition files.

2. **C# File Naming**
   - Use PascalCase for C# file names (e.g., `CompDryable.cs`).

3. **Namespace**
   - Use consistent namespaces for all classes (e.g., `SimpleMiningExtension`).

4. **Class Naming**
   - Use PascalCase for class names (e.g., `CompDryable`).

5. **Method Naming**
   - Use camelCase for method names (e.g., `CompInspectStringExtra`).

6. **Variable Naming**
   - Use camelCase for variable names (e.g., `dryProgress`).

7. **XML File Naming**
   - Use PascalCase for XML file names (e.g., `HeDiffDefs.xml`).

8. **XML Element Naming**
   - Use PascalCase for XML element names (e.g., `HediffDef`).

## XML Integration

XML files are crucial for defining game mechanics, research projects, jobs, and work givers. Follow these conventions:

1. **Def Files**
   - Place each def type in its respective folder (e.g., `HeDiffDefs.xml` for hediff definitions).

2. **Def Naming**
   - Use PascalCase for def names (e.g., `MinersHunger`).

3. **Def Attributes**
   - Use camelCase for attribute names (e.g., `label`, `description`).

4. **Def Values**
   - Use appropriate data types (e.g., integers, strings).

5. **Def Relationships**
   - Ensure that def relationships are correctly defined (e.g., `parentDef`).

## Harmony Patching

Harmony patches are used to modify vanilla RimWorld code without altering the original files. Follow these conventions:

1. **Patch File Naming**
   - Use PascalCase for patch file names (e.g., `Patch_MineShaft.cs`).

2. **Patch Method Naming**
   - Use camelCase for patch method names (e.g., `Prefix_MiningWorkDone`).

3. **Patch Prefix/Suffix**
   - Use `Prefix_`, `Postfix_`, or `Transpiler_` depending on the patch type.

4. **Patch Target**
   - Ensure that the patch target is correctly specified (e.g., `TargetMethod`).

5. **Patch Parameters**
   - Use appropriate parameters for the patch method.

6. **Patch Return Type**
   - Ensure that the return type matches the original method.

## Suggestions for Copilot

1. **C# File Templates**
   - Create templates for common C# file structures (e.g., `Comp` class, `Def` class).

2. **XML File Templates**
   - Create templates for common XML def structures (e.g., `HediffDef`, `JobDef`).

3. **Harmony Patch Templates**
   - Create templates for common Harmony patch structures (e.g., `Prefix`, `Postfix`).

4. **Code Snippets**
   - Create code snippets for common tasks (e.g., adding a new def, creating a new building).

5. **Documentation**
   - Provide documentation for key classes, methods, and XML elements.

6. **Example Patches**
   - Provide example Harmony patches for common modifications (e.g., modifying pawn behavior, changing building mechanics).

7. **Mod Integration**
   - Provide guidance on integrating with supported mods (e.g., Coal Expanded, WD Fueled Heaters).

8. **Debugging Tips**
   - Provide tips for debugging common issues (e.g., missing def errors, patch conflicts).

9. **Testing Strategies**
   - Provide strategies for testing new features and patches (e.g., unit tests, in-game testing).

10. **Community Feedback**
    - Encourage community feedback and contributions (e.g., GitHub issues, Discord discussions).

By following these guidelines, developers can contribute effectively to the Simple Mining Extension (Continued) mod and ensure consistency across the project.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

