using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DLS.Game.Utilities.Json
{
    public class UnityConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Vector3 vector3)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector3.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector3.y);
                writer.WritePropertyName("z");
                writer.WriteValue(vector3.z);
                writer.WriteEndObject();
            }
            else if (value is Vector2 vector2)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector2.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector2.y);
                writer.WriteEndObject();
            }
            else if (value is Color color)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("r");
                writer.WriteValue(color.r);
                writer.WritePropertyName("g");
                writer.WriteValue(color.g);
                writer.WritePropertyName("b");
                writer.WriteValue(color.b);
                writer.WritePropertyName("a");
                writer.WriteValue(color.a);
                writer.WriteEndObject();
            }
        }
        

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Vector3))
            {
                JObject jo = JObject.Load(reader);
                return new Vector3(jo["x"].Value<float>(), jo["y"].Value<float>(), jo["z"].Value<float>());
            }
            else if (objectType == typeof(Vector2))
            {
                JObject jo = JObject.Load(reader);
                return new Vector2(jo["x"].Value<float>(), jo["y"].Value<float>());
            }
            else if (objectType == typeof(Color))
            {
                JObject jo = JObject.Load(reader);
                return new Color(jo["r"].Value<float>(), jo["g"].Value<float>(), jo["b"].Value<float>(), jo["a"].Value<float>());
            }
            else
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3) || objectType == typeof(Vector2) || objectType == typeof(Color);
        }
    }
}