using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTransactionPayloadConverter : JsonConverter
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
            if (reader.TokenType == JsonToken.EndObject)
                break;

            if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "$oid")
            {
                reader.Read();
                value = (string)reader.Value;
            }
        }

        return "";// string.IsNullOrEmpty(value) ? ObjectId.Empty : new ObjectId(value);
    }

    public override bool CanConvert(Type objectType)
    {
        return false;// typeof(ObjectId).IsAssignableFrom(objectType);
    }
}
