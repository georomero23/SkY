using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Skysense_models.APIS.Huawei
{
    #region Fusion 
    [Serializable]
    public class APIFusionSolar<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int failCode { get; set; }
        public T[] data { get; set; }
    }

    [Serializable]
    public class APIEnergiaDataFusionSolar
    {
        public long collectTime { get; set; }
        public string stationCode { get; set; }
        public APIEnergiaDataItemMapFusionSolar dataItemMap { get; set; }
    }

    [Serializable]
    public class APIEnergiaDataItemMapFusionSolar
    {
        public decimal inverterYield { get; set; }
    }

    [Serializable]
    public class APIFusionSolar_Devices
    {
        public string devDn { get; set; }
        public string devName { get; set; }
        public int devTypeId { get; set; }
        public string esnCode { get; set; }
        public string id { get; set; }
        public string invType { get; set; }
        public decimal? latitude { get; set; }
        public decimal? longitude { get; set; }
        public string model { get; set; }
        public int? optimizerNumber { get; set; }
        public string softwareVersion { get; set; }
        public string stationCode { get; set; }
    }

    [Serializable]
    public class APIFusionSolar_DevicesData
    {
        public string devId { get; set; }
        public long collectTime { get; set; }
        public string sn { get; set; }
        public APIFusionSolar_DevicesData_dataItemMap dataItemMap { get;set; }
    }

    [Serializable]
    public class APIFusionSolar_DevicesData_dataItemMap
    {
        public decimal? product_power { get; set; }
        public decimal? perpower_ratio { get; set; }
        public decimal? installed_capacity { get; set; }
    }
    #endregion
}
