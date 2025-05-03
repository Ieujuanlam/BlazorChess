using BlazorChess.Components.Grid;
using static BlazorChess.Components.Pieces.PieceColours;

namespace BlazorChess.Components.Pieces
{
	public class Bishop(PieceColours.PieceColour colour) : ChessPiece(colour, colour == PieceColour.White ? "♗" : "♝")
	{

		public override List<(int row, int col)> CalculateRawMoves(ChessGrid board)
		{
			List<(int, int)> moves = [];
			AddLineMoves(moves, board, 1, 1); 
			AddLineMoves(moves, board, 1, -1); 
			AddLineMoves(moves, board, -1, 1); 
			AddLineMoves(moves, board, -1, -1); 
			return moves;
		}

		public override ChessPiece Clone()
		{
			return new Bishop(Colour)
			{
				Row = Row,
				Column = Column,
				PieceType = PieceType,
				HasMoved = HasMoved
			};
		}
	}
}
