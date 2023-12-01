using System;
using Xunit;

namespace Library.Entities.Test
{
    public class ReservationTest
    {
        [Fact]
        public void Period_ParameterlessCtor_DateFromIsToday()
        {
            // Arrange & Act
            var period = new Period();

            // Assert
            Assert.Equal(DateTime.Now.Date, period.DateFrom);
        }

        [Fact]
        public void Period_ParameterlessCtor_DateToIsFiveDaysFromToday()
        {
            // Arrange & Act
            var period = new Period();

            // Assert
            Assert.Equal(DateTime.Now.Date.AddDays(5), period.DateTo);
        }

        [Theory]
        [InlineData(2012, 3, 15)]
        [InlineData(2050, 1, 1)]
        public void Period_ParametrizedCtor_DateToIsFiveDaysFromAnyDate_ByDefault(int year, int month, int day)
        {
            // Arrange
            var dateFrom = new DateTime(year, month, day);
            
            // Act
            var period = new Period(dateFrom);

            // Assert
            const int maxNoOfDays = 5;
            Assert.Equal(dateFrom.AddDays(maxNoOfDays), period.DateTo);
        }
    }
}