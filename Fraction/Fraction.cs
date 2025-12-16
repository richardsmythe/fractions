namespace Fraction
{
    public readonly struct Fraction : IComparable<Fraction>
    {
        public long WholePart { get; }
        public long Numerator { get; }
        public long Denominator { get; }

        /// <summary>
        /// Initializes a new instance of the Fraction struct with a numerator and denominator.
        /// </summary>
        /// <param name="numerator">The numerator of the fraction.</param>
        /// <param name="denominator">The denominator of the fraction.</param>
        /// <exception cref="DivideByZeroException">Thrown when denominator is zero.</exception>
        public Fraction(long numerator, long denominator) : this(0, numerator, denominator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Fraction struct with a whole part, numerator, and denominator.
        /// </summary>
        /// <param name="wholePart">The whole number part of the mixed fraction.</param>
        /// <param name="numerator">The numerator of the fraction.</param>
        /// <param name="denominator">The denominator of the fraction.</param>
        /// <exception cref="DivideByZeroException">Thrown when denominator is zero.</exception>
        public Fraction(long wholePart, long numerator, long denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException("Denominator cannot be zero.");

            if (numerator == 0)
            {
                WholePart = wholePart;
                Numerator = 0;
                Denominator = 1;
            }
            else if (denominator < 0)
            {
                // standardises by negating everything so that two negatives make positive.
                WholePart = -wholePart;
                Numerator = -numerator;
                Denominator = -denominator;
            }
            else
            {
                WholePart = wholePart;
                Numerator = numerator;
                Denominator = denominator;
            }
        }

        /// <summary>
        /// Gets the simplified form of this fraction.
        /// </summary>
        /// <returns>A new Fraction instance in simplified form.</returns>
        public Fraction GetSimplifiedFraction() => Simplify(Numerator, Denominator);

        /// <summary>
        /// Gets the reciprocal (multiplicative inverse) of this fraction.
        /// </summary>
        /// <returns>A new Fraction representing the reciprocal.</returns>
        /// <exception cref="DivideByZeroException">Thrown when attempting to get reciprocal of zero.</exception>
        public Fraction Reciprocal
        {
            get
            {
                var improper = WholePart != 0 ? MixedToFraction(WholePart, Numerator, Denominator) : this;
                if (improper.Numerator == 0)
                    throw new DivideByZeroException("Cannot take reciprocal of zero.");

                return new Fraction(improper.Denominator, improper.Numerator);
            }
        }

        /// <summary>
        /// Converts the fraction to a double-precision floating-point number.
        /// </summary>
        /// <returns>The decimal representation of the fraction.</returns>
        public double ToDouble()
        {
            var improper = WholePart != 0 ? MixedToFraction(WholePart, Numerator, Denominator) : this;
            return (double)improper.Numerator / improper.Denominator;
        }

        /// <summary>
        /// Returns a string representation of the fraction in simplified form.
        /// </summary>
        /// <returns>A string representing the fraction (e.g., "1/2", "3", "2 1/4").</returns>
        public override string ToString()
        {
            if (Numerator == 0)
                return WholePart == 0 ? "0" : $"{WholePart}";
            
            if (WholePart == 0)
            {
                if (Denominator == 1)
                    return $"{Numerator}";
                if (Denominator != 0 && Numerator % Denominator == 0)
                    return $"{Numerator / Denominator}";
                return $"{Numerator}/{Denominator}";
            }
            
            return $"{WholePart} {Numerator}/{Denominator}";
        }

        /// <summary>
        /// Simplifies the given fraction represented by the numerator and denominator.
        /// </summary>
        /// <param name="numerator">The numerator of the fraction.</param>
        /// <param name="denominator">The denominator of the fraction.</param>
        /// <returns>The simplified fraction.</returns>
        /// <exception cref="DivideByZeroException">Thrown when the denominator is zero.</exception>
        private static Fraction Simplify(long numerator, long denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException("Denominator cannot be zero.");

            long hcf = CalculateHCF(numerator, denominator);
            if (hcf == 1)
                return new Fraction(numerator, denominator);            // can't be simplified

            long simplifiedNumerator = numerator / hcf;
            long simplifiedDenominator = Math.Abs(denominator / hcf);   // keep denominator positive for convention

            // check if it's a mixed fraction
            if (Math.Abs(simplifiedNumerator) > Math.Abs(simplifiedDenominator))
            {
                return ToMixedFraction(simplifiedNumerator, simplifiedDenominator);
            }
            else
            {
                return new Fraction(simplifiedNumerator, simplifiedDenominator);
            }
        }

        /// <summary>
        /// Converts a decimal number to its equivalent simplified fraction.
        /// </summary>
        /// <param name="dec">The decimal number to convert.</param>
        /// <returns>The simplified fraction representation of the input decimal.</returns>
        /// <exception cref="System.Exception">Thrown when the repeating decimal cannot be converted to a fraction.</exception>
        public static Fraction ExactDecimalToFraction(decimal dec)
        {
            string decStr = dec.ToString();
            int decimalIndex = decStr.IndexOf('.');
            string afterDecimalPart = decStr.Substring(decimalIndex + 1);
            long numerator = long.Parse(afterDecimalPart);
            long denominator = (long)Math.Pow(10, afterDecimalPart.Length);

            return new Fraction(numerator, denominator).GetSimplifiedFraction();
        }

        /// <summary>
        /// Converts a simple recurring decimal to its fraction equivalent.
        /// Note: Does not work for complex patterns like 123123123.
        /// </summary>
        /// <param name="dec">The recurring decimal to convert.</param>
        /// <returns>The simplified fraction representation.</returns>
        public static Fraction SimpleReoccuringDecimalToFraction(decimal dec)
        {
            // TO DO: doesn't work for complex patterns like 123123123
            
            string decStr = dec.ToString();
            int repeatingDigit = 0;

            for (int i = decStr.Length - 1; i >= 0; i--)
            {
                if (decStr[i] != '0' && decStr[i] != '.')
                {
                    repeatingDigit = int.Parse(decStr[i].ToString());
                    break;
                }
            }

            int patternLength = FindRepeatingPatternLength(decStr.Substring(decStr.IndexOf('.') + 1));
            decimal multiplier = (decimal)Math.Pow(10, patternLength);
            decimal x = dec * multiplier;
            string xStr = x.ToString();
            char[] xChars = xStr.ToCharArray();
            int decimalPointIndex = Array.IndexOf(xChars, '.');

            for (int i = decimalPointIndex + 1; i < xChars.Length; i++)
            {
                if (xChars[i] == '0' || xChars[i] == '.')
                {
                    xChars[i] = (char)(repeatingDigit + '0');
                }
            }

            string modifiedDecStr = new string(xChars);
            decimal modifiedDecimal = decimal.Parse(modifiedDecStr);
            decimal difference = modifiedDecimal - dec; // 41666.2500000
            string differenceStr = difference.ToString().Replace(".", "").TrimEnd('0');
            long numerator = long.Parse(differenceStr);
            long denominator = (long)Math.Pow(10, patternLength) - 1;

            if (denominator.ToString().Length < numerator.ToString().Length)
            {
                int padding = numerator.ToString().Length - denominator.ToString().Length;
                denominator *= (long)Math.Pow(10, padding);
            }

            return new Fraction(numerator, denominator).GetSimplifiedFraction();
        }


        /// <summary>
        /// Iterates through all possible starting positions of the repeating pattern 
        /// within the string and checks if the extracted substring repeats throughout the string. 
        /// If a repeating pattern is found, its length is returned. if no repeating pattern is found, return 0. 
        /// E.g. for 4166666 it will return 5
        /// </summary>
        /// <param name="repeatingPart">The string containing the repeating pattern.</param>
        /// <returns>The length of the repeating pattern, or 0 if no pattern is found.</returns>
        public static int FindRepeatingPatternLength(string repeatingPart)
        {
            int length = repeatingPart.Length;
            if (length == 1) return 0;
            string repeatedSequence = "";

            for (int patternLength = 1; patternLength <= length / 2; patternLength++)
            {
                string pattern = repeatingPart.Substring(0, patternLength);

                // identify if the pattern repeats throughout the string
                bool isRepeated = true;
                for (int i = 0; i < length; i += patternLength)
                {
                    int remainingLength = Math.Min(patternLength, length - i);
                    if (!repeatingPart.Substring(i, remainingLength).Equals(pattern.Substring(0, remainingLength)))
                    {
                        isRepeated = false;
                        break;
                    }
                }

                //if so, store it and continue counting
                if (isRepeated)
                {
                    repeatedSequence = pattern;
                    int count = 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (repeatingPart[i] == repeatedSequence[count % patternLength])
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    return count;
                }
            }

            // if no repeating pattern is found, iterate through the string to count repeating digits
            // this is to check if the repeating digits occur further into the string
            string repeatedDigits = "";
            for (int i = 0; i < length; i++)
            {
                if (repeatingPart[i] == repeatingPart[(i + 1) % length] || (i == length - 1 && repeatingPart[i] == repeatingPart[i - 1]))
                {
                    repeatedDigits += repeatingPart[i];
                }
            }
            return repeatedDigits.Length;
        }

        /// <summary>
        /// Tries to find the shortest repeating pattern within a string and returns its length.
        /// E.g. for 123123123 it will return 3
        /// </summary>
        /// <param name="repeatingPart">The string to search for repeating patterns.</param>
        /// <returns>The length of the shortest repeating pattern, or 0 if none found.</returns>
        public static int FindUniqueRepeatingPattern(string repeatingPart)
        {
            int length = repeatingPart.Length;
            for (int patternLength = 1; patternLength <= length / 2; patternLength++)
            {
                for (int startIndex = 0; startIndex + 2 * patternLength <= length; startIndex++)
                {
                    string pattern = repeatingPart.Substring(startIndex, patternLength);
                    string nextPattern = repeatingPart.Substring(startIndex + patternLength, patternLength);

                    if (pattern != nextPattern) continue;

                    // if all substrings match, return the pattern length
                    if (startIndex + 2 * patternLength == length) return patternLength;
                }
            }
            return 0;
        }

        /// <summary>
        /// Determines whether this instance represents a mixed fraction (has a non-zero whole part).
        /// </summary>
        /// <returns>True if this is a mixed fraction; otherwise, false.</returns>
        public bool IsMixedFraction()
        {
            if (WholePart > 0) return true;
            return false;
        }

        /// <summary>
        /// Converts a mixed fraction to an improper fraction.
        /// </summary>
        /// <param name="whole">The whole number part.</param>
        /// <param name="numerator">The numerator of the fractional part.</param>
        /// <param name="denominator">The denominator of the fractional part.</param>
        /// <returns>An improper fraction equivalent to the mixed fraction.</returns>
        public static Fraction MixedToFraction(long whole, long numerator, long denominator)
        {
            var newNumerator = (whole * denominator) + numerator;
            return new Fraction(newNumerator, denominator);
        }

        /// <summary>
        /// Converts an improper fraction to a mixed fraction.
        /// </summary>
        /// <param name="numerator">The numerator of the improper fraction.</param>
        /// <param name="denominator">The denominator of the improper fraction.</param>
        /// <returns>A mixed fraction if the numerator is greater than the denominator; otherwise, a regular fraction.</returns>
        public static Fraction ToMixedFraction(long numerator, long denominator)
        {
            long wholePart = numerator / denominator;
            long remainder = numerator % denominator;

            if (wholePart == 0)
            {
                // no whole part, return a regular fraction
                return new Fraction(numerator, denominator);
            }
            long adjustedNumerator = Math.Abs(remainder);

            return new Fraction(wholePart, adjustedNumerator, denominator);
        }

        /// <summary>
        /// Calculates the highest common factor (HCF/GCD) using Euclid's algorithm.
        /// </summary>
        /// <param name="numerator">The first number.</param>
        /// <param name="denominator">The second number.</param>
        /// <returns>The highest common factor of the two numbers.</returns>
        /// <exception cref="ArgumentException">Thrown when both values are zero.</exception>
        public static long CalculateHCF(long numerator, long denominator)
        {
            numerator = Math.Abs(numerator);
            denominator = Math.Abs(denominator);

            if (numerator == 0 && denominator == 0)
                throw new ArgumentException("Numerator and denominator cannot be zero.");

            while (denominator != 0)
            {
                long temp = numerator % denominator;
                numerator = denominator;
                denominator = temp;
            }
            
            return numerator == 0 ? 1 : numerator;
        }

        /// <summary>
        /// Find the least common multiple (LCM) of given numerator and denominator
        /// </summary>
        /// <param name="numerator">The first number.</param>
        /// <param name="denominator">The second number.</param>
        /// <returns>The least common multiple of the two numbers.</returns>
        /// <exception cref="ArgumentException">Thrown when both values are zero.</exception>
        /// <exception cref="OverflowException">Thrown when the result exceeds long.MaxValue.</exception>
        public static long CalculateLCM(long numerator, long denominator)
        {
            numerator = Math.Abs(numerator);
            denominator = Math.Abs(denominator);
            
            if (numerator == 0 || denominator == 0)
                return 0;
            
            long hcf = CalculateHCF(numerator, denominator);
            
            checked
            {
                return (numerator / hcf) * denominator;
            }
        }

        /// <summary>
        /// Parses a fraction string (eg "1/2", "-3/4", "5") into a Fraction object.
        /// </summary>
        /// <param name="fractionStr">The string representation of a fraction.</param>
        /// <returns>A Fraction object.</returns>
        /// <exception cref="ArgumentException">Thrown when the string is empty or has invalid format.</exception>
        public static Fraction ParseFraction(string fractionStr)
        {
            fractionStr = fractionStr.Trim();
            
            if (string.IsNullOrWhiteSpace(fractionStr)) throw new ArgumentException("Fraction string cannot be empty.");
            if (fractionStr.Contains('/'))
            {
                string[] parts = fractionStr.Split('/');
                if (parts.Length != 2)throw new ArgumentException($"Invalid fraction format: {fractionStr}");

                long numerator = long.Parse(parts[0].Trim());
                long denominator = long.Parse(parts[1].Trim());
                return new Fraction(numerator, denominator);
            }
            else
            {
                long wholeNumber = long.Parse(fractionStr);
                return new Fraction(wholeNumber, 1);
            }
        }

        /// <summary>
        /// Parses a mathematical expression containing fractions and returns a list of fractions and operators.
        /// Distinguishes between negative signs and subtraction operators.
        /// </summary>
        /// <param name="expression">The expression string (eg "-1/2+1/2+1/3").</param>
        /// <param name="operation">The primary operation type ('+', '-', '*', '/').</param>
        /// <returns>A list of tuples containing fractions and their associated operator ('+' or '-' for sign).</returns>
        private static List<(Fraction fraction, char operation)> ParseExpression(string expression, char primaryOperation)
        {
            expression = expression.Replace(" ", "");
            var result = new List<(Fraction, char)>();
            
            int i = 0;
            while (i < expression.Length)
            {
                char currentOp = '+';
                
                // check if this is an operator or a negative sign
                if (i > 0 && (expression[i] == '+' || expression[i] == '-' || expression[i] == '*' || expression[i] == '/'))
                {
                    currentOp = expression[i];
                    i++;
                }
                else if (i == 0 && expression[i] == '-')
                {
                    currentOp = '+';
                }
                
                int start = i;
                if (i < expression.Length && expression[i] == '-')
                {
                    i++; 
                }
                
                while (i < expression.Length && char.IsDigit(expression[i]))
                {
                    i++;
                }
                
                if (i < expression.Length && expression[i] == '/')
                {
                    i++;                   
                    while (i < expression.Length && char.IsDigit(expression[i]))
                    {
                        i++;
                    }
                }
                
                string fractionStr = expression.Substring(start, i - start);
                Fraction fraction = ParseFraction(fractionStr);
                result.Add((fraction, currentOp));
            }
            
            return result;
        }

        /// <summary>
        /// Adds multiple fractions from a string expression (e.g., "1/2+1/3+1/6").
        /// </summary>
        /// <param name="expression">The expression string containing fractions and operators.</param>
        /// <returns>The result of adding all fractions in the expression.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is empty or contains invalid operators.</exception>
        public static Fraction Add(string expression)
        {
            var tokens = ParseExpression(expression, '+');
            if (tokens.Count == 0)
                throw new ArgumentException("Expression must contain at least one fraction.");

            Fraction result = tokens[0].fraction;
            
            for (int i = 1; i < tokens.Count; i++)
            {
                var (fraction, operation) = tokens[i];
                
                if (operation == '+')
                {
                    result = Add(result, fraction);
                }
                else if (operation == '-')
                {
                    result = Subtraction(result, fraction);
                }
                else
                {
                    throw new ArgumentException($"Invalid operator '{operation}' in addition expression.");
                }
            }
            
            return result;
        }

        /// <summary>
        /// Adds multiple fractions together.
        /// </summary>
        /// <param name="fractions">An array of fractions to add.</param>
        /// <returns>The sum of all fractions.</returns>
        /// <exception cref="ArgumentException">Thrown when no fractions are provided.</exception>
        public static Fraction Add(params Fraction[] fractions)
        {
            if (fractions == null || fractions.Length == 0) throw new ArgumentException("At least one fraction is required.");

            Fraction result = fractions[0];
            for (int i = 1; i < fractions.Length; i++)
            {
                result = Add(result, fractions[i]);
            }
            return result;
        }

        /// <summary>
        /// Subtracts multiple fractions from a string expression (e.g., "1/2-1/3-1/6").
        /// </summary>
        /// <param name="expression">The expression string containing fractions and operators.</param>
        /// <returns>The result of subtracting all fractions in the expression.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is empty or contains invalid operators.</exception>
        public static Fraction Subtract(string expression)
        {
            var tokens = ParseExpression(expression, '-');
            if (tokens.Count == 0) throw new ArgumentException("Expression must contain at least one fraction.");

            Fraction result = tokens[0].fraction;
            
            for (int i = 1; i < tokens.Count; i++)
            {
                var (fraction, operation) = tokens[i];
                
                if (operation == '-'
)
                {
                    result = Subtraction(result, fraction);
                }
                else if (operation == '+')
                {
                    result = Add(result, fraction);
                }
                else
                {
                    throw new ArgumentException($"Invalid operator '{operation}' in subtraction expression.");
                }
            }
            
            return result;
        }

        /// <summary>
        /// Subtracts multiple fractions sequentially.
        /// </summary>
        /// <param name="fractions">An array of fractions to subtract.</param>
        /// <returns>The result of sequential subtraction.</returns>
        /// <exception cref="ArgumentException">Thrown when no fractions are provided.</exception>
        public static Fraction Subtract(params Fraction[] fractions)
        {
            if (fractions == null || fractions.Length == 0)throw new ArgumentException("At least one fraction is required.");

            Fraction result = fractions[0];
            for (int i = 1; i < fractions.Length; i++)
            {
                result = Subtraction(result, fractions[i]);
            }
            return result;
        }

        /// <summary>
        /// Multiplies multiple fractions from a string expression (e.g., "2/3*3/4*4/5").
        /// </summary>
        /// <param name="expression">The expression string containing fractions and operators.</param>
        /// <returns>The product of all fractions in the expression.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is empty or contains invalid operators.</exception>
        public static Fraction Multiply(string expression)
        {
            var tokens = ParseExpression(expression, '*');
            if (tokens.Count == 0)throw new ArgumentException("Expression must contain at least one fraction.");

            Fraction result = tokens[0].fraction;
            
            for (int i = 1; i < tokens.Count; i++)
            {
                var (fraction, operation) = tokens[i];
                
                if (operation == '*')
                {
                    result = Multiplication(result, fraction);
                }
                else if (operation == '/')
                {
                    result = Division(result, fraction);
                }
                else
                {
                    throw new ArgumentException($"Invalid operator '{operation}' in multiplication expression.");
                }
            }
            
            return result;
        }

        /// <summary>
        /// Multiplies multiple fractions together.
        /// </summary>
        /// <param name="fractions">An array of fractions to multiply.</param>
        /// <returns>The product of all fractions.</returns>
        /// <exception cref="ArgumentException">Thrown when no fractions are provided.</exception>
        public static Fraction Multiply(params Fraction[] fractions)
        {
            if (fractions == null || fractions.Length == 0) throw new ArgumentException("At least one fraction is required.");

            Fraction result = fractions[0];
            for (int i = 1; i < fractions.Length; i++)
            {
                result = Multiplication(result, fractions[i]);
            }
            return result;
        }

        /// <summary>
        /// Divides multiple fractions from a string expression (e.g., "1/2/2").
        /// </summary>
        /// <param name="expression">The expression string containing fractions and operators.</param>
        /// <returns>The result of sequential division.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is empty or contains invalid operators.</exception>
        public static Fraction Divide(string expression)
        {
            var tokens = ParseExpression(expression, '/');
            if (tokens.Count == 0) throw new ArgumentException("Expression must contain at least one fraction.");

            Fraction result = tokens[0].fraction;
            
            for (int i = 1; i < tokens.Count; i++)
            {
                var (fraction, operation) = tokens[i];
                
                if (operation == '/')
                {
                    result = Division(result, fraction);
                }
                else if (operation == '*')
                {
                    result = Multiplication(result, fraction);
                }
                else
                {
                    throw new ArgumentException($"Invalid operator '{operation}' in division expression.");
                }
            }
            
            return result;
        }

        /// <summary>
        /// Divides multiple fractions sequentially.
        /// </summary>
        /// <param name="fractions">An array of fractions to divide.</param>
        /// <returns>The result of sequential division.</returns>
        /// <exception cref="ArgumentException">Thrown when no fractions are provided.</exception>
        /// <exception cref="DivideByZeroException">Thrown when dividing by zero.</exception>
        public static Fraction Divide(params Fraction[] fractions)
        {
            if (fractions == null || fractions.Length == 0) throw new ArgumentException("At least one fraction is required.");

            Fraction result = fractions[0];
            for (int i = 1; i < fractions.Length; i++)
            {
                result = Division(result, fractions[i]);
            }
            return result;
        }

        /// <summary>
        /// Adds two fractions together.
        /// </summary>
        /// <param name="frac1">The first fraction.</param>
        /// <param name="frac2">The second fraction.</param>
        /// <returns>The sum of the two fractions in simplified form.</returns>
        public static Fraction Add(Fraction frac1, Fraction frac2)
        {
            // convert mixed fractions to improper fractions
            if (frac1.WholePart != 0 ) frac1 = MixedToFraction(frac1.WholePart, frac1.Numerator, frac1.Denominator);
            if (frac2.WholePart != 0) frac2 = MixedToFraction(frac2.WholePart, frac2.Numerator, frac2.Denominator);
            
            long newNumerator;
            long commonDenominator;
            if (frac1.Denominator == frac2.Denominator)     // denominators are the same, 
            {

                newNumerator = frac1.Numerator + frac2.Numerator;
                commonDenominator = frac1.Denominator;
            }
            else                                            // denominators are different, find the common denominator 
            {
                newNumerator = (frac1.Numerator * frac2.Denominator) + (frac1.Denominator * frac2.Numerator);
                commonDenominator = frac1.Denominator * frac2.Denominator;
            }
            Fraction result = new Fraction(newNumerator, commonDenominator);
            return result.GetSimplifiedFraction();
        }

        /// <summary>
        /// Subtracts the second fraction from the first.
        /// </summary>
        /// <param name="frac1">The fraction to subtract from.</param>
        /// <param name="frac2">The fraction to subtract.</param>
        /// <returns>The difference of the two fractions in simplified form.</returns>
        public static Fraction Subtraction(Fraction frac1, Fraction frac2)
        {
            // convert mixed fractions to improper fractions
            if (frac1.WholePart != 0) frac1 = MixedToFraction(frac1.WholePart, frac1.Numerator, frac1.Denominator);
            if (frac2.WholePart != 0)frac2 = MixedToFraction(frac2.WholePart, frac2.Numerator, frac2.Denominator);
            
            long newNumerator;
            long commonDenominator;
            if (frac1.Denominator == frac2.Denominator)
            {
                newNumerator = frac1.Numerator - frac2.Numerator;
                commonDenominator = frac1.Denominator;
            }
            else
            {
                long lcm = CalculateLCM(frac1.Denominator, frac2.Denominator);
                long adjustedNumerator1 = frac1.Numerator * (lcm / frac1.Denominator);
                long adjustedNumerator2 = frac2.Numerator * (lcm / frac2.Denominator);
                newNumerator = adjustedNumerator1 - adjustedNumerator2;
                commonDenominator = lcm;
            }
            Fraction result = new Fraction(newNumerator, commonDenominator);
            return result.GetSimplifiedFraction();
        }

        /// <summary>
        /// Multiplies two fractions together.
        /// </summary>
        /// <param name="frac1">The first fraction.</param>
        /// <param name="frac2">The second fraction.</param>
        /// <returns>The product of the two fractions in simplified form.</returns>
        /// <exception cref="OverflowException">Thrown when the multiplication exceeds long.MaxValue.</exception>
        public static Fraction Multiplication(Fraction frac1, Fraction frac2)
        {
            // convert mixed fractions to improper fractions
            if (frac1.WholePart != 0) frac1 = MixedToFraction(frac1.WholePart, frac1.Numerator, frac1.Denominator);
            if (frac2.WholePart != 0)frac2 = MixedToFraction(frac2.WholePart, frac2.Numerator, frac2.Denominator);
            
            checked
            {
                long newNumerator = frac1.Numerator * frac2.Numerator;
                long newDenominator = frac1.Denominator * frac2.Denominator;
                
                Fraction result = new Fraction(newNumerator, newDenominator);
                return result.GetSimplifiedFraction();
            }
        }

        /// <summary>
        /// Divides the first fraction by the second.
        /// </summary>
        /// <param name="frac1">The dividend fraction.</param>
        /// <param name="frac2">The divisor fraction.</param>
        /// <returns>The quotient of the two fractions in simplified form.</returns>
        /// <exception cref="DivideByZeroException">Thrown when dividing by zero.</exception>
        public static Fraction Division(Fraction frac1, Fraction frac2)
        {
            // Convert mixed fractions to improper fractions
            if (frac1.WholePart != 0)
                frac1 = MixedToFraction(frac1.WholePart, frac1.Numerator, frac1.Denominator);
            if (frac2.WholePart != 0)
                frac2 = MixedToFraction(frac2.WholePart, frac2.Numerator, frac2.Denominator);
            
            Fraction frac2Reciprocal = frac2.Reciprocal;
            return Multiplication(frac1, frac2Reciprocal);

        }

        /// <summary>
        /// Maintains precision as they are still fractions 
        /// and thus are ratios of integers. Whereas converting to decimals
        /// would require rounding, so precision is lost
        /// e.g 1/3 = 0.33333..., converting to to 0.33 reduces preicision.
        /// </summary>
        /// <param name="other">The fraction to compare to.</param>
        /// <returns>A negative value if this is less than other, zero if equal, positive if greater.</returns>
        public int CompareTo(Fraction other)
        {
            long lcm = CalculateLCM(Denominator, other.Denominator);
            long thisNumerator = Numerator * (lcm / Denominator);
            long otherNumerator = other.Numerator * (lcm / other.Denominator);

            return thisNumerator.CompareTo(otherNumerator);
        }

        /// <summary>
        /// Ensures that equivalent fractions produce the same hash code, 
        /// enabling them to be stored and retrieved correctly in hash-based collections 
        /// </summary>
        /// <returns>A hash code for this fraction.</returns>
        public override int GetHashCode()
        {
            var normalized = GetSimplifiedFraction();
            var improper = normalized.WholePart != 0 
                ? MixedToFraction(normalized.WholePart, normalized.Numerator, normalized.Denominator) 
                : normalized;
            
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + improper.Numerator.GetHashCode();
                hash = hash * 23 + improper.Denominator.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to this fraction.
        /// </summary>
        /// <param name="obj">The object to compare with this fraction.</param>
        /// <returns>True if the specified object is equal to this fraction; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Fraction)
            {
                Fraction other = (Fraction)obj;
                return this.Equals(other);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified fraction is mathematically equal to this fraction.
        /// Compares normalized improper fractions (e.g., 1/2 equals 2/4).
        /// </summary>
        /// <param name="other">The fraction to compare with this fraction.</param>
        /// <returns>True if the fractions are mathematically equal; otherwise, false.</returns>
        public bool Equals(Fraction other)
        {
            var thisNormalized = GetSimplifiedFraction();
            var otherNormalized = other.GetSimplifiedFraction();
    
            var thisImproper = thisNormalized.WholePart != 0 
                ? MixedToFraction(thisNormalized.WholePart, thisNormalized.Numerator, thisNormalized.Denominator) 
                : thisNormalized;
            var otherImproper = otherNormalized.WholePart != 0 
                ? MixedToFraction(otherNormalized.WholePart, otherNormalized.Numerator, otherNormalized.Denominator) 
                : otherNormalized;
    
            return thisImproper.Numerator == otherImproper.Numerator && thisImproper.Denominator == otherImproper.Denominator;
        }

        /// <summary>
        /// Determines whether two fractions are mathematically equal.
        /// </summary>
        /// <param name="left">The first fraction.</param>
        /// <param name="right">The second fraction.</param>
        /// <returns>True if the fractions are equal; otherwise, false.</returns>
        public static bool operator ==(Fraction left, Fraction right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two fractions are not mathematically equal.
        /// </summary>
        /// <param name="left">The first fraction.</param>
        /// <param name="right">The second fraction.</param>
        /// <returns>True if the fractions are not equal; otherwise, false.</returns>
        public static bool operator !=(Fraction left, Fraction right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether the first fraction is less than the second.
        /// </summary>
        /// <param name="left">The first fraction.</param>
        /// <param name="right">The second fraction.</param>
        /// <returns>True if left is less than right; otherwise, false.</returns>
        public static bool operator <(Fraction left, Fraction right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Determines whether the first fraction is less than or equal to the second.
        /// </summary>
        /// <param name="left">The first fraction.</param>
        /// <param name="right">The second fraction.</param>
        /// <returns>True if left is less than or equal to right; otherwise, false.</returns>
        public static bool operator <=(Fraction left, Fraction right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Determines whether the first fraction is greater than the second.
        /// </summary>
        /// <param name="left">The first fraction.</param>
        /// <param name="right">The second fraction.</param>
        /// <returns>True if left is greater than right; otherwise, false.</returns>
        public static bool operator >(Fraction left, Fraction right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Determines whether the first fraction is greater than or equal to the second.
        /// </summary>
        /// <param name="left">The first fraction.</param>
        /// <param name="right">The second fraction.</param>
        /// <returns>True if left is greater than or equal to right; otherwise, false.</returns>
        public static bool operator >=(Fraction left, Fraction right)
        {
            return left.CompareTo(right) >= 0;
        }


    }
}
