using Newtonsoft.Json;
public class VietQRGenerateRequest
{
    [JsonProperty("accountNo")]
    public string AccountNo { get; set; }

    [JsonProperty("accountName")]
    public string AccountName { get; set; }

    [JsonProperty("acqId")]
    public int AcqId { get; set; }

    [JsonProperty("amount")]
    public long? Amount { get; set; }

    [JsonProperty("addInfo")]
    public string AddInfo { get; set; }

    [JsonProperty("format")]
    public string Format { get; set; } = "text";

    [JsonProperty("template")]
    public string Template { get; set; } = "compact";
}

public class VietQRResponse
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("desc")]
    public string Desc { get; set; }

    [JsonProperty("data")]
    public VietQRData Data { get; set; }
}

public class VietQRData
{
    [JsonProperty("acpId")]
    public int AcpId { get; set; }

    [JsonProperty("accountName")]
    public string AccountName { get; set; }

    [JsonProperty("qrCode")]
    public string QrCode { get; set; }

    [JsonProperty("qrDataURL")]
    public string QrDataURL { get; set; }
}