using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions.IMessageVariants.ICANSEE
{
    public enum ControlFunction
    {
        ExecutePreset,
        ListAllCameraConfigurations,
        ListAllComputeDevices,
        QueryComputeDevice,
        ListAllCameraInUse,
        UnloadPreset,
        UnloadCamera,
        UnloadAlgorithm,
        AddCamera,
        LoadCamera,
        LoadAlgorithm,
        LoadDeviceLocalImage,
        DisplayImageInServerGUI,
        RequestImageMessage
    }

    public interface IInputMessage : IMessage
    {
        ControlFunction ControlFunction { get; }
        List<string> Parameters { get; }

        string Text { get; }
    }
}
