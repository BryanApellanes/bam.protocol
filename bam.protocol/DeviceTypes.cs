namespace Bam.Protocol;

/// <summary>
/// Enumerates the types of client devices that can connect to a Bam server.
/// </summary>
public enum DeviceTypes
{
    /// <summary>
    /// Invalid or unrecognized device type.
    /// </summary>
    Invalid,
    /// <summary>
    /// Android mobile device.
    /// </summary>
    MobileAndroid,
    /// <summary>
    /// iOS mobile device.
    /// </summary>
    MobileIOS,
    /// <summary>
    /// Windows desktop.
    /// </summary>
    DesktopWindows,
    /// <summary>
    /// macOS desktop.
    /// </summary>
    DesktopMac,
    /// <summary>
    /// Linux desktop.
    /// </summary>
    DesktopLinux,
}