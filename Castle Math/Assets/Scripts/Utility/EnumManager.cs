﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumManager{

	public enum QuestionType { Add, Subtract, Multiply, Divide, Algebra, Compare, Fraction, TrueOrFalse };

    public enum Upgrades { Barrricade1, Barricade2, IceArrows1, IceArrows2A, IceArrows2B,
        FireArrows1, FireArrows2A, FireArrows2B, ShockArrows1, ShockArrows2A, ShockArrows2B, 
        Inventory1, Inventory2
    }

    public enum ActiveQuestionCategories { AddOrSubtract, MultiplyOrDivide, Algebra}

    public enum Controls { keyboard, vr, mouse};

    public enum GameState { MainMenu, LevelSelect, Wave, PotionShop, Intermath}

    public enum PotionState { shop, menu, inventory }

    public enum GemType { Red, Yellow, Purple, Cyan, Green}

    public static Dictionary<EnumManager.GemType, float> gemsValues = new Dictionary<GemType, float>()
    {
        {GemType.Red, .01f },
        {GemType.Yellow, .05f },
        {GemType.Purple, .1f },
        {GemType.Cyan, .25f },
        {GemType.Green, 1f }

    };
}
