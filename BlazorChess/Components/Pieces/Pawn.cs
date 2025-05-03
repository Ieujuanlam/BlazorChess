using BlazorChess.Components.Grid;
using System.Drawing;
using static BlazorChess.Components.Pieces.PieceColours;

namespace BlazorChess.Components.Pieces
{
	public class Pawn(PieceColours.PieceColour colour) : ChessPiece(colour, colour == PieceColour.White ? "♙" : "♟")
	{

		public override List<(int row, int col)> CalculateRawMoves(ChessGrid board)
		{
			var moves = new List<(int, int)>();
			int direction = Colour == PieceColour.White ? 1 : -1;
			int startRow = Row;
			int startCol = Column;

			// one square forward
			if (board.GetPiece(startRow + direction, startCol) == null)
				moves.Add((startRow + direction, startCol));

			// wwo squares forward from start
			if (!HasMoved &&
				board.GetPiece(startRow + 2 * direction, startCol) == null)
			{
				moves.Add((startRow + 2 * direction, startCol));
			}

			// captures
			foreach (int colOffset in new[] { -1, 1 })
			{
				int newCol = startCol + colOffset;
				if (newCol < 0 || newCol > 7) continue;

				// diagonal capture
				var piece = board.GetPiece(startRow + direction, newCol);
				if (piece != null && piece.Colour != Colour)
				{
					moves.Add((startRow + direction, newCol));
				}
				// en passant
				else 
				if (board.LastMovedPiece is Pawn lastPawn &&
						 lastPawn.Colour != Colour &&
						 lastPawn.Row == startRow &&
						 lastPawn.Column == newCol &&
						 Math.Abs(lastPawn.PreviousRow - lastPawn.Row) == 2)
				{
					moves.Add((startRow + direction, newCol));
				}
			}

			return moves;
		}

		public override ChessPiece Clone()
		{
			return new Pawn(Colour)
			{
				Row = Row,
				Column = Column,
				PieceType = PieceType,
				HasMoved = HasMoved,
				PreviousRow = PreviousRow
			};
		}
	}
}
