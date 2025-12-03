using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Skysense_models.APIS
{
    //#region Fusion Solar
    //[Serializable]
    //public class APIFusionSolar
    //{
    //    public bool success { get; set; }
    //    public string message { get; set; }
    //    public int failCode { get; set; }
    //    public APIEnergiaDataFusionSolar[] data { get; set; }
    //}

    //[Serializable]
    //public class APIEnergiaDataFusionSolar
    //{
    //    public long collectTime { get; set; }
    //    public string stationCode { get; set; }
    //    public APIEnergiaDataItemMapFusionSolar dataItemMap { get; set; }
    //}

    //[Serializable]
    //public class APIEnergiaDataItemMapFusionSolar
    //{
    //    public decimal inverterYield { get; set; }
    //}
    //#endregion
    #region Solis
    [Serializable]
    public class APISolis
    {
        public string success { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public APIEnergiaAnualSolis[] data { get; set; }
    }

    [Serializable]
    public class APIEnergiaAnualSolis
    {
        public string id { get; set; }
        public string money { get; set; }
        public decimal energy { get; set; }
        public string dateStr { get; set; }
    }

    [Serializable]
    public class APISolisInverterList
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public APISolisInverterList_data data { get; set; }
    }
    [Serializable]
    public class APISolisInverterList_data
    {
        public APISolisInverterList_data_page page { get; set; }
        public int mpptSwitch { get; set; }
    }
    [Serializable]
    public class APISolisInverterList_data_page
    {
        public APISolisInverterList_data_page_records[] records { get; set; }
        public int total { get; set; }
        public int size { get; set; }
        public int current { get; set; }
        public int pages { get; set; }
    }
    [Serializable]
    public class APISolisInverterList_data_page_records
    {
        public string id { get; set; }
        public string sn { get; set; }
        public string stationId { get; set; }
        public double fisGenerateTime { get; set; }
    }
    [Serializable]
    public class APISolisInverterDaily
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public APISolisInverterDaily_data[] data { get; set; }
    }
    [Serializable]
    public class APISolisInverterDaily_data
    {
        public string inverterId { get; set; }
        public string dateStr { get; set; }
        public decimal energy { get; set; }
    }
    #endregion
    #region Sungrow
    [Serializable]
    public class APISungrow_Device
    {
        public string result_code { get; set; }
        public string result_msg { get; set; }
        public APISungrow_DeviceResultData result_data { get; set; }
    }

    [Serializable]
    public class APISungrow_DeviceResultData
    {
        public APISungrow_DeviceResultDataPageList[] pageList { get; set; }
        public int rowCount { get; set; }
    }

    [Serializable]
    public class APISungrow_DeviceResultDataPageList
    {
        public int chnnl_id { get; set; }
        public string type_name { get; set; }
        public string ps_key { get; set; }
        public string device_sn { get; set; }
        public string dev_status { get; set; }
        public int device_type { get; set; }
        public string factory_name { get; set; }
        public int uuid { get; set; }
        public DateTime grid_connection_date { get; set; }
        public string device_name { get; set; }
        public int dev_fault_status { get; set; }
        public int rel_state { get; set; }
        public int device_code { get; set; }
        public int ps_id { get; set; }
        public int device_model_id { get; set; }
        public string communication_dev_sn { get; set; }
        public string device_model_code { get; set; }
    }

    [Serializable]
    public class APISungrow_InvertersYield<T>
    {
        public string result_code { get; set; }
        public string result_msg { get; set; }
        public Dictionary<string, APISungrow_InvertersYieldInfo1<T>> result_data { get; set; }
    }

    [Serializable]
    public class APISungrow_InvertersYieldInfo1<T>
    {
        public T[] p1 { get; set; }
    }

    [Serializable]
    public class APISungrow_InvertersYieldInfo4
    {
        [JsonProperty(PropertyName = "4")]
        public decimal dato { get; set; }
        public string time_stamp { get; set; }
    }

    [Serializable]
    public class APISungrow_InvertersYieldInfo2
    {
        [JsonProperty(PropertyName = "2")]
        public decimal dato { get; set; }
        public string time_stamp { get; set; }
    }
    
    [Serializable]
    public class APISungrow_Token
    {
        public string result_code { get; set; }
        public string result_msg { get; set; }
        public APISungrow_ResultData result_data { get; set; }
    }
    [Serializable]
    public class APISungrow_ResultData
    {
        public string token { get; set; }
    }
    #endregion
}
