using System.Threading.Tasks;

namespace PuppeteerSharp;

/// <summary>
/// Exposes the bluetooth emulation abilities.
/// </summary>
public interface IBluetoothEmulation
{
    /// <summary>
    /// Emulate Bluetooth adapter. Required for bluetooth simulations.
    /// </summary>
    /// <param name="state">The desired bluetooth adapter state.</param>
    /// <param name="leSupported">Mark if the adapter supports low-energy bluetooth.</param>
    /// <returns>A <see cref="Task"/> that completes when the adapter is emulated.</returns>
    Task EmulateAdapterAsync(AdapterState state, bool leSupported = true);

    /// <summary>
    /// Disable emulated bluetooth adapter.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the emulation is disabled.</returns>
    Task DisableEmulationAsync();

    /// <summary>
    /// Simulated preconnected Bluetooth Peripheral.
    /// </summary>
    /// <param name="preconnectedPeripheral">The peripheral to simulate.</param>
    /// <returns>A <see cref="Task"/> that completes when the peripheral is simulated.</returns>
    Task SimulatePreconnectedPeripheralAsync(PreconnectedPeripheral preconnectedPeripheral);
}
