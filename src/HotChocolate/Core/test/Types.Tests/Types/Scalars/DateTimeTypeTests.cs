using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Language;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;

namespace HotChocolate.Types;

public class DateTimeTypeTests
{
    [Fact]
    public void Serialize_Utc_DateTimeOffset()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        DateTimeOffset dateTime = new DateTime(
            2018,
            6,
            11,
            8,
            46,
            14,
            DateTimeKind.Utc);

        var expectedValue = "2018-06-11T08:46:14.000Z";

        // act
        var serializedValue = (string)dateTimeType.Serialize(dateTime);

        // assert
        Assert.Equal(expectedValue, serializedValue);
    }

    [Fact]
    public void Serialize_DateTimeOffset()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        var dateTime = new DateTimeOffset(
            new DateTime(2018, 6, 11, 8, 46, 14),
            new TimeSpan(4, 0, 0));
        var expectedValue = "2018-06-11T08:46:14.000+04:00";

        // act
        var serializedValue = (string)dateTimeType.Serialize(dateTime);

        // assert
        Assert.Equal(expectedValue, serializedValue);
    }

    [Fact]
    public void Serialize_Null()
    {
        // arrange
        var dateTimeType = new DateTimeType();

        // act
        var serializedValue = dateTimeType.Serialize(null);

        // assert
        Assert.Null(serializedValue);
    }

    [Fact]
    public void Serialize_String_Exception()
    {
        // arrange
        var dateTimeType = new DateTimeType();

        // act
        Action a = () => dateTimeType.Serialize("foo");

        // assert
        Assert.Throws<SerializationException>(a);
    }

    [Fact]
    public void ParseLiteral_StringValueNode()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        var literal = new StringValueNode(
            "2018-06-29T08:46:14+04:00");
        var expectedDateTime = new DateTimeOffset(
            new DateTime(2018, 6, 29, 8, 46, 14),
            new TimeSpan(4, 0, 0));

        // act
        var dateTime = (DateTimeOffset)dateTimeType
            .ParseLiteral(literal);

        // assert
        Assert.Equal(expectedDateTime, dateTime);
    }

    [InlineData("en-US")]
    [InlineData("en-AU")]
    [InlineData("en-GB")]
    [InlineData("de-CH")]
    [InlineData("de-de")]
    [Theory]
    public void ParseLiteral_StringValueNode_DifferentCulture(string cultureName)
    {
        // arrange
        Thread.CurrentThread.CurrentCulture =
            CultureInfo.GetCultureInfo(cultureName);

        var dateTimeType = new DateTimeType();
        var literal = new StringValueNode(
            "2018-06-29T08:46:14+04:00");
        var expectedDateTime = new DateTimeOffset(
            new DateTime(2018, 6, 29, 8, 46, 14),
            new TimeSpan(4, 0, 0));

        // act
        var dateTime = (DateTimeOffset)dateTimeType
            .ParseLiteral(literal);

        // assert
        Assert.Equal(expectedDateTime, dateTime);
    }

    [Fact]
    public void Deserialize_IsoString_DateTimeOffset()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        var dateTime = new DateTimeOffset(
            new DateTime(2018, 6, 11, 8, 46, 14),
            new TimeSpan(4, 0, 0));

        // act
        var deserializedValue = (DateTimeOffset)dateTimeType
            .Deserialize("2018-06-11T08:46:14+04:00");

        // assert
        Assert.Equal(dateTime, deserializedValue);
    }

    [Fact]
    public void Deserialize_ZuluString_DateTimeOffset()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        var dateTime = new DateTimeOffset(
            new DateTime(2018, 6, 11, 8, 46, 14),
            new TimeSpan(0, 0, 0));

        // act
        var deserializedValue = (DateTimeOffset)dateTimeType
            .Deserialize("2018-06-11T08:46:14.000Z");

        // assert
        Assert.Equal(dateTime, deserializedValue);
    }

    [Fact]
    public void Deserialize_IsoString_DateTime()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        var dateTime = new DateTime(
            2018,
            6,
            11,
            8,
            46,
            14,
            DateTimeKind.Unspecified);

        // act
        var deserializedValue = ((DateTimeOffset)dateTimeType
            .Deserialize("2018-06-11T08:46:14+04:00")).DateTime;

        // assert
        Assert.Equal(dateTime, deserializedValue);
        Assert.Equal(DateTimeKind.Unspecified, deserializedValue.Kind);
    }

    [Fact]
    public void Deserialize_ZuluString_DateTime()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        DateTimeOffset dateTime = new DateTime(
            2018,
            6,
            11,
            8,
            46,
            14,
            DateTimeKind.Utc);

        // act
        var deserializedValue = ((DateTimeOffset)dateTimeType
            .Deserialize("2018-06-11T08:46:14.000Z"));

        // assert
        Assert.Equal(dateTime, deserializedValue.UtcDateTime);
    }

    [Fact]
    public void Deserialize_InvalidString_To_DateTimeOffset()
    {
        // arrange
        var type = new DateTimeType();

        // act
        var success = type.TryDeserialize("abc", out var deserialized);

        // assert
        Assert.False(success);
    }

    [Fact]
    public void Deserialize_DateTimeOffset_To_DateTimeOffset()
    {
        // arrange
        var type = new DateTimeType();
        var time = new DateTimeOffset(
            new DateTime(2018, 6, 11, 8, 46, 14, DateTimeKind.Utc));

        // act
        var success = type.TryDeserialize(time, out var deserialized);

        // assert
        Assert.True(success);
        Assert.Equal(time, deserialized);
    }

    [Fact]
    public void Deserialize_DateTime_To_DateTimeOffset()
    {
        // arrange
        var type = new DateTimeType();
        var time = new DateTime(2018, 6, 11, 8, 46, 14, DateTimeKind.Utc);

        // act
        var success = type.TryDeserialize(time, out var deserialized);

        // assert
        Assert.True(success);
        Assert.Equal(time,
            Assert.IsType<DateTimeOffset>(deserialized).UtcDateTime);
    }

    [Fact]
    public void Deserialize_NullableDateTime_To_DateTimeOffset()
    {
        // arrange
        var type = new DateTimeType();
        DateTime? time =
            new DateTime(2018, 6, 11, 8, 46, 14, DateTimeKind.Utc);

        // act
        var success = type.TryDeserialize(time, out var deserialized);

        // assert
        Assert.True(success);
        Assert.Equal(time,
            Assert.IsType<DateTimeOffset>(deserialized).UtcDateTime);
    }

    [Fact]
    public void Deserialize_NullableDateTime_To_DateTimeOffset_2()
    {
        // arrange
        var type = new DateTimeType();
        DateTime? time = null;

        // act
        var success = type.TryDeserialize(time, out var deserialized);

        // assert
        Assert.True(success);
        Assert.Null(deserialized);
    }

    [Fact]
    public void Deserialize_Null_To_Null()
    {
        // arrange
        var type = new DateTimeType();

        // act
        var success = type.TryDeserialize(null, out var deserialized);

        // assert
        Assert.True(success);
        Assert.Null(deserialized);
    }

    [Fact]
    public void ParseLiteral_NullValueNode()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        var literal = NullValueNode.Default;

        // act
        var value = dateTimeType.ParseLiteral(literal);

        // assert
        Assert.Null(value);
    }

    [Fact]
    public void ParseValue_DateTimeOffset()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        var dateTime = new DateTimeOffset(
            new DateTime(2018, 6, 11, 8, 46, 14),
            new TimeSpan(4, 0, 0));
        var expectedLiteralValue = "2018-06-11T08:46:14.000+04:00";

        // act
        var stringLiteral =
            (StringValueNode)dateTimeType.ParseValue(dateTime);

        // assert
        Assert.Equal(expectedLiteralValue, stringLiteral.Value);
    }

    [Fact]
    public void ParseValue_Utc_DateTimeOffset()
    {
        // arrange
        var dateTimeType = new DateTimeType();
        DateTimeOffset dateTime =
            new DateTime(2018, 6, 11, 8, 46, 14, DateTimeKind.Utc);
        var expectedLiteralValue = "2018-06-11T08:46:14.000Z";

        // act
        var stringLiteral =
            (StringValueNode)dateTimeType.ParseValue(dateTime);

        // assert
        Assert.Equal(expectedLiteralValue, stringLiteral.Value);
    }

    [Fact]
    public void ParseValue_Null()
    {
        // arrange
        var dateTimeType = new DateTimeType();

        // act
        var literal = dateTimeType.ParseValue(null);

        // assert
        Assert.IsType<NullValueNode>(literal);
    }

    [Fact]
    public void EnsureDateTimeTypeKindIsCorrect()
    {
        // arrange
        var type = new DateTimeType();

        // act
        var kind = type.Kind;

        // assert
        Assert.Equal(TypeKind.Scalar, type.Kind);
    }

    [Fact]
    public async Task Integration_DefaultDateTime()
    {
        // arrange
        var executor = await new ServiceCollection()
            .AddGraphQL()
            .AddQueryType<DefaultDateTime>()
            .BuildRequestExecutorAsync();

        // act
        var res = await executor.ExecuteAsync("{ test }");

        // assert
        res.ToJson().MatchSnapshot();
    }

    public class DefaultDateTime
    {
        public DateTime Test => default(DateTime);
    }
}
