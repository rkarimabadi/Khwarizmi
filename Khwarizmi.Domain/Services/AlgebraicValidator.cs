using Khwarizmi.Domain.Entities;
using Khwarizmi.Domain.ValueObjects;

namespace Khwarizmi.Domain.Services
{
    public class AlgebraicValidator
    {
        public bool IsSolutionValid(string targetEquation, List<Tile> placedTiles)
        {
            var currentLayoutExpression = BuildExpressionFromTiles(placedTiles);

            return NormalizeExpression(targetEquation) == NormalizeExpression(currentLayoutExpression);
        }

        private string BuildExpressionFromTiles(List<Tile> tiles)
        {
            var terms = new Dictionary<string, int>();

            foreach (var tile in tiles)
            {
                string termKey = GetTermKey(tile.Dimensions.WidthVar.Type, tile.Dimensions.HeightVar.Type);
                if (terms.ContainsKey(termKey))
                    terms[termKey]++;
                else
                    terms[termKey] = 1;
            }

            return string.Join(" + ", terms.Select(kvp => $"{(kvp.Value == 1 ? "" : kvp.Value.ToString())}{kvp.Key}"));
        }

        private string GetTermKey(VariableType v1, VariableType v2)
        {
            if (v1 == VariableType.None && v2 == VariableType.None) return "1";

            var types = new List<string> { v1.ToString().ToLower(), v2.ToString().ToLower() }
                .Where(t => t != "none")
                .OrderBy(t => t)
                .ToList();

            if (types.Count == 1) return types[0];
            if (types.Count == 2)
            {
                if (types[0] == types[1]) return $"{types[0]}^2";
                return $"{types[0]}{types[1]}";
            }

            return "1";
        }

        private string NormalizeExpression(string expression)
        {
            // حذف فضاها و مرتب‌سازی جملات برای مقایسه دقیق
            return new string(expression.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();
        }

        public bool IsPerfectGeometricalShape(List<Tile> tiles)
        {
            if (!tiles.Any()) return false;

            int minX = tiles.Min(t => t.Position!.X);
            int maxX = tiles.Max(t => t.Position!.X + t.Dimensions.Width);
            int minY = tiles.Min(t => t.Position!.Y);
            int maxY = tiles.Max(t => t.Position!.Y + t.Dimensions.Height);

            int totalArea = (maxX - minX) * (maxY - minY);
            int sumTilesArea = tiles.Sum(t => t.Dimensions.Width * t.Dimensions.Height);

            return totalArea == sumTilesArea;
        }
    }
}