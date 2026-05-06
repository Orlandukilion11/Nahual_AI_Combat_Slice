using UnityEngine;

// --- 1. THE DEFINITIONS ---

public enum CreatureStat
{
    None, HP, pATK, pDEF, spATK, spDEF, Speed
}

public enum ElementType
{
    None, Fire, Water, Grass, Ground, Air, Rock, Ice, Light, Dark, Electric, Ghost, Psychic, Fighting, Poison, Bug, Iron
}

public enum ItemType
{
    Resource,       // Crafting materials (Wood, Stone)
    Consumable,     // Potions, Berries (Heals/Feeds)
    BattleBooster,  // Battle Only: Temporary Buffs (Chili)
    Throwable,      // Overworld: Rock, Mud Ball (Distractions)
    CaptureTool,    // The Empty Totem (To catch creatures)
    CreatureBead,   // NEW: A captured creature (The Jade Bead)
    KeyItem,        // Letter, Map (Story)
    HeldItem        // Passive Buff (Jaguar Bracelet)
}

// --- 2. THE ASSET MENU ---

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("General Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;         // The 2D image for the UI
    public ItemType itemType;
    public float weight = 1.0f; // For physics throwing (Rock is heavier than Berry)

    [Header("3D Visuals")]
    public GameObject prefab;   // What spawns when you throw/drop it

    [Header("Consumable / Battle Logic")]
    public int healthRestore;        // For Potions/Berries
    public CreatureStat boostStat;   // Which stat? (e.g. pATK)
    public ElementType boostElement; // Or which element? (e.g. Fire)
    public int boostAmount;          // +5 Attack
    public float duration;           // 0 = Permanent, 3 = 3 Turns

    [Header("Capture Logic")]
    [Range(0, 100)] public float captureRate; // Strength of the Totem (Jade > Clay)
    public ElementType effectiveType;         // (Optional) Dark Totem works on Dark types
}
