using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class EnumManager{

	public enum QuestionType { Add, Subtract, Multiply, Divide, Algebra, Compare, Fraction, TrueOrFalse };

    public enum Upgrades { Barrricade1, Barricade2, IceArrows1, IceArrows2A, IceArrows2B,
        FireArrows1, FireArrows2A, FireArrows2B, ShockArrows1, ShockArrows2A, ShockArrows2B, 
        Inventory1, Inventory2
    }

    public enum Loadout { IceArrowsA, IceArrowsB, FireArrowsA, FireArrowsB, ShockArrowsA, ShockArrowsB };

    public enum ActiveQuestionCategories { AddOrSubtract, MultiplyOrDivide, Algebra}

    public enum Controls { keyboard, vr, mouse};

    public enum GameState { MainMenu, LevelSelect, Wave, PotionShop, Intermath}

    public enum GameplayMode { Easy, Medium, Hard}

    public enum PotionState { shop, menu, inventory }

    public enum GemType { Penny, Nickel, Dime, Quarter, Dollar}

    public enum ActivationType { Free, Paid, Special }

    public static Dictionary<EnumManager.GemType, float> gemValues = new Dictionary<GemType, float>()
    {
        {GemType.Penny, .01f },
        {GemType.Nickel, .05f },
        {GemType.Dime, .1f },
        {GemType.Quarter, .25f },
        {GemType.Dollar, 1f }

    };

    public static Dictionary<Upgrades, Loadout> upgradeToLoadout = new Dictionary<Upgrades, Loadout>()
    {
        {Upgrades.IceArrows2A, Loadout.IceArrowsA },
        {Upgrades.IceArrows2B, Loadout.IceArrowsB },
        {Upgrades.FireArrows2A, Loadout.FireArrowsA },
        {Upgrades.FireArrows2B, Loadout.FireArrowsB },
        {Upgrades.ShockArrows2A, Loadout.ShockArrowsA },
        {Upgrades.ShockArrows2B, Loadout.ShockArrowsB }
    };

    public static Dictionary<string, int> sceneNameToLevelNumber = new Dictionary<string, int>()
    {
        {SceneManager.GetSceneByBuildIndex(2).name, 0},
        {SceneManager.GetSceneByBuildIndex(3).name, 1},
        {SceneManager.GetSceneByBuildIndex(4).name, 2},
        {SceneManager.GetSceneByBuildIndex(5).name, 3}
    };

}
