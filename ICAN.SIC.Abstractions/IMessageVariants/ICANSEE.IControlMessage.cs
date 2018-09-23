using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions.IMessageVariants.ICANSEE
{
    public enum ControlFunction
    {
        Execute,
        ExecuteScalar,
        LoadCamera,
        UnloadAllCameras,
        UnloadAllAlgorithms
    }

    public interface IControlMessage : IMessage
    {
        ControlFunction Function { get; }
        string parameter { get; }
    }
}
