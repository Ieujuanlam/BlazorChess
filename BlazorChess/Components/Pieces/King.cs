using BlazorChess.Components.Grid;
using System.Net.NetworkInformation;
using static BlazorChess.Components.Pieces.PieceColours;

namespace BlazorChess.Components.Pieces
{
	public class King(PieceColours.PieceColour colour) : ChessPiece(colour, colour == PieceColour.White ? "♔" : "♚")
	{

		public override List<(int row, int col)> CalculateRawMoves(ChessGrid board)
		{
			List<(int, int)> moves = [];
			AddLineMoves(moves, board, 1, 0, 1);
			AddLineMoves(moves, board, -1, 0, 1);
			AddLineMoves(moves, board, 0, 1, 1); 
			AddLineMoves(moves, board, 0, -1, 1); 
			AddLineMoves(moves, board, 1, 1, 1);
			AddLineMoves(moves, board, 1, -1, 1);
			AddLineMoves(moves, board, -1, 1, 1);
			AddLineMoves(moves, board, -1, -1, 1);
			AddCastleMoves(moves, board, true);
			AddCastleMoves(moves, board, false);
			return moves;
		}
		public void AddCastleMoves(List<(int, int)> moves, ChessGrid board, bool isKingSide)
		{
			if (HasMoved)
				return;

			int rookCol = isKingSide ? 7 : 0;
			int direction = isKingSide ? 1 : -1;
			if (board.GetPiece(Row, rookCol) is not Rook rook || rook.HasMoved)
				return;
			for (int i = 1; i <= 2; i++)
			{
				int col = Column + (i * direction);
				if (board.GetPiece(Row, col) != null)
					return;
			}
			moves.Add((Row, Column + 2 * direction));
		}
		

		public override ChessPiece Clone()
		{
			return new King(Colour)
			{
				Row = Row,
				Column = Column,
				PieceType = PieceType,
				HasMoved = HasMoved
			};
		}
	}
}
