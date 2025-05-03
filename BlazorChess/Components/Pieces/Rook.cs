using BlazorChess.Components.Grid;
using static BlazorChess.Components.Pieces.PieceColours;

namespace BlazorChess.Components.Pieces
{
	public class Rook(PieceColours.PieceColour colour) : ChessPiece(colour, colour == PieceColour.White ? "♖" : "♜")
	{

		public override List<(int row, int col)> CalculateRawMoves(ChessGrid board)
		{
			List<(int, int)> moves = [];
			AddLineMoves(moves, board, 1, 0); // Down
			AddLineMoves(moves, board, -1, 0); // Up
			AddLineMoves(moves, board, 0, 1); // Right
			AddLineMoves(moves, board, 0, -1); // Left
			return moves;
		}


		public override ChessPiece Clone()
		{
			return new Rook(Colour)
			{
				Row = Row,
				Column = Column,
				PieceType = PieceType,
				HasMoved = HasMoved
			};
		}
	}
}
