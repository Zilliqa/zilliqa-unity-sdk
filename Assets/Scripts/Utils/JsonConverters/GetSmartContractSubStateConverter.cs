using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSmartContractSubStateConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var value = "";
        while (reader.Read())
        {
            value += reader.Value;
        }

        return JObject.Parse(value);
    }

    public override bool CanConvert(Type objectType)
    {
        return false;// typeof(ObjectId).IsAssignableFrom(objectType);
    }
}
