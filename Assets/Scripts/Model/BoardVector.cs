using System;
using System.Collections.Generic;


namespace Chess
{
    public struct BoardVector : IEquatable<BoardVector>
    {

        public const string horizontalCharacters = "abcdefgh";

        public static BoardVector Zero => new BoardVector(0);

        public static IEnumerable<BoardVector> GetRange(BoardVector size)
        {
            for (int v = 0; v < size.vertical; v++)
            {
                for (int h = 0; h < size.horizontal; h++)
                {
                    yield return new BoardVector(h, v);
                }
            }
        }

        public static int NormalizeInt(int number)
        {
            return (number != 0) ? ((number > 0) ? 1 : -1) : 0;
        }


        public int horizontal, vertical;


        public bool IsZero => horizontal == 0 && vertical == 0;
        public BoardVector Normalized => new BoardVector(NormalizeInt(horizontal), NormalizeInt(vertical));


        public BoardVector(int size) : this(size, size) { }

        public BoardVector(int horizontal, int vertical)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
        }

        public BoardVector(string stringCoords)
        {
            string parsingCoord = stringCoords.Substring(0, 1);
            horizontal =
                (short)horizontalCharacters.IndexOf(parsingCoord, System.StringComparison.InvariantCultureIgnoreCase);

            // Make sure horizontal coords is valid
            if (horizontal < 0)
                throw new System.ArgumentException($"Horizontal coord not valid: {parsingCoord}", nameof(stringCoords));

            parsingCoord = stringCoords.Substring(1);
            if (int.TryParse(parsingCoord, out vertical))
            {
                // Since this represents the coordinates as a1 -> (0, 0) substract 1 to the horizontal parsed coord
                vertical--;
            }
            else throw new System.ArgumentException($"Vertical coord not valid: {parsingCoord}", nameof(stringCoords));
        }


        public bool IsInsideBox(BoardVector boxLength)
        {
            return IsInsideBox(boxLength.horizontal, boxLength.vertical);
        }

        public bool IsInsideBox(int horizontalLength, int verticalLength)
        {
            return horizontal >= 0 && vertical >= 0 && horizontal < horizontalLength && vertical < verticalLength;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is BoardVector)) return false;

            return Equals((BoardVector)obj);
        }

        public bool Equals(BoardVector other)
        {
            return horizontal == other.horizontal && vertical == other.vertical;
        }

        public override int GetHashCode()
        {
            var hashCode = -1785799412;
            hashCode = hashCode * -1521134295 + horizontal.GetHashCode();
            hashCode = hashCode * -1521134295 + vertical.GetHashCode();
            return hashCode;
        }


        public override string ToString()
        {
            return $"({horizontal}, {vertical})";
        }

        public string ToStringCoordinates(bool includeNumericCoords = false)
        {
            if (horizontal < 0 || vertical < 0) return $"Invalid coords ({horizontal}, {vertical})";

            var stringBuilder = new System.Text.StringBuilder($"{horizontalCharacters[horizontal]}{vertical + 1}");
            if (includeNumericCoords)
            {
                stringBuilder.Append(' ');
                stringBuilder.Append(this.ToString());
            }

            return stringBuilder.ToString();
        }


        // Operators

        public static bool operator ==(BoardVector left, BoardVector right) => left.Equals(right);

        public static bool operator !=(BoardVector left, BoardVector right) => !(left == right);

        public static BoardVector operator +(BoardVector left, BoardVector right)
        {
            return new BoardVector(left.horizontal + right.horizontal, left.vertical + right.vertical);
        }

        public static BoardVector operator -(BoardVector left, BoardVector right)
        {
            return new BoardVector(left.horizontal - right.horizontal, left.vertical - right.vertical);
        }

        public static BoardVector operator *(BoardVector boardVector, int multiplier)
        {
            return new BoardVector(boardVector.horizontal * multiplier, boardVector.vertical * multiplier);
        }

        public static BoardVector operator /(BoardVector boardVector, int divisor)
        {
            return new BoardVector(boardVector.horizontal / divisor, boardVector.vertical / divisor);
        }

    }
}
