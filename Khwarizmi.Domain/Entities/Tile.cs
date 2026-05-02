using Khwarizmi.Domain.ValueObjects;

namespace Khwarizmi.Domain.Entities
{
    public class Tile
    {
        public Guid Id { get; private set; }
        public TileType Type { get; private set; }
        public Dimensions Dimensions { get; private set; }
        public Coordinate? Position { get; private set; }

        public Tile(TileType type, Dimensions dimensions)
        {
            ValidateInvariants(type, dimensions);

            Id = Guid.NewGuid();
            Type = type;
            Dimensions = dimensions;
        }

        public void Rotate()
        {
            var newDimensions = new Dimensions(Dimensions.HeightVar, Dimensions.WidthVar);

            ValidateInvariants(Type, newDimensions);

            Dimensions = newDimensions;
        }

        private void ValidateInvariants(TileType type, Dimensions dimensions)
        {
            var widthVar = dimensions.WidthVar;
            var heightVar = dimensions.HeightVar;

            switch (type)
            {
                case TileType.Unit:
                    if (widthVar.Type != VariableType.None || heightVar.Type != VariableType.None)
                        throw new ArgumentException("Unit tile cannot have variables.");
                    if (dimensions.Width != 1 || dimensions.Height != 1)
                        throw new ArgumentException("Unit tile must be 1x1.");
                    break;

                case TileType.Square:
                    if (widthVar.Type == VariableType.None || heightVar.Type == VariableType.None)
                        throw new ArgumentException("Square type is for variables only.");
                    if (!dimensions.IsSquare)
                        throw new ArgumentException("Square tiles must have identical variables and values on both sides.");
                    break;

                case TileType.Rectangle:
                    if (widthVar.Type == VariableType.None && heightVar.Type == VariableType.None)
                        throw new ArgumentException("Rectangle cannot be two constants. Use Unit.");

                    if (widthVar.Type == heightVar.Type && widthVar.Type != VariableType.None)
                        throw new ArgumentException("For identical variables, use Square type.");
                    break;
            }
        }

        public void SetPosition(Coordinate position)
        {
            Position = position;
        }

        public void RemoveFromBoard()
        {
            Position = null;
        }
    }
}