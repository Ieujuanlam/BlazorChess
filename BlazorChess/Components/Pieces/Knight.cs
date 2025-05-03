using BlazorChess.Components.Grid;
using static BlazorChess.Components.Pieces.PieceColours;

namespace BlazorChess.Components.Pieces
{
	public class Knight(PieceColours.PieceColour colour) : ChessPiece(colour, colour == PieceColour.White ? "♘" : "♞")
	{

		public override List<(int row, int col)> CalculateRawMoves(ChessGrid board)
		{
			var moves = new List<(int, int)>();
			int[,] offsets = new int[,]
			{
			{ 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 },
			{ 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 }
			};//jumpy bois

			for (int i = 0; i < offsets.GetLength(0); i++)
			{
				int r = Row + offsets[i, 0];
				int c = Column + offsets[i, 1];

				if (board.IsInBounds(r, c))
				{
					var piece = board.Grid[r, c];
					if (piece == null || piece.Colour != this.Colour)
					{
						moves.Add((r, c));
					}
				}
			}

			return moves;
		}

		public override ChessPiece Clone()
		{
			return new Knight(Colour)
			{
				Row = Row,
				Column = Column,
				PieceType = PieceType,
				HasMoved = HasMoved
			};
		}
	}
}
