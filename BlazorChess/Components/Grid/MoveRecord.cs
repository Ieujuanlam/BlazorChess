using BlazorChess.Components.Pieces;

namespace BlazorChess.Components.Grid
{
	public class MoveRecord
	{
		public ChessPiece Piece { get; }
		public int StartRow { get; }
		public int StartCol { get; }
		public int EndRow { get; }
		public int EndCol { get; }
		public ChessPiece? CapturedPiece { get; }
		public bool WasFirstMove { get; }

		public int TurnNum { get; }
		public string MoveNotation { get; }

		public ChessGrid BoardSnapshot { get; }

		public MoveRecord(ChessPiece piece, int startRow, int startCol, int endRow, int endCol, ChessPiece? capturedPiece, int moveNum, ChessGrid board)
		{
			Piece = piece;
			StartRow = startRow;
			StartCol = startCol;
			EndRow = endRow;
			EndCol = endCol;
			CapturedPiece = capturedPiece;
			WasFirstMove = !piece.HasMoved;
			TurnNum = moveNum;
			BoardSnapshot = board;
			MoveNotation = GenerateNotation();
		}

		private string GenerateNotation()
		{
			string file(int col) => ((char)('a' + col)).ToString();
			string rank(int row) => (row + 1).ToString();

			string pieceSymbol = Piece switch
			{
				Pawn => "",
				Knight => "N",
				Bishop => "B",
				Rook => "R",
				Queen => "Q",
				King => "K",
				_ => ""
			};

			string capture = CapturedPiece != null ? "x" : "";

			if (Piece is King && Math.Abs(EndCol - StartCol) == 2)
				return EndCol == 6 ? "O-O" : "O-O-O";

			string fromFile = file(StartCol);
			string fromRank = rank(StartRow);
			string toFile = file(EndCol);
			string toRank = rank(EndRow);

			if (Piece is Pawn && CapturedPiece != null)
				return $"{fromFile}x{toFile}{toRank}";
			if(Piece is Rook || Piece is Queen)
			{
			}

			return $"{pieceSymbol}{capture}{toFile}{toRank}";
		}
	}
}
