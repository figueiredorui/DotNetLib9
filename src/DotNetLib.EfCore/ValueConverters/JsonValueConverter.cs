using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace DotNetLib.EfCore.ValueConverters
{
    public class JsonValueConverter<TEntity> : ValueConverter<TEntity, string>
    {
        public JsonValueConverter(ConverterMappingHints mappingHints = default)
            : base(model => JsonSerializer.Serialize(model, (JsonSerializerOptions)null),
                   value => JsonSerializer.Deserialize<TEntity>(value, (JsonSerializerOptions)null), mappingHints)
        {
        }

        public static ValueConverterInfo DefaultInfo
        {
            get
            {
                return new ValueConverterInfo(typeof(TEntity), typeof(string), i => new JsonValueConverter<TEntity>(i.MappingHints));
            }
        }

        public static ValueConverter Default
        {
            get
            {
                return new JsonValueConverter<TEntity>(null);
            }
        }

    }

}
