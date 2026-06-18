namespace Loyeris.Portfolio.Core.Enums;

/// <summary>
/// Classifies the physical or commercial type of a managed lot.
/// </summary>
public enum LotType
{
    /// <summary>
    /// A studio apartment.
    /// </summary>
    Studio = 0,

    /// <summary>
    /// A one-room apartment.
    /// </summary>
    T1 = 1,

    /// <summary>
    /// A two-room apartment.
    /// </summary>
    T2 = 2,

    /// <summary>
    /// A three-room apartment.
    /// </summary>
    T3 = 3,

    /// <summary>
    /// A garage or parking-related unit.
    /// </summary>
    Garage = 4,

    /// <summary>
    /// A commercial or professional unit.
    /// </summary>
    Local = 5,

    /// <summary>
    /// A lot type not covered by the predefined categories.
    /// </summary>
    Other = 6
}
