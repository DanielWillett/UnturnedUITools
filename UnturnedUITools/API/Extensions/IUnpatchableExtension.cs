namespace DanielWillett.UITools.API.Extensions;

/// <summary>
/// Any patches using harmony should implement this. Allows the last extension left when the manager is being disposed to unpatch any static patches.
/// </summary>
public interface IUnpatchableExtension
{
    /// <summary>
    /// Called on the last extension left when the manager is being disposed.
    /// </summary>
    public void Unpatch();
}
