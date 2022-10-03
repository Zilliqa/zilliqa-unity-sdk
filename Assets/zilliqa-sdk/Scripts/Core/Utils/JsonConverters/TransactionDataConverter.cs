using System;
using Newtonsoft.Json;
using Zilliqa.Requests;

public class TransactionDataConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

        string datastr = ((string)reader.Value);
        //datastr = datastr.Remove(0, 1);
        //datastr = datastr.Remove(datastr.Length -1, 1);
        

        return JsonConvert.DeserializeObject<TransactionData>(datastr);
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(string).IsAssignableFrom(objectType);
    }

}
