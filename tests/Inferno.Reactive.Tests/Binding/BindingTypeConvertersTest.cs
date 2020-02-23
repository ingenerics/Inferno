﻿using System;
using Xunit;

namespace Inferno.Reactive.Tests
{
    /*
     * Tests will fail in debug mode, because of exceptions being thrown.
     */
    public class BindingTypeConvertersTest
    {
        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertNullNullableValues()
        {
            double? nullDouble = null;
            double? expected = null;
            var result = EqualityTypeConverter.DoReferenceCast(nullDouble, typeof(double?));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertNullableValues()
        {
            double? doubleValue = 1.0;
            double? expected = 1.0;
            var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double?));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldThrowWhenConvertingFromNullNullableToValueType()
        {
            double? nullDouble = null;
            Assert.Throws<InvalidCastException>(() => EqualityTypeConverter.DoReferenceCast(nullDouble, typeof(double)));
        }

        [Fact]
        public void EqualityTypeConverterDoReferenceCastNullableToValueType()
        {
            double? doubleValue = 1.0;
            double? expected = 1.0;
            var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void EqualityTypeConverterDoReferenceCastShouldConvertValueTypes()
        {
            var doubleValue = 1.0;
            var result = EqualityTypeConverter.DoReferenceCast(doubleValue, typeof(double));
            Assert.Equal(doubleValue, result);
        }
    }
}
