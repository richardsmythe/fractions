namespace Fraction.Tests
{
    public class FractionOperationTests
    {
        [Theory]
        [InlineData(2, 5, 1, 4, Operation.Subtraction, "3/20")]
        [InlineData(-3, 4, 1, 2, Operation.Subtraction, "-5/4")]
        [InlineData(-7, 10, -2, 10, Operation.Subtraction, "-1/2")]
        [InlineData(-7, -10, -2, 10, Operation.Subtraction, "9/10")]
        [InlineData(-7, -10, -2, -10, Operation.Subtraction, "1/2")]
        [InlineData(-7, -10, -2, -15, Operation.Addition, "5/6")]
        [InlineData(2, 5, 1, 4, Operation.Addition, "13/20")]
        [InlineData(-7, 10, 2, 10, Operation.Addition, "-1/2")]
        [InlineData(7, 10, -2, 10, Operation.Addition, "1/2")]
        [InlineData(-7, 10, -2, 10, Operation.Addition, "-9/10")]
        [InlineData(2, 5, 1, 4, Operation.Multiplication, "1/10")]
        [InlineData(-7, 10, -2, 10, Operation.Multiplication, "7/50")]
        [InlineData(7, -10, 2, -10, Operation.Multiplication, "7/50")]
        [InlineData(7, 10, -2, 53, Operation.Multiplication, "-7/265")]
        [InlineData(2, 5, 1, 4, Operation.Division, "8/5")]
        [InlineData(-2, 5, -5, 6, Operation.Division, "12/25")]
        [InlineData(-2, 6, 5, 56, Operation.Division, "-3 11/15")]
        [InlineData(-2, 6, -5, 56, Operation.Division, "3 11/15")]
        [InlineData(2, -6, 5, -56, Operation.Division, "3 11/15")]
        public void TestFractionOperation(
            long numerator1, long denominator1,
            long numerator2, long denominator2,
            Operation operation,
            string expectedResult)
        {
            Fraction frac1 = new Fraction(numerator1, denominator1);
            Fraction frac2 = new Fraction(numerator2, denominator2);
            Fraction result;

            switch (operation)
            {
                case Operation.Addition:
                    result = Fraction.Add(frac1, frac2);
                    break;
                case Operation.Subtraction:
                    result = Fraction.Subtraction(frac1, frac2);
                    break;
                case Operation.Multiplication:
                    result = Fraction.Multiplication(frac1, frac2);
                    break;
                case Operation.Division:
                    result = Fraction.Division(frac1, frac2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), "Invalid operation specified.");
            }
            Assert.Equal(expectedResult, result.ToString());
        }

        [Fact]
        public void DenominatorNotZero()
        {
            Assert.Throws<DivideByZeroException>(() => new Fraction(4, 0));
        }


        [Fact]
        public void IsMixedFraction()
        {
            var frac1 = new Fraction(3, 2, 5);
            bool result = frac1.IsMixedFraction();
            Assert.True(result);
        }

        [Fact]
        public void MixedFractionToFraction()
        {
            var result = Fraction.MixedToFraction(3, 2, 5);
            Assert.Equal(17, result.Numerator);
            Assert.Equal(5, result.Denominator);
        }

        [Fact]
        public void ImproperToMixedFraction()
        {
            var frac1 = new Fraction(11, 4);
            Fraction result = Fraction.ToMixedFraction(frac1.Numerator, frac1.Denominator);
            Assert.Equal(2, result.WholePart);
            Assert.Equal(3, result.Numerator);
            Assert.Equal(4, result.Denominator);
        }
        [Fact]
        public void ProperToMixedFraction()
        {
            var frac1 = new Fraction(4, 11);
            Fraction result = Fraction.ToMixedFraction(frac1.Numerator, frac1.Denominator);
            Assert.Equal(0, result.WholePart);
            Assert.Equal(4, result.Numerator);
            Assert.Equal(11, result.Denominator);
        }


        [Fact]
        public void FractionsAreDifferent()
        {
            var frac1 = new Fraction(1, 2);
            var frac2 = new Fraction(3, 4);
            Assert.True(frac1 != frac2);
            Assert.Equal(1, frac2.CompareTo(frac1));
            Assert.Equal(-1, frac1.CompareTo(frac2));
        }

        [Fact]
        public void FractionsAreEqual()
        {
            var frac1 = new Fraction(3, 4);
            var frac2 = new Fraction(3, 4);
            Assert.True(frac1 == frac2);
            Assert.Equal(0, frac2.CompareTo(frac1));
            Assert.Equal(0, frac1.CompareTo(frac2));
        }

        [Fact]
        public void FractionGreaterThan()
        {
            var frac1 = new Fraction(5, 6);
            var frac2 = new Fraction(3, 4);
            Assert.True(frac1 > frac2);
            Assert.Equal(1, frac1.CompareTo(frac2));
            Assert.Equal(-1, frac2.CompareTo(frac1));
        }

        [Fact]
        public void FractionLessThan()
        {
            var frac1 = new Fraction(3, 4);
            var frac2 = new Fraction(5, 6);
            Assert.True(frac1 < frac2);
            Assert.Equal(-1, frac1.CompareTo(frac2));
            Assert.Equal(1, frac2.CompareTo(frac1));
        }

        [Theory]
        [InlineData(0.6, "3,5")]
        [InlineData(0.75, "3,4")]
        [InlineData(0.50, "1,2")]
        [InlineData(0.123456789, "123456789,1000000000")]
        [InlineData(0.987654321, "987654321,1000000000")]
        public void ExactDecimalToFraction(decimal input, string expectedFractionString)
        {
            string[] fractionParts = expectedFractionString.Split(',');
            int expectedNumerator = int.Parse(fractionParts[0]);
            int expectedDenominator = int.Parse(fractionParts[1]);
            Fraction expected = new Fraction(expectedNumerator, expectedDenominator);
            Fraction result = Fraction.ExactDecimalToFraction(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0.6, false)]
        [InlineData(0.333333, true)]
        [InlineData(0.123123123, true)]
        [InlineData(0.4166666, true)]
        public void IsReoccuringDecimal(decimal input, bool expectedResult)
        {
            string fractionPart = input.ToString().Split('.')[1];
            int pattern = Fraction.FindRepeatingPatternLength(fractionPart);
            bool isRecurring = pattern > 1;
            Assert.Equal(expectedResult, isRecurring);
        }

        [Theory]
        [InlineData(0.6, 0)]
        [InlineData(1.23, 0)]
        [InlineData(0.333333, 6)]
        [InlineData(0.123123123, 9)]
        [InlineData(0.4166666, 5)]
        public void FindRepeatingPatternLength(decimal input, int expectedResult)
        {
            string fractionPart = input.ToString().Split('.')[1];
            int pattern = Fraction.FindRepeatingPatternLength(fractionPart);
            Assert.Equal(expectedResult, pattern);
        }

        [Theory]
        [InlineData(0.6, 0)]
        [InlineData(0.333333, 1)]
        [InlineData(0.123123123, 3)]
        [InlineData(0.4166666, 1)]
        public void FindUniqueRepeatingPattern(decimal input, int expectedResult)
        {
            string fractionPart = input.ToString().Split('.')[1];
            int pattern = Fraction.FindUniqueRepeatingPattern(fractionPart);
            Assert.Equal(expectedResult, pattern);
        }

        [Theory]
        [InlineData(0.111111111, "1,9")]
        [InlineData(0.5555, "5,9")]
        [InlineData(1.233333333, "37,300")]
        [InlineData(1.4444444, "13,90")]
        [InlineData(0.6666666, "2,3")]
        [InlineData(0.41666666666, "5,12")]

        public void SimpleRecurringDecimalToFraction(decimal input, string expectedFractionString)
        {
            string[] fractionParts = expectedFractionString.Split(',');
            int expectedNumerator = int.Parse(fractionParts[0]);
            int expectedDenominator = int.Parse(fractionParts[1]);
            Fraction expected = new Fraction(expectedNumerator, expectedDenominator);
            Fraction result = Fraction.SimpleReoccuringDecimalToFraction(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateHCF_Returns_Correct_Result()
        {
            long numerator = 4166625;
            long denominator = 9999900;
            long expectedHCF = 833325;
            long actualHCF = Fraction.CalculateHCF(numerator, denominator);
            Assert.Equal(expectedHCF, actualHCF);
        }

        [Fact]
        public void LargeNumbers_Addition_Precision()
        {
            Fraction frac1 = new Fraction(long.MaxValue / 2, 1);
            Fraction frac2 = new Fraction(long.MaxValue / 2, 1);
            Fraction expected = new Fraction(long.MaxValue - 1, 1);
            Fraction result = Fraction.Add(frac1, frac2);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Multiplication_LargeNumbers_Precision()
        {
            Fraction frac1 = new Fraction(long.MaxValue / 2, 1);
            Fraction frac2 = new Fraction(2, 1);
            Fraction expected = new Fraction(long.MaxValue - 1, 1);
            Fraction result = Fraction.Multiplication(frac1, frac2);
            Assert.Equal(expected, result);
        }

        public enum Operation
        {
            GetSimplifiedFraction,
            Addition,
            Subtraction,
            Multiplication,
            Division
        }

        [Theory]
        [InlineData("-1/2+1/2+1/3", "1/3")]
        [InlineData("1/2+1/3+1/6", "1")]
        [InlineData("1/4+1/4+1/4+1/4", "1")]
        [InlineData("-3/4+1/2", "-1/4")]
        [InlineData("2/5+3/10", "7/10")]
        [InlineData("1/2+1/3-1/6", "2/3")]
        public void TestAdditionWithStringExpression(string expression, string expectedResult)
        {
            Fraction result = Fraction.Add(expression);
            Assert.Equal(expectedResult, result.ToString());
        }

        [Theory]
        [InlineData("1/2-1/3-1/6", "0")]
        [InlineData("3/4-1/4", "1/2")]
        [InlineData("-3/4-1/4", "-1")]
        [InlineData("5/6-1/3", "1/2")]
        [InlineData("1/2-1/4+1/8", "3/8")]
        public void TestSubtractionWithStringExpression(string expression, string expectedResult)
        {
            Fraction result = Fraction.Subtract(expression);
            Assert.Equal(expectedResult, result.ToString());
        }

        [Theory]
        [InlineData("2/3*3/4*4/5", "2/5")]
        [InlineData("1/2*2", "1")]
        [InlineData("-1/2*2", "-1")]
        [InlineData("3/4*4/3", "1")]
        [InlineData("2/5*5/2", "1")]
        public void TestMultiplicationWithStringExpression(string expression, string expectedResult)
        {
            Fraction result = Fraction.Multiply(expression);
            Assert.Equal(expectedResult, result.ToString());
        }

        [Theory]
        [InlineData("1/2/2", "1/4")]
        [InlineData("3/4/3/4", "1")]
        [InlineData("1/3/1/6", "2")]
        [InlineData("8/5/2", "4/5")]
        public void TestDivisionWithStringExpression(string expression, string expectedResult)
        {
            Fraction result = Fraction.Divide(expression);
            Assert.Equal(expectedResult, result.ToString());
        }

        [Theory]
        [InlineData("1/2", "1/2")]
        [InlineData("-1/2", "-1/2")]
        [InlineData("3", "3")]
        [InlineData("-5", "-5")]
        public void TestParseFraction(string fractionString, string expectedResult)
        {
            Fraction result = Fraction.ParseFraction(fractionString);
            Assert.Equal(expectedResult, result.ToString());
        }

        [Fact]
        public void TestNegativeNumberVsSubtractionOperator()
        {
            Fraction result1 = Fraction.Add("-1/2+1/2");
            Assert.Equal("0", result1.ToString());
            Fraction result2 = Fraction.Add("1/2-1/2");
            Assert.Equal("0", result2.ToString());
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void TestComplexNegativeExpression()
        {
            string expression = "-7/10+2/15";
            Fraction result = Fraction.Add(expression);
            Assert.Equal("-17/30", result.ToString());
        }

        [Fact]
        public void TestEmptyExpressionThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Fraction.Add(""));
        }

        [Fact]
        public void TestInvalidFractionFormatThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Fraction.ParseFraction("1/2/3"));
        }

    }

}

