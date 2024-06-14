using System.Text.Json.Serialization;

namespace Model.Entities.Device
{
    public sealed record Device
    {
        [JsonPropertyName("chip_model")]
        public int ChipModel { get; set; }

        [JsonPropertyName("chip_revision")]
        public int ChipRevision { get; set; }

        [JsonPropertyName("chip_id")]
        public string ChipId { get; set; }

        [JsonPropertyName("flash_chip_id")]
        public int FlashChipId { get; set; }

        [JsonPropertyName("flash_chip_size")]
        public int FlashChipSize { get; set; }

        [JsonPropertyName("free_heap")]
        public int FreeHeap { get; set; }

        [JsonPropertyName("num_of_cores")]
        public int NumOfCores { get; set; }

        [JsonPropertyName("cpu_freq_mhz")]
        public int CpuFreqMhz { get; set; }

        [JsonPropertyName("sdk_version")]
        public string SdkVersion { get; set; }

        [JsonPropertyName("boot_version")]
        public string IpAddress { get; set; }

        [JsonPropertyName("mac_address")]
        public string MacAddress { get; set; }

        [JsonPropertyName("wifi_ssid")]
        public string Ssid { get; set; }

        [JsonPropertyName("wifi_rssi")]
        public string Rssi { get; set; }

        [JsonPropertyName("uptime")]
        public int Uptime { get; set; }
    }
}
