
using Bam.Data.Repositories;
using Newtonsoft.Json;

namespace Bam.Protocol.Data.Common;


public class DeviceData : MachineData, IDevice, IHasHandle
{
    public DeviceData(): base(false)
    {
        this.ProcessDescriptorData = ProcessDescriptorData.Current;

        this.DeviceType = DeviceTypes.Invalid;
    }

    public DeviceData(bool initialize): base(initialize)
    {
        if (initialize)
        {
            Initialize();
        }
    }

    protected override void Initialize()
    {
        if (!IsInitialized)
        {
            base.Initialize();
            this.ProcessDescriptorData = ProcessDescriptorData.Current;
            if (string.IsNullOrEmpty(_handle))
            {
                this.Handle = Guid.NewGuid().ToString();
            }
            this.IsInitialized = true;
        }
    }
    
    public virtual ulong ProcessDescriptorId { get; set; }
    
    [JsonIgnore]
    public virtual ProcessDescriptorData ProcessDescriptorData { get; set; } = null!;

    public DeviceTypes DeviceType { get; set; }

    private string _handle = null!;

    public new string Handle
    {
        get
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            return _handle;
        }
        set
        {
            _handle = value;
        }
    }
}